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

> [!IMPORTANT]  
> RevitLookup must be installed for each Revit version separately.
> 
> The compatible Revit version is specified in the installer name, e.g. **RevitLookup-2020.0.0-SingleUser.msi** is only compatible with **Revit 2020**.

RevitLookup is also available as an AppBundle, to install the Bundle with all available versions in the release,
use the [AppBundle](https://www.nuget.org/packages/ricaun.AppBundleTool) tool:

```shell
AppBundleTool -a https://github.com/lookup-foundation/RevitLookup/releases/latest/download/RevitLookup.bundle.zip -i
```