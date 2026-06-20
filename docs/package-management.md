# Package Management

The solution uses centralized NuGet package management.
All versions live in `Directory.Packages.props` (`ManagePackageVersionsCentrally=true`, with floating and transitive pinning enabled).
Renovate (`renovate.json`) bumps versions automatically, so manual version edits are rare.

## Rules

* Define every package version in `Directory.Packages.props`. Do not add a `<Version>` to an individual `PackageReference`.
* Keep Revit-version-specific packages conditional on `$(RevitVersion)`. The Revit API packages float to `$(RevitVersion).*`, and the per-version packages are pinned with a `$(RevitVersion)` condition.
* Keep shared dependency versions unconditional unless they truly vary by Revit version.
* Use `GlobalPackageReference` only for solution-wide packages such as the polyfill and annotation sources.
* Keep the Revit API out of the shared and Playground projects. Only the production add-in references it.

## Add a Dependency

1. Add the package version to `Directory.Packages.props`.
2. Add a versionless `PackageReference` to the project that uses it.
3. Keep the scope narrow. Add a dependency to the project that needs it, not solution-wide, and prefer the platform, the Toolkit, or the Extensions before introducing a new one.
