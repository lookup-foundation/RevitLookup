# Testing

Tests cover RevitLookup's custom logic, not the Revit API itself.
Every change ships with tests.
There are two test surfaces: automated tests that run inside a real Revit process, and the Playground that exercises the UI without Revit.

## What to Test

* **Custom logic only.** Test the code that adds behavior: decomposition, descriptors, services, and reporting. Skip a thin wrapper that only forwards to a Revit API call.
* **Edge cases:** null inputs, empty collections, and boundary values.
* **No UI assertions in the automated suite.** Exercise the UI through the Playground.

## Automated Tests

* **Framework:** TUnit on the Microsoft.Testing.Platform.
* **Revit access:** Nice3point.TUnit.Revit runs the tests inside the Revit process, so they reach the real Revit API.
* **Configuration:** build for a Revit version with the matching configuration, for example `Debug.R27`. Prefer the latest supported version unless the change is version-specific.
* **Structure:** split each test into blocks marked with `// Arrange`, `// Act`, and `// Assert` comments.

```csharp
public sealed class MyFeatureTests : RevitApiReportTest
{
    [Test]
    public async Task MyFeature_ValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var feature = new MyFeature();

        // Act
        var result = feature.Run();

        // Assert
        await Assert.That(result).IsNotNull();
    }
}
```

## The Playground as a UI Test Environment

The Playground hosts the shared UI as a standalone WPF application with mock ViewModels, so a UI change can be checked without launching Revit.

* Build the Playground with the plain `Debug` configuration.
* The mocks generate realistic sample data with Bogus, so the UI fills with representative content.
* Use the Playground to verify layouts, themes, templates, and interactions, then confirm the production behavior under Revit when the change touches the Revit API.
* See [UI Development](./ui-development.md) for the ViewModel pattern the mocks follow.

## Benchmarks

Performance benchmarks belong to the LookupEngine submodule, which owns the decomposition hot paths.
Do not add benchmarks here.
When a change affects decomposition performance, raise it in the engine repository.

## Build and Test

TUnit runs on the Microsoft.Testing.Platform, so `dotnet test` runs the suite directly.
Pass the target Revit configuration, for example `dotnet test -c Debug.R27`.
