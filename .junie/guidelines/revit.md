# Revit Development Best Practices

Guidelines for interacting with the Revit API and developing the plugin environment.

## 1. Revit API Context

* **Toolkit:** Use `Nice3point.Revit.Toolkit` and `Nice3point.Revit.Extensions` for all Revit interactions.
* **Thread Safety:** The Revit API is single-threaded.
    * **NEVER** use `Dispatcher.Invoke`.
    * **ALWAYS** use `ExternalEvent` attribute with source-generator (via Toolkit) to marshal execution from async/background threads to the Revit Context.

## 2. Environments Compatibility

Code must be written to be compatible with both Revit and Playground where possible.
* Use `RevitLookup.Abstractions` for interfaces.
* Isolate Revit-specific logic behind abstractions or in the `RevitLookup` project.
