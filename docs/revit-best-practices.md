# Revit Best Practices

The production add-in runs inside the Revit process and must respect the API's threading and version constraints.
RevitLookup reaches the Revit API through Nice3point.Revit.Toolkit and Nice3point.Revit.Extensions, which remove the boilerplate and manage the lifecycle.

## The Revit API Context

The Toolkit exposes the active Revit application, document, and view through its static contexts, so code reaches them without an `ExternalCommandData` threaded through every call.
The add-in entry point inherits a Toolkit base class that handles the Revit plumbing and exposes a single override.

* Read the active document, view, and UI application from the Toolkit context rather than pass them down by hand.
* Use the Toolkit and the Extensions for Revit work before reaching for the raw API, so the add-in stays consistent and allocation-conscious.

## Threading

The Revit API may only be touched from the Revit thread.

* Marshal cross-thread Revit work through the `[ExternalEvent]` source generator from the Toolkit. The generator emits the event plumbing for an annotated method, and the call site raises it.
* Never call the Revit API from a background thread, and never use `Dispatcher.Invoke` for Revit work.
* A descriptor that runs a Revit command from a context-menu entry raises an external event rather than call the API directly. See [Architecture](./architecture.md).

## Revit Versions

The active Revit version comes from the build configuration, which selects the matching API packages and emits the version constants.
The supported range is Revit 2021 through 2027.

* Gate version-specific Revit APIs with `#if REVIT2024_OR_GREATER`-style directives, and only where the API genuinely differs between versions.
* Invert a constant to compile a block for older versions, such as `#if !REVIT2023_OR_GREATER`.
* Every type compiles under every supported version configuration.
* Version-specific package versions belong in `Directory.Packages.props`. See [Package Management](./package-management.md).

## The Environment Boundary

The Revit API stays inside the production add-in project.

* The abstractions, common, and shared UI projects never reference the Revit API. They depend on contracts the add-in implements.
* Isolate Revit-specific logic behind a contract so the Playground supplies a mock and the shared UI runs without Revit.
* When a feature needs the Revit API, put the implementation in the add-in and the contract in the abstractions project. See [Project Structure](./project-structure.md).
