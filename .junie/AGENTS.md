# RevitLookup Development Guidelines

RevitLookup is an inspection tool for Autodesk Revit. Core principles:

1. Dual Environments: Production (Revit.exe) and Playground (standalone WPF). Code must be environment-agnostic.
2. LookupEngine: Core logic resides in LookupEngine and must be decoupled from Revit UI.
3. Production Standards: Enterprise-grade C#, SOLID, and strict naming conventions.
4. No Revit UI in Core: Never reference Autodesk.Revit.UI in Engine or Abstractions.
5. Modern C#: Use latest features (Primary Constructors, Collection expressions).

## Specialized Guidelines

Before doing specialized work, read the matching file:

* [Project Structure](../docs/project-structure.md) – When searching for specific module responsibilities, adding new projects, submodules.
* [Architecture](../docs/architecture.md) – When extending LookupEngine (Descriptors, Resolvers, Extensions).
* [Code Style](../docs/code-style.md) – When writing or refactoring C# code, implementing MVVM (CommunityToolkit).
* [UI Development](../docs/ui-development.md) – When creating XAML templates, styling WPF components, or implementing custom type visualization.
* [Revit Best Practices](../docs/revit-best-practices.md) – When interacting with Revit API.
* [Testing Strategy](../docs/testing-strategy.md) – When writing TUnit tests for Revit or creating mocks for the Playground environment.
* [Package Management](../docs/package-management.md) – When adding, updating, or managing NuGet dependencies.
