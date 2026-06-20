# WinGet Publishing

CI publishes updates to existing WinGet packages automatically through `PublishWinGetModule`.
The first release of a new Revit-year package is a one-time manual step, described below.

## When this applies

Adding a brand-new Revit-year identifier (e.g., `LookupFoundation.RevitLookup.2025`) to the `microsoft/winget-pkgs` repository. Subsequent version bumps of an existing package are submitted automatically by CI.

## Prerequisites

* The `lookup-foundation-bot` GitHub account (or a maintainer fork of `microsoft/winget-pkgs`).
* A **classic** Personal Access Token with the `public_repo` scope issued from that account. Fine-grained PATs are rejected by `microsoft/winget-pkgs`.
* [Komac](https://github.com/russellbanks/Komac) installed locally:

  ```powershell
  winget install RussellBanks.Komac
  ```

* A signed, public GitHub Release for the new Revit-year containing the `.msi` installer(s).

## One-time submission

Run from PowerShell. Substitute the version, URLs, and `package-name` with the new Revit year:

```powershell
$env:GITHUB_TOKEN = "<lookup-foundation-bot PAT>"

komac new LookupFoundation.RevitLookup.2025 `
  --version 2025.0.0 `
  --urls https://github.com/lookup-foundation/RevitLookup/releases/download/2025.0.0/RevitLookup-2025.0.0-SingleUser.msi `
         https://github.com/lookup-foundation/RevitLookup/releases/download/2025.0.0/RevitLookup-2025.0.0-MultiUser.msi `
  --publisher "Lookup Foundation" `
  --publisher-url https://github.com/lookup-foundation `
  --publisher-support-url https://github.com/lookup-foundation/RevitLookup/issues `
  --author Nice3point `
  --package-name "RevitLookup 2025" `
  --package-url https://github.com/lookup-foundation/RevitLookup `
  --license MIT `
  --license-url https://github.com/lookup-foundation/RevitLookup/blob/HEAD/LICENSE.md `
  --copyright "Copyright (c) Lookup Foundation and Contributors" `
  --short-description "Interactive Revit RFA and RVT project database exploration tool to view and navigate BIM element parameters, properties and relationships." `
  --release-notes-url https://github.com/lookup-foundation/RevitLookup/releases/tag/2025.0.0 `
```

## After the pull request merges

1. Append the new identifier to `.config/winget/configuration.winget` and `.config/winget/configuration-uninstall.winget`:

   ```yaml
   - resource: Microsoft.WinGet.DSC/WinGetPackage
     directives:
       description: RevitLookup for Autodesk Revit 2025
     settings:
       id: LookupFoundation.RevitLookup.2025
       source: winget
       ensure: Present   # use `Absent` in configuration-uninstall.winget
   ```

2. Ensure the Revit year is present in `build/appsettings.json` under `Build.Versions` (it usually already is).

From the next CI release onward, `PublishWinGetModule` updates this package automatically.
No further manual `komac new` is required for that year.