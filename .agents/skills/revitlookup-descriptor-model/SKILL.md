---
name: revitlookup-descriptor-model
description: >
  Teach RevitLookup to inspect a new Revit type by adding a LookupEngine descriptor and registering it in the type map, giving the object resolved members, synthetic extensions, and context-menu actions.
  USE FOR: creating a descriptor class, resolving members that need arguments or the active view, disabling or marking members unsupported, registering synthetic extension members, redirecting one type to another, adding context-menu commands that touch Revit through an external event, and wiring the descriptor into DescriptorsMap.
  DO NOT USE FOR: building a UI page or view model (use revitlookup-ui-development), or changing the LookupEngine engine itself in the submodule.
license: MIT
---

# RevitLookup Descriptor Model

RevitLookup decomposes any object through LookupEngine, and a descriptor is how it teaches the engine about one Revit type.
Add support for a type by writing a descriptor and adding one `case` to the type map — never by special-casing the type anywhere else.
Descriptors live in `source/RevitLookup/Core/Decomposition/Descriptors/`; the worked reference is `ElementDescriptor.cs`.

## When to use

- Adding or changing how a Revit type decomposes: resolving members, exposing synthetic members, redirecting, or adding context-menu actions.

## When not to use

- Changing the decompose pipeline, the never-throw contract, or the `Variants` API, which live in the `source/LookupEngine` submodule.

## Workflow

### Step 1: Create the descriptor and set its name

Add `XxxDescriptor.cs` deriving from `Descriptor` (`LookupEngine.Abstractions.Decomposition`), or from RevitLookup's `ResolvingDescriptor` when you need its protected `Resolve...` helpers.
Capture the object in the constructor and set `Name` to a human label.

```csharp
public sealed class WallDescriptor : Descriptor
{
    private readonly Wall _wall;

    public WallDescriptor(Wall wall)
    {
        _wall = wall;
        Name = $"{wall.Name}, ID{wall.Id}";
    }
}
```

If the type derived from another type and you want to reuse its constructor, inherit from it:

```csharp
public sealed class FamilyDescriptor(Family family) : ElementDescriptor(family)
{
    //No constructor, the Name policy derived from ElementDescriptor.
}
```

### Step 2: Opt into behavior through the configuration interfaces

Implement only the interfaces the type needs:

- `IDescriptorConfigurator` and `IDescriptorConfigurator<Document>` — `Configure(IMemberConfigurator)` to shape members.
- `IDescriptorRedirector<Document>` — `TryRedirect(string target, Document context, out object result)` hands off to a richer type, as `ElementIdDescriptor` resolves an `ElementId` to its `Element`.
- `IContextMenuConnector` — `RegisterMenu(...)` to add actions in the UI.
- `IDescriptorCollector` — a marker for UI that the object exposes members worth drilling into.

### Step 3: Resolve, disable, and register members in Configure

Use `configuration.Member(nameof(...))` to `Resolve(...)` a member the engine cannot invoke blindly (it needs arguments or the active view), and `configuration.Extension(name)` to `Register(...)` a synthetic member, `.Map(...)` a display name, or mark it `.NotSupported()`.
Build multi-value results with `Variants.Value(...)`, `Variants.Values<T>(capacity).Add(...).Consume()`, or `Variants.Empty<T>()`.
Gate a version-specific member for the Revit version it targets.

```csharp
public void Configure(IMemberConfigurator configuration)
{
    configuration.Member(nameof(Wall.Dispose)).Disable();
    configuration.Member("BoundingBox").Resolve(() => Variants.Values<BoundingBoxXYZ>(2)
        .Add(_wall.get_BoundingBox(null), "Model")
        .Add(_wall.get_BoundingBox(RevitContext.ActiveView), "Active view")
        .Consume());

    configuration.Extension(nameof(JoinGeometryUtils.GetJoinedElements)).Register(() => JoinGeometryUtils.GetJoinedElements(_wall.Document, _wall));
}
```

### Step 4: Call Revit only on the Revit thread

A context-menu command that reads or mutates the model raises an `[ExternalEvent(AllowDirectInvocation = true)]`; never call the Revit API from the UI thread or through `Dispatcher.Invoke`.
Log on a failure path through a source-generated `[LoggerMessage]`.

```csharp
public void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
{
    contextMenu.AddMenuItem("SelectMenuItem")
        .SetCommand(_wall, wall => SelectElementEvent.Raise(wall))
        .SetShortcut(Key.F6);
}

[ExternalEvent(AllowDirectInvocation = true)]
private static void SelectElement(UIApplication application, Element element)
{
    if (application.ActiveUIDocument is null) return;
    application.ActiveUIDocument.Selection.SetElementIds([element.Id]);
}
```

### Step 5: Register the descriptor in DescriptorsMap

Add a `case` to the `FindDescriptor` switch in `source/RevitLookup/Core/Decomposition/DescriptorsMap.cs`, guarding on the exact type; it matches both the approximate (display) and exact (member-resolution) lookups.

```csharp
Wall value when type is null || type == typeof(Wall) => new WallDescriptor(value),
```

Place the case in the matching category block; the default arm stays `_ => new ObjectDescriptor(obj)`.

### Step 6: Verify

Add a test in `tests/RevitLookup.Tests` covering the new resolution, then run the add-in in a `Debug.R##` configuration and decompose an instance of the type inside Revit.

```shell
dotnet test -c Debug.R##
```

## Validation

- [ ] The descriptor lives in `Core/Decomposition/Descriptors/`, captures its object, and sets `Name`.
- [ ] It implements only the configuration interfaces it needs; the resolution context is `Document`.
- [ ] `Configure` resolves members that need arguments or the active view, disables noise, and registers synthetic extensions; version-specific members are `#if`-gated.
- [ ] A menu command that touches Revit raises an `[ExternalEvent]` method, not a direct API call on the UI thread; failures log through a source-generated `[LoggerMessage]`.
- [ ] The type is registered with a single `case` in `DescriptorsMap.FindDescriptor`, guarded for the exact type.
- [ ] A test covers the change and it decomposes correctly in a `Debug.RNN` run.

## Common Pitfalls

| Pitfall                                                           | Correct approach                                                                          |
|-------------------------------------------------------------------|-------------------------------------------------------------------------------------------|
| Special-casing a type outside a descriptor                        | Add a descriptor and one `DescriptorsMap` case; keep type knowledge in the descriptor.    |
| Letting the engine invoke a member that needs arguments or a view | `Resolve` it explicitly with `Variants`, or `Disable` it.                                 |
| Calling the Revit API from a menu handler on the UI thread        | Raise a static `[ExternalEvent]` method; never `Dispatcher.Invoke` for Revit work.        |
| Registering the descriptor case without the exact-type guard      | Guard with `type is null \|\| type == typeof(T)`; member resolution needs the exact type. |
| Silent exception handling                                         | Declare a source-generated `[LoggerMessage]` method.                                      |
