# Project Structure

The solution follows a logical hierarchy to separate concerns, decouple the UI from Revit, and enable rapid prototyping.

## Solution Groups (Virtual Folders)

* **`/Automation`**: Build scripts and Installers.
    * `build`: ModularPipelines projects for building and publishing the solution.
    * `install`: WiX Toolset projects for creating MSIs.
* **`/Engine`**: Core LookupEngine framework.
    * `LookupEngine.Abstractions`: **[Submodule - DO NOT MODIFY]** Interfaces and contract definitions.
    * `LookupEngine`: **[Submodule - DO NOT MODIFY]** Core decomposition and object analysis engine.
* **`/Frontend`**: UI framework and RevitLookup UI components.
    * `LookupEngine.UI.Abstractions`: **[Submodule - DO NOT MODIFY]** UI interfaces and contracts.
    * `LookupEngine.UI`: **[Submodule - DO NOT MODIFY]** Core UI components.
    * `RevitLookup.UI.Framework`: Shared UI components and base classes specific to RevitLookup.
* **`/Playground`**: The prototyping environment.
    * `RevitLookup.UI.Playground`: The mockup entry point for testing UI without Revit.
* **`/Revit`**: The Production environment (Revit Plugin).
    * `RevitLookup`: The main Plugin Host (Entry Point).
* **`/Tests`**: Testing projects.
    * `RevitLookup.Tests`: TUnit tests for RevitLookup.
* **Root Level**: Shared projects. Pure C#, no Revit references.
    * `RevitLookup.Abstractions`: Interfaces and contract definitions.
    * `RevitLookup.Common`: General purpose utilities.
    * `RevitLookup.ServiceDefaults`: Service configurations shared between Playground and Revit.
