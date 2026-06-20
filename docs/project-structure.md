# Project Structure

RevitLookup explores the Revit object database from one shared codebase that runs in two environments.
The solution separates the shared contracts and UI, the two host environments, the external decomposition engine, the tests, and the build and installer automation.
Keep each piece of code in the project that owns its responsibility.

## Solution Groups

* **Shared core.** The pure C# projects that carry no Revit reference.
    * The abstractions project holds the contracts: ViewModel interfaces, service interfaces, options, and observable models that both environments implement.
    * The common project holds general-purpose utilities and extensions.
    * The service-defaults project holds the host configuration shared between the two environments.
* **Frontend.** The shared UI.
    * The UI framework project holds the views, controls, converters, and base classes that both environments display. It carries no Revit reference.
* **Revit.** The production environment.
    * The add-in project is the entry point loaded inside `Revit.exe`. It owns every Revit API call, the descriptors, the Revit-backed service implementations, and the host registration. It is the only project that references the Revit API.
* **Playground.** The prototyping environment.
    * The Playground project is a standalone WPF application that hosts the shared UI with mock implementations of the shared contracts, so UI work needs no running Revit.
* **Engine.** The external decomposition framework, referenced as git submodules.
    * The LookupEngine submodule decomposes a runtime object into its members and evaluates each value. See [Architecture](./architecture.md).
    * The LookupEngine.UI submodule provides the base UI components the framework builds on.
* **Tests.** The verification project. See [Testing](./testing.md).
* **Automation.** The ModularPipelines build and the WiX installer projects.
* **Root.** Build and package configuration, the README and CHANGELOG, the agent guidelines, the wiki sources, and the CI workflows.

## Submodules

The two `Engine` projects are git submodules that live in external repositories.
Treat them as external dependencies.
Do not modify their source and do not document their internals here.
Consume their public surface the same way any package is consumed.

## Change Placement

* A contract that both environments share goes in the abstractions project. The Revit-backed implementation goes in the add-in project, and the mock goes in the Playground.
* A LookupEngine descriptor for a Revit type goes with the other descriptors in the add-in project. See [Architecture](./architecture.md).
* A view, control, or converter that both environments display goes in the UI framework project.
* A general-purpose helper with no Revit dependency goes in the common project.
* Host configuration shared by both environments goes in the service-defaults project.
* Test coverage goes in the test project. See [Testing](./testing.md).
