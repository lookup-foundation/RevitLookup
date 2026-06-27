# Code Style

Production C# only. This is a public application, so its code stays clean, readable, and robust.

## General Principles

* **SOLID and DRY.** One responsibility per type. Extract shared logic into the common or abstractions project rather than duplicate it.
* **Explicit over implicit.** Code is self-explanatory. Avoid hidden behavior and unclear defaults.
* **Nullable safety.** Nullable reference types are enabled solution-wide. Treat every nullability warning as a defect.
* **Span for memory-sensitive work.** Use `Span<T>` and `ReadOnlySpan<T>` for data processing outside business logic where they avoid allocations.
* **StyleCop style.** Follow StyleCop conventions for layout, member ordering, and spacing.

## Modern C#

`LangVersion` is `latest`. Reach for the newest feature that expresses the intent directly, and do not hand-roll what the language already provides.

* Primary constructors when a type captures state or injected dependencies.
* Collection expressions for literals and spans.
* Pattern matching and switch expressions over branching chains.
* Expression-bodied members for simple accessors.
* File-scoped namespaces.

## Comments

Public types and members carry XML doc comments, see [Documentation](./documentation.md). Inside the code, comments are the exception.

* Names and structure carry the meaning. Default to no comment.
* Add one only when the reason cannot be read from the code and a reader could break the code without it, such as a non-obvious invariant or a threading constraint.
* A comment explains why, never what. Do not restate the code.

## Attributes

Decorate members with every JetBrains and .NET attribute that carries meaning, so analyzers, the debugger, and callers read the full contract.

* `[UsedImplicitly]` on a type the container resolves by convention, so analysis does not flag it as unused.
* `[ObservableProperty]` and `[RelayCommand]` on the ViewModel members the CommunityToolkit source generator expands.
* `[ExternalEvent]` on a method that the Toolkit source generator wraps for the Revit thread. See [Revit Best Practices](./revit-best-practices.md).

## Naming

* **Clarity first.** Names are descriptive and never abbreviated: `element` not `elem`, `document` not `doc`, `repository` not `repo`, `configuration` not `config`, `context` not `ctx`.
* Follow the Revit API naming conventions for Revit-facing code.
* Methods that return `Task` or `Task<T>` end with `Async`.
* No single-letter variables except in a short loop or lambda.

## File and Class Structure

* **File-scoped namespaces** that match the project and folder, for example `namespace RevitLookup.Core.Decomposition;`.
* **Member order:** private fields, constructors, public properties, public methods, private methods.

## Dependency Injection

Both hosts register their services through `Microsoft.Extensions.Hosting`, and Scrutor scans the assemblies to register views and ViewModels by convention.
See [Architecture](./architecture.md).

* Inject dependencies through the constructor, with a primary constructor where practical.
* Do not declare a `private readonly` field only to mirror an injected service. Regular fields stay for instance state and constructor-derived values.
* Register a service against its abstraction so the production host and the Playground bind the same view to different implementations.

```csharp
public class ContentService(
    IContentRepository repository,
    ILogger<ContentService> logger) : IContentService
{
    public async Task<Content> GetAsync(string id)
    {
        logger.LogInformation("Getting content {Id}", id);
        return await repository.GetByIdAsync(id);
    }
}
```

## MVVM

The UI follows MVVM with `CommunityToolkit.Mvvm`.

* Generate observable state with `[ObservableProperty]` and commands with `[RelayCommand]`.
* A ViewModel implements a shared contract so the production host and the Playground each supply their own.
* Use `IMessenger` for loose coupling between ViewModels.

```csharp
public partial class SearchViewModel(ISearchService searchService) : ObservableObject, ISearchViewModel
{
    [ObservableProperty]
    public partial string Query { get; set; }

    [RelayCommand]
    private async Task SearchAsync()
    {
        // ...
    }
}
```

## Logging

Serilog is the logging backend, configured through the host.

* **Always log through source-generated `LoggerMessage` partial methods.** Never call `logger.LogInformation`, `logger.LogError`, `logger.LogDebug`, or the other `ILogger` extension methods directly in production code. This applies everywhere: ViewModels, services, descriptors, and hosted services.
* Mark the owning class `partial` and declare the log methods as `private static partial void` at the bottom of the class. Name each one with a `Log` prefix, take `ILogger<T>` as the first parameter, and pass the level positionally on the attribute.

```csharp
public sealed partial class HostBackgroundService(
    ISoftwareUpdateService updateService,
    ILogger<HostBackgroundService> logger)
    : IHostedService
{
    private async Task CheckUpdatesAsync()
    {
        try
        {
            if (!await updateService.CheckUpdatesAsync()) return;
            LogUpdateAvailable(logger, updateService.NewVersion);
        }
        catch (Exception exception)
        {
            LogUpdateServiceError(logger, exception);
        }
    }

    [LoggerMessage(LogLevel.Information, "RevitLookup {Version} is available to download")]
    private static partial void LogUpdateAvailable(ILogger<HostBackgroundService> logger, string? version);

    [LoggerMessage(LogLevel.Error, "Update service error")]
    private static partial void LogUpdateServiceError(ILogger<HostBackgroundService> logger, Exception exception);
}
```

Call the generated method by passing the logger explicitly, with the exception first when present:

```csharp
LogUpdateAvailable(logger, updateService.NewVersion);
LogUpdateServiceError(logger, exception);
```

* **Pass values as structured properties, never as a pre-formatted string.** Expose each value through a `{Property}` placeholder backed by a typed parameter, instead of building the message at the call site and logging it as a single string.
    * Bad: a method with the template `"{Message}"` and a `string message` parameter, called as `LogUpdateAvailable(logger, $"RevitLookup {version} is available")` — the version is lost as a property.
    * Good: template `"RevitLookup {Version} is available to download"` with a `string? version` parameter, called as `LogUpdateAvailable(logger, version)`.
* Choose the level on the `[LoggerMessage]` attribute. Use `LogLevel.Debug` for detailed diagnostics and reserve `LogLevel.Information` for meaningful lifecycle events.

## Error Handling

* **Validate inputs at the boundary.** Guard public methods, not internal call sites Revit already validates.
* **Let unexpected exceptions reach the boundary.** Do not scatter `try-catch` through business logic. A handler at the boundary reports the failure.
* **Define semantic exceptions** for expected failures rather than throw a bare `Exception`.

## Data Objects

* Use `record` for data-transfer objects, messages, and configuration objects.
* Use `{ get; init; }` for immutable properties on a class.
