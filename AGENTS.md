# RevitLookup Agent Instructions

RevitLookup is a public open-source Revit add-in that explores the Revit object database at runtime and shows every member of any element, type, or document. It runs in two environments from one shared UI: the production add-in inside `Revit.exe` and a standalone WPF Playground for prototyping without Revit. It consumes the LookupEngine submodule for decomposition, Nice3point.Revit.Toolkit and Nice3point.Revit.Extensions for the Revit API, CommunityToolkit.Mvvm for the UI, Microsoft.Extensions.Hosting with Scrutor for dependency injection, and Serilog for logging.

## Non-Negotiables

* **The two environments stay in sync.** Shared UI and logic compile and run under both the Revit add-in and the Playground. Revit-specific code lives behind an abstraction so the Playground supplies a mock in its place.
* **The Revit API stays out of the shared layers.** The abstractions, common utilities, and shared UI never reference `Autodesk.Revit.UI` or the Revit API. Revit access lives in the production add-in project alone.
* **The Revit API runs on the Revit thread.** Marshal cross-thread Revit work through the `[ExternalEvent]` source generator from the Toolkit. Never call the Revit API from a background thread and never use `Dispatcher.Invoke` for it.
* **Every type compiles for every supported Revit version.** Gate version-specific Revit APIs with `#if REVIT2024_OR_GREATER`-style directives. The supported range is Revit 2021 through 2027.
* **Extend through LookupEngine descriptors.** Add support for a Revit type with a descriptor that implements the engine's configuration interfaces, never with a special case elsewhere.
* **Production-grade C#.** Follow SOLID and DRY, keep nullable reference types satisfied, and use the MVVM, dependency-injection, and structured-logging patterns the app already follows. See [Code Style](./docs/code-style.md).
* **Verify unfamiliar APIs.** When unsure of a Revit or .NET API's behavior or signature, confirm it before use. Search the web for the official docs. To read a referenced library's source, query GitHub with `gh` (`gh api`, `gh search code`). If `gh` is unavailable, search the web or ask. Never inspect compiled DLLs or XML extracted from NuGet packages.
* **Tests ship with every change.** Cover custom logic with TUnit and exercise UI changes in the Playground. See [Testing](./docs/testing.md).
* **Keep docs in sync.** A user-facing change updates `README.md`, `CHANGELOG.md`, the wiki, and the affected XML docs in the same commit. See [Documentation](./docs/documentation.md).

## Build

The build is a ModularPipelines project. Run `dotnet run -c Release` from the `build` directory to compile.

## Specialized Docs

Read the matching file before related work.

* [Project Structure](./docs/project-structure.md). Solution layout, the shared and environment-specific projects, and change placement.
* [Architecture](./docs/architecture.md). The dual environments, dependency injection, and the LookupEngine decomposition model.
* [Code Style](./docs/code-style.md). C# conventions, MVVM, dependency injection, and structured logging.
* [UI Development](./docs/ui-development.md). WPF data templates, type customization, the ViewModel pattern, and the Playground.
* [Revit Best Practices](./docs/revit-best-practices.md). The Revit API context, threading, version directives, and the two environments.
* [Testing](./docs/testing.md). TUnit tests and the Playground as a UI test environment.
* [Documentation](./docs/documentation.md). XML docs, README, CHANGELOG, and the wiki.
* [Package Management](./docs/package-management.md). Centralized versions, Revit-version packages, and dependencies.
* [WinGet Publishing](./docs/winget-publishing.md). The one-time submission for a new Revit-year package.
