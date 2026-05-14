# Package Management

RevitLookup uses centralized package management.

* **Centralized:** All NuGet versions are defined in `Directory.Packages.props`.
* **Clean csproj:** Do NOT include `<Version>` tags in individual project files.
* **Update Policy:** Use the common props file for updates.
