---
name: revitlookup-winget-publishing
description: >
    Register a new Revit-year RevitLookup package on the WinGet community repository, the one-time manual step the release pipeline cannot do on its own.
    USE FOR: submitting a brand-new LookupFoundation.RevitLookup.{year} identifier to microsoft/winget-pkgs with komac, then wiring the year into the repo's WinGet DSC config and the build version list so CI updates it thereafter.
    DO NOT USE FOR: routine version bumps of an already-registered package (the pipeline does those automatically), or building the MSI installers and GitHub release.
license: MIT
---

# RevitLookup WinGet Publishing

The release pipeline updates already-registered WinGet packages on its own: `PublishWinGetModule` submits a new version for every Revit year through Komac, skipping prereleases and any package not yet registered.
The only manual step is the **first** release of a new Revit-year identifier, which `microsoft/winget-pkgs` requires a human to create.
This skill covers that one-time registration and the follow-up that hands the package back to CI.

## When to use

- Adding a brand-new `LookupFoundation.RevitLookup.{year}` package to `microsoft/winget-pkgs` (for example the first `.2028` release).

## When not to use

- A version bump of an existing Revit-year package — `PublishWinGetModule` submits that automatically on release.

## Workflow

### Step 1: Confirm the prerequisites

- The `lookup-foundation-bot` GitHub account, or a maintainer fork of `microsoft/winget-pkgs`.
- A **classic** Personal Access Token with the `public_repo` scope from that account; `microsoft/winget-pkgs` rejects fine-grained tokens.
- [Komac](https://github.com/russellbanks/Komac) installed locally (`winget install RussellBanks.Komac`).
- A signed, public GitHub Release for the new Revit year that already contains the SingleUser and MultiUser `.msi` installers (produced by the `pack`/`publish` pipeline).

### Step 2: Submit the new package with komac new

Run from PowerShell, substituting the year, version, and installer URLs.

```powershell
$env:GITHUB_TOKEN = "<lookup-foundation-bot PAT>"

komac new LookupFoundation.RevitLookup.2028 `
  --version 2028.0.0 `
  --urls https://github.com/lookup-foundation/RevitLookup/releases/download/2028.0.0/RevitLookup-2028.0.0-SingleUser.msi `
         https://github.com/lookup-foundation/RevitLookup/releases/download/2028.0.0/RevitLookup-2028.0.0-MultiUser.msi `
  --publisher "Lookup Foundation" `
  --package-name "RevitLookup 2028" `
  --license MIT
```

Keep the identifier exactly `LookupFoundation.RevitLookup.{year}`; the pipeline derives it as `LookupFoundation.RevitLookup.` plus the Revit year and matches installers by the `-{version}-` fragment in the file name.

### Step 3: Register the year in the repo after the pull request merges

Append the identifier to both WinGet DSC files under `.config/winget/`, using `ensure: Present` in `configuration.winget` and `ensure: Absent` in `configuration-uninstall.winget`.

```yaml
-   resource: Microsoft.WinGet.DSC/WinGetPackage
    directives:
        description: RevitLookup for Autodesk Revit 2028
    settings:
        id: LookupFoundation.RevitLookup.2028
        source: winget
        ensure: Present
```

Then confirm the Revit year is present in `build/appsettings.json` under `Build.Versions`; `PublishWinGetModule` iterates over it.

### Step 4: Verify the handoff to CI

On the next release, `PublishWinGetModule` finds the now-registered package with `komac list-versions`, builds the asset URLs from the GitHub release, and submits the update; no further `komac new` is needed for that year.
A package that is still unregistered logs a warning and is skipped, which is the signal the manual step has not completed.

## Validation

- [ ] The new identifier is exactly `LookupFoundation.RevitLookup.{year}`.
- [ ] The submission used a classic PAT with `public_repo` scope and pointed at a signed public release containing both `.msi` installers.
- [ ] The identifier was appended to `configuration.winget` (`ensure: Present`) and `configuration-uninstall.winget` (`ensure: Absent`).
- [ ] The Revit year appears in `build/appsettings.json` under `Build.Versions`.
- [ ] The next CI release updates the package automatically, with no not-registered warning.

## Common Pitfalls

| Pitfall                                         | Correct approach                                                                    |
|-------------------------------------------------|-------------------------------------------------------------------------------------|
| Running `komac new` for an existing package     | Only the first release of a Revit year is manual; CI updates the rest.              |
| Using a fine-grained PAT                        | `microsoft/winget-pkgs` requires a classic PAT with `public_repo`.                  |
| Forgetting the DSC config after the PR merges   | Append the id to both `.config/winget` files, Present and Absent.                   |
| Omitting the year from `build/appsettings.json` | Add it to `Build.Versions` for the pipeline to iterate the package.                 |
| A deviating identifier                          | Keep `LookupFoundation.RevitLookup.{year}`; the pipeline derives and matches on it. |
