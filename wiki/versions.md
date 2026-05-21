### RevitLookup releases

Current supported Revit versions: 2021-2027.

| Revit version | Link                                                                       |
|---------------|----------------------------------------------------------------------------|
| 2027          | https://github.com/lookup-foundation/RevitLookup/releases/latest           |
| 2026          | https://github.com/lookup-foundation/RevitLookup/releases/latest           |
| 2025          | https://github.com/lookup-foundation/RevitLookup/releases/latest           |
| 2024          | https://github.com/lookup-foundation/RevitLookup/releases/latest           |
| 2023          | https://github.com/lookup-foundation/RevitLookup/releases/latest           |
| 2022          | https://github.com/lookup-foundation/RevitLookup/releases/latest           |
| 2021          | https://github.com/lookup-foundation/RevitLookup/releases/latest           |
| 2020          | https://github.com/lookup-foundation/RevitLookup/releases/tag/2023.1.0-EOL |
| 2019          | https://github.com/lookup-foundation/RevitLookup/releases/tag/2023.1.0-EOL |
| 2018          | https://github.com/lookup-foundation/RevitLookup/releases/tag/2023.1.0-EOL |
| 2017          | https://github.com/lookup-foundation/RevitLookup/releases/tag/2023.1.0-EOL |
| 2016          | https://github.com/lookup-foundation/RevitLookup/releases/tag/2023.1.0-EOL |
| 2015          | https://github.com/lookup-foundation/RevitLookup/releases/tag/2023.1.0-EOL |

> [!NOTE]  
> - Single-user installation is for one user only and does not require administrator rights.
> - Multi-user installation requires administrator rights and is installed for all users.

### Installation

#### WinGet (recommended)

List all available RevitLookup packages:

```ps1
winget search LookupFoundation.RevitLookup
```

Install for a single Revit version, current user only (replace `2025` with your Revit version):

```ps1
winget install LookupFoundation.RevitLookup.2025
```

Install for a single Revit version, all users (replace `2025` with your Revit version):

```ps1
winget install LookupFoundation.RevitLookup.2025 --scope machine
```

Install for every supported Revit version at once using a [WinGet Configuration](https://learn.microsoft.com/en-us/windows/package-manager/configuration/):

```ps1
winget configure -f https://raw.githubusercontent.com/lookup-foundation/RevitLookup/main/.config/winget/configuration.winget --accept-configuration-agreements
```

Uninstall every RevitLookup version:

```ps1
winget configure -f https://raw.githubusercontent.com/lookup-foundation/RevitLookup/main/.config/winget/configuration-uninstall.winget --accept-configuration-agreements
```

#### AppBundle

Install the bundle with every available Revit version using the [AppBundle](https://www.nuget.org/packages/ricaun.AppBundleTool) tool:

```ps1
AppBundleTool -a https://github.com/lookup-foundation/RevitLookup/releases/latest/download/RevitLookup.bundle.zip -i
```

#### Manual

Download the appropriate `.msi` for your Revit version from the [latest GitHub release](https://github.com/lookup-foundation/RevitLookup/releases/latest) and run it.

> [!IMPORTANT]  
> RevitLookup must be installed for each Revit version separately.
>
> The compatible Revit version is specified in the installer name, e.g. **RevitLookup-2025.0.0-SingleUser.msi** is only compatible with **Revit 2025**.