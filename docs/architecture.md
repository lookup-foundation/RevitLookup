# Architecture & Environments

The project runs in two distinct environments. Code must be written to be compatible with both where possible (UI/Logic), or isolated where specific (Revit API).

## 1. Revit Environment (Production)

* **Context:** Runs inside the `Revit.exe` process.
* **Entry Point:** `RevitLookup`.
* **Constraints:** Single-threaded, specialized API access.

## 2. Playground Environment (Prototyping)

* **Context:** Runs as a standalone WPF application.
* **Entry Point:** `RevitLookup.UI.Playground`.
* **Purpose:** Rapid UI development, testing, and mocking without the overhead of restarting Revit.

## 3. LookupEngine Architecture

RevitLookup is built on the [LookupEngine](https://github.com/lookup-foundation/LookupEngine) framework, which provides a system for analyzing object structures at runtime.

### Descriptors

Descriptors are specialized classes that define how objects should be handled by the LookupEngine. Each descriptor is responsible for a specific type or family of types in Revit.

To add a descriptor for a new class:

1. Create a new descriptor class in the appropriate folder under `source/RevitLookup/Core/Decomposition/Descriptors/`
2. Register the descriptor in the descriptor map located at `source/RevitLookup/Core/Decomposition/DescriptorsMap.cs`

### IDescriptorResolver

This interface allows descriptors to control how methods and properties with parameters are evaluated.
In RevitLookup, `Document` serves as the context for resolution.

**Single Value Resolution:**

```csharp
// source/RevitLookup/Core/Decomposition/Descriptors/ElementDescriptor.cs
public class ElementDescriptor(Element element) : Descriptor, IDescriptorResolver<Document>
{
    public virtual Func<Document, IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(Element.IsHidden) => context => Variants.Value(element.IsHidden(context.ActiveView), "Active view"),
            _ => null
        };
    }
}
```

**Multiple Value Resolution:**

```csharp
// source/RevitLookup/Core/Decomposition/Descriptors/ElementDescriptor.cs
public class ElementDescriptor(Element element) : Descriptor, IDescriptorResolver<Document>
{
    public virtual Func<Document, IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            "BoundingBox" => ResolveBoundingBox,
            _ => null
        };

        IVariant ResolveBoundingBox(Document context)
        {
            return Variants.Values<BoundingBoxXYZ>(2)
                .Add(element.get_BoundingBox(null), "Model")
                .Add(element.get_BoundingBox(context.ActiveView), "Active view")
                .Consume();
        }
    }
}
```

**Disabling Methods:**

```csharp
// source/RevitLookup/Core/Decomposition/Descriptors/DocumentDescriptor.cs
public class DocumentDescriptor(Document document) : Descriptor, IDescriptorResolver
{
    public virtual Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(Document.Close) => Variants.Disabled,
            _ => null
        };
    }
}
```

**Targeting Specific Overloads:**

```csharp
// source/RevitLookup/Core/Decomposition/Descriptors/EntityDescriptor.cs
public sealed class EntityDescriptor(Entity entity) : Descriptor, IDescriptorResolver
{
    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(Entity.Get) when parameters.Length == 1 &&
                                    parameters[0].ParameterType == typeof(string) => ResolveGetByField,
            _ => null
        };

        IVariant ResolveGetByField()
        {
            return Variants.Value(entity.Get("Parameter Name"));
        }
    }
}
```

### IDescriptorExtension

This interface allows adding custom methods and properties to objects that don't exist in the original type.
In RevitLookup, extensions can use the `Document` as a context during registration for document-dependent elements.

```csharp
// source/RevitLookup/Core/Decomposition/Descriptors/ColorDescriptor.cs
public sealed class ColorDescriptor(Color color) : Descriptor, IDescriptorExtension
{
    public void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define("HEX").Register(() => Variants.Value(ColorRepresentationUtils.ColorToHex(color)));
        manager.Define("RGB").Register(() => Variants.Value(ColorRepresentationUtils.ColorToRgb(color)));
        manager.Define("HSL").Register(() => Variants.Value(ColorRepresentationUtils.ColorToHsl(color)));
    }
}
```

With context:

```csharp
// source/RevitLookup/Core/Decomposition/Descriptors/SchemaDescriptor.cs
public sealed class SchemaDescriptor(Schema schema) : Descriptor, IDescriptorExtension<Document>
{
    public void RegisterExtensions(IExtensionManager<Document> manager)
    {
        manager.Define("GetElements").Register(context => Variants.Value(context.CollectElements()
            .WithExtensibleStorage(schema.GUID)
            .ToElements()));
    }
}
```

### IDescriptorRedirector

This interface lets a descriptor redirect to another object. For example, converting from an ID to the actual element.

```csharp
// source/RevitLookup/Core/Decomposition/Descriptors/ElementIdDescriptor.cs
public sealed class ElementIdDescriptor(ElementId elementId) : Descriptor, IDescriptorRedirector<Document>
{
    public bool TryRedirect(string target, Document context, out object result)
    {
        result = elementId;
        if (elementId == ElementId.InvalidElementId)
        {
            return false;
        }

        var element = elementId.ToElement(context);
        if (element is null) return false;

        result = element;
        return true;
    }
}
```

### IDescriptorCollector

This interface serves as a marker indicating that the descriptor can decompose the object's members. It's essential for allowing users to inspect an object's internal structure.

```csharp
// source/RevitLookup/Core/Decomposition/Descriptors/WorksetDescriptor.cs
public sealed class WorksetDescriptor : Descriptor, IDescriptorCollector
{
    public WorksetDescriptor(Workset workset)
    {
        Name = workset.Name;
    }
}
```

### IContextMenuConnector

This interface enables integration with the RevitLookup UI, allowing descriptors to add custom context menu options and commands.

```csharp
// source/RevitLookup/Core/Decomposition/Descriptors/ElementDescriptor.cs
public sealed class ElementDescriptor : Descriptor, IContextMenuConnector
{
    public void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
    {
        contextMenu.AddMenuItem("ShowMenuItem")
            .SetCommand(_element, element => ShowElementEvent.Raise(element))
            .SetAvailability(_element is not ElementType)
            .SetShortcut(Key.F7);
    }
}
```
