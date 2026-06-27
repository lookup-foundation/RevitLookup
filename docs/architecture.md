# Architecture

RevitLookup inspects the Revit object database and presents every member of any object through a shared UI.
The same UI and logic run in two environments, drive their services through dependency injection, and decompose objects through the LookupEngine framework.

## The Two Environments

The application targets two hosts from one shared codebase.

* The **production** host loads inside `Revit.exe` as a Revit add-in. It supplies the real Revit-backed service and ViewModel implementations and owns every Revit API call.
* The **Playground** host runs as a standalone WPF application. It supplies mock implementations of the shared contracts so UI work needs no running Revit.

Both hosts display the same shared views.
A shared contract lives in the abstractions project, the production host implements it against the Revit API, and the Playground implements it with mock data.
Code that must reach the Revit API stays in the production host alone.
See [Revit Best Practices](./revit-best-practices.md) for the environment boundary and [Project Structure](./project-structure.md) for where each piece lives.

## Dependency Injection

Each host builds a `Microsoft.Extensions.Hosting` host and registers its services in a `Host` class.
The shared contracts resolve to the host-specific implementation, so the same view binds to a Revit-backed ViewModel in production and a mock ViewModel in the Playground.

* Views and ViewModels register automatically through Scrutor assembly scanning, so a new view or ViewModel needs no manual registration.
* Services register explicitly with the lifetime they need.
* Serilog provides structured logging through the host. See [Code Style](./code-style.md).

## LookupEngine

RevitLookup builds on [LookupEngine](https://github.com/LookupEngine/LookupEngine), the framework that decomposes a runtime object into its members through reflection and evaluates each value.
The engine records the time and memory each evaluation costs and never crashes on a reflection failure.
RevitLookup consumes the engine and teaches it about Revit types through descriptors.

### Descriptors

A descriptor defines how the engine handles one Revit type or family of types.
It implements the engine's configuration interfaces to control evaluation, add synthetic members, redirect to related objects, or connect to the RevitLookup UI.
Add support for a new Revit type with a new descriptor, never with a special case elsewhere.
In RevitLookup the active `Document` serves as the resolution context.

### Resolve a Value

`IDescriptorConfigurator` and `IDescriptorConfigurator<TContext>` let a descriptor control how an existing member is evaluated.
`configuration.Member(name)` selects the member, and `Resolve` supplies the value to display, optionally labeled.
The context-aware form receives the resolution context, the active `Document` in RevitLookup.

```csharp
public sealed class ElementDescriptor(Element element) : Descriptor, IDescriptorConfigurator<Document>
{
    public void Configure(IMemberConfigurator<Document> configuration)
    {
        configuration.Member(nameof(Element.IsHidden)).Resolve(document => Variants.Value(element.IsHidden(document.ActiveView), "Active view"));
    }
}
```

A resolver returns several labeled values through `Variants.Values`, filters overloads by their runtime parameters with `When`, and marks a member unsafe to evaluate with `Disable`.

### Add Synthetic Members

`IDescriptorConfigurator` and `IDescriptorConfigurator<TContext>` also add members that do not exist on the original type.
`configuration.Extension(name)` defines the synthetic member and `Register` supplies its value.
The context-aware form receives the `Document` for document-dependent members.

```csharp
public sealed class ColorDescriptor(Color color) : Descriptor, IDescriptorConfigurator
{
    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Extension("HEX").Register(() => ColorRepresentationUtils.ColorToHex(color));
        configuration.Extension("RGB").Register(() => ColorRepresentationUtils.ColorToRgb(color));
    }
}
```

### Redirect to Another Object

`IDescriptorRedirector<TContext>` redirects from one object to a related one, such as from an `ElementId` to the `Element` it identifies, so the user inspects the resolved object.

```csharp
public sealed class ElementIdDescriptor(ElementId elementId) : Descriptor, IDescriptorRedirector<Document>
{
    public bool TryRedirect(string target, Document context, out object result)
    {
        result = elementId;
        var element = elementId.ToElement(context);
        if (element is null) return false;

        result = element;
        return true;
    }
}
```

### Mark a Type Decomposable

`IDescriptorCollector` marks a descriptor whose object exposes members worth decomposing, so the user can drill into its internal structure.

### Connect to the UI

`IContextMenuConnector` lets a descriptor add context-menu commands to the RevitLookup UI.
Commands that touch the Revit API run through an external event. See [Revit Best Practices](./revit-best-practices.md).

```csharp
public sealed class ElementDescriptor : Descriptor, IContextMenuConnector
{
    public void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
    {
        contextMenu.AddMenuItem("ShowMenuItem")
            .SetCommand(_element, element => ShowElementEvent.Raise(element))
            .SetShortcut(Key.F7);
    }
}
```
