# RevitLookup

RevitLookup is a public open-source Revit add-in that decomposes any element, type, or document at runtime and shows every member in the UI.
It runs in two hosts from one shared codebase: the production add-in inside `Revit.exe`, and a standalone Playground that mocks the Revit layer; UI work needs no running Revit.

## Non-negotiables

* The two hosts stay in sync. Every shared contract the add-in implements against the Revit API has a matching Playground mock; a Revit-backed service, view model, or descriptor ships with its Playground counterpart in the same change.
* The Revit API stays out of the shared layers. `RevitLookup.Abstractions`, `RevitLookup.Common`, `RevitLookup.ServiceDefaults`, and `RevitLookup.UI.Framework` never reference the Revit API; Revit access lives in the `RevitLookup` add-in project alone.
* Touch the Revit API only on the Revit thread. Marshal cross-thread Revit work through the `[ExternalEvent]` source generator; never call the Revit API from a background thread or through `Dispatcher.Invoke`.
* Extend through LookupEngine descriptors, never a special case elsewhere.
* Every type compiles under every supported Revit configuration.
* A change ships with a TUnit test; UI changes are exercised in the Playground.
* Confirm an unfamiliar Revit or .NET API before use through official docs or `gh` (`gh api`, `gh search code`).
* A user-facing change updates `CHANGELOG.md`, the wiki, and the affected XML docs in the same commit.

## Repository map

* `source/RevitLookup` — the production add-in loaded inside `Revit.exe`, and the only project that references the Revit API. Owns the descriptors (`Core/Decomposition/Descriptors`), the Revit-backed service and view-model implementations, and the DI host registration in `Host.cs`.
* `source/RevitLookup.Abstractions` — the shared contracts: view-model interfaces, service interfaces, options, and observable models both hosts implement. No Revit reference.
* `source/RevitLookup.Common` — general-purpose helpers with no Revit dependency.
* `source/RevitLookup.ServiceDefaults` — host configuration shared between the two hosts.
* `source/RevitLookup.UI.Framework` — the shared WPF views, controls, converters, and base classes.
* `source/RevitLookup.UI.Playground` — the standalone WPF host that mocks the shared contracts under `Mocks/`.
* `source/LookupEngine`, `source/LookupEngine.UI` — git submodules supplying the decomposition engine and its base UI.
* `tests/RevitLookup.Tests` — the TUnit suite that runs inside a Revit process using Nice3point.TUnit.Revit.
* `build/` — the ModularPipelines build.
* `.config/winget/` — the WinGet DSC configuration files listing the per-Revit-year package identifiers.
* Root — `Directory.Build.props`, `Directory.Packages.props`, `global.json`, `GitVersion.yml`, the README, CHANGELOG, wiki sources, CI workflows.

## Build and verify

* Build: `dotnet build --project source/RevitLookup/RevitLookup.csproj -c Release.R##`, where the `R##` suffix is the Revit year (`R27` targets Revit 2027).
* Build the Playground: `dotnet run --project source/RevitLookup.UI.Playground -c Debug`.
* Test: `dotnet test -c Release.R##`; required a matching licensed Revit installation.
