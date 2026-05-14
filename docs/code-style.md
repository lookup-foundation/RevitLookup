# Strict C# Production Style

All code must adhere to enterprise-grade standards. "It works" is not enough; it must be clean, readable, and robust.

## 1. General Principles

* **SOLID:** strictly follow Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, and Dependency Inversion.
* **DRY (Don't Repeat Yourself):** Extract common logic to `RevitLookup.Common` or `RevitLookup.Abstractions`.
* **Explicit over Implicit:** Code should be self-explanatory. Avoid "magic" behavior.
* **Modern C#:** Always use the latest language features.
* **Use Span:** Utilize `Span<T>` and `ReadOnlySpan<T>` for memory-efficient data processing where applicable outside of business logic.
* **JetBrains Annotations:** Use JetBrains Annotations where applicable to improve code analysis.

## 2. Naming Conventions

* **Clarity is King:** Names must be descriptive.
* **No Abbreviations:**
    * ❌ `repo`, `config`, `ctx`, `svc`
    * ✅ `repository`, `configuration`, `context`, `service`
* **No Single-Letter Variables:**
    * ❌ `p`, `i`, `e` (except in very short lambdas or for loops).
    * ✅ `property`, `element`, `exception`.
* **Async Suffix:** Methods returning `Task` or `Task<T>` must end with `Async`.
    * ✅ `GetDataAsync()`

## 3. Formatting & Layout

* **File-Scoped Namespaces:** Always use file-scoped namespaces matching the project/folder namespace (for example, `namespace RevitLookup.Core.Decomposition;`).
* **Nullable Reference Types:** Enabled project-wide. Treat warnings as errors.
* **Organization:**
    1. Private Fields (if strictly necessary)
    2. Primary Constructor
    3. Public Properties
    4. Public Methods
    5. Private Methods

## 4. Async/Await

* **Task:** Use `Task` everywhere. Avoid `async void` (except for top-level event handlers).
* **Context:** Be mindful of the SynchronizationContext. In UI/Revit, `await` returns to the main thread by default. Use `ConfigureAwait(false)` in pure library code (Core/Common) if it doesn't touch UI/Revit API.

## 5. Error Handling

* **Centralized Handling:** Do not clutter business logic with `try-catch` blocks. Let exceptions propagate to a global handler or boundary.
* **Custom Exceptions:** Define semantic exceptions (e.g., `ConfigurationMissingException`) rather than throwing generic `Exception`.
* **Validation:** Validate inputs at the boundary (public methods).

## 6. Data Objects

* **Immutability:** Use `record` for DTOs, messages, and configuration objects.
* **Properties:** Use `{ get; init; }` for immutable properties in classes.

## 7. Dependency Injection

Use constructor injection for dependencies.

* **Primary Constructors Preferred:** Use C# primary constructors for dependency injection when practical.
* **No Manual DI Fields:** Do not declare `private readonly` fields only to mirror injected services. Regular fields are fine for instance state and constructor-derived values.
* **Registration:**
    * Revit: `source/RevitLookup/Host.cs`
    * Playground: `source/RevitLookup.UI.Playground/Host.cs`.

```csharp
// ✅ Correct
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

## 8. MVVM Pattern (CommunityToolkit)

* **Framework:** `CommunityToolkit.Mvvm`.
* **Source Generators:** extensively use `[ObservableProperty]` and `[RelayCommand]`.
* **State:** Keep ViewModel state private, expose via generated properties.
* **Messaging:** Use `IMessenger` for loose coupling between ViewModels.

```csharp
public partial class SearchViewModel(ISearchService searchService) : ObservableObject
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

## 9. Logging & Telemetry

* **Serilog:** The standard logging backend.
* **OpenTelemetry:** Configured for tracing.
* **Structured Logging:** **MANDATORY**. Never use string interpolation in log messages.
    * ❌ `LogInformation($"User {id} logged in")`
    * ✅ `LogInformation("User {UserId} logged in", id)`
