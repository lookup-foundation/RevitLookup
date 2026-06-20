# Contributing to RevitLookup

Thanks for taking the time to contribute. This guide covers issues and pull requests. For the architecture, conventions, and release process, see the project guidelines in [AGENTS.md](AGENTS.md) and the [docs](docs/) folder.

## Maintainers

| Maintainer | Period |
|---|---|
| <img src="https://github.com/Nice3point.png" width="20"/> [Nice3point](https://github.com/Nice3point) | 2022 - present |
| <img src="https://github.com/jeremytammik.png" width="20"/> [Jeremy Tammik](https://github.com/jeremytammik) | 2008 - 2022 |
| <img src="https://github.com/identicons/jimawe.png" width="20"/> [Jim Awe](https://www.linkedin.com/in/james-awe-4630a94/) | 2005 - 2008 |

Planning a significant change? Check in with the current maintainer before you open a pull request.

## Table of contents

<!-- TOC -->
  * [Fork, Clone, Branch and Create your PR](#fork-clone-branch-and-create-your-pr)
  * [Prerequisites](#prerequisites)
  * [Building](#building)
    * [Building the MSI installer and the Autodesk bundle on local machine](#building-the-msi-installer-and-the-autodesk-bundle-on-local-machine)
  * [Publishing Releases](#publishing-releases)
    * [Updating the Changelog](#updating-the-changelog)
    * [Creating a new Release from the JetBrains Rider](#creating-a-new-release-from-the-jetbrains-rider)
    * [Creating a new Release from the Terminal](#creating-a-new-release-from-the-terminal)
    * [Creating a new Release on GitHub](#creating-a-new-release-on-github)
  * [Compiling a solution on GitHub](#compiling-a-solution-on-github)
  * [Conditional compilation for a specific Revit version](#conditional-compilation-for-a-specific-revit-version)
  * [Managing Supported Revit Versions](#managing-supported-revit-versions)
    * [Solution configurations](#solution-configurations)
    * [Project configurations](#project-configurations)
  * [API references](#api-references)
  * [Development Guidelines](#development-guidelines)
<!-- TOC -->

## Fork, Clone, Branch and Create your PR

1. Fork the repo if you haven't already
2. Clone your fork locally
3. Create & push a feature branch
4. Create a [Draft Pull Request (PR)](https://github.blog/2019-02-14-introducing-draft-pull-requests/)
5. Work on your changes

## Prerequisites

Before you can build this project, you need to install .NET and IDE.
If you haven't already installed these, you can do so by visiting the following:

- [.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48)
- [.NET 10](https://dotnet.microsoft.com/en-us/download/dotnet)
- [JetBrains Rider](https://www.jetbrains.com/rider/) or [Visual Studio](https://visualstudio.microsoft.com/)

## Building

We recommend JetBrains Rider as preferred IDE, since it has outstanding .NET support. If you don't have Rider installed, you can download it
from [here](https://www.jetbrains.com/rider/).

1. Open JetBrains Rider
2. In the `Solutions Configuration` drop-down menu, select `Release.R27` or `Debug.R27`. Suffix `R27` means compiling for the Revit 2027.
3. After the solution loads, you can build it by clicking on `Build -> Build Solution`.
4. `Debug` button will start Revit add-in in the debug mode.

   ![image](https://github.com/user-attachments/assets/d209d863-a6d5-43a9-83e1-5eeb2b9fddac)

Also, you can use Visual Studio. If you don't have Visual Studio installed, download it from [here](https://visualstudio.microsoft.com/downloads/).

1. Open Visual Studio
2. In the `Solutions Configuration` drop-down menu, select `Release.R27` or `Debug.R27`. Suffix `R27` means compiling for the Revit 2027.
3. After the solution loads, you can build it by clicking on `Build -> Build Solution`.

### Building the MSI installer and the Autodesk bundle on local machine

To build the project for all versions, create the installer and bundle, this project uses [ModularPipelines](https://github.com/thomhurst/ModularPipelines)

To execute your ModularPipelines build locally, you can follow these steps:

1. **Navigate to your project directory**. Open a terminal / command prompt and navigate to your project's root directory.
2. **Run the build**. Once you have navigated to your project's root directory, you can run the ModularPipelines build by calling:

   Compile:
   ```shell
   cd build; dotnet run
   ```

   Create installer and bundle:
   ```shell
   cd build; dotnet run -- pack
   ```

   This command will execute the ModularPipelines build defined in your project.

## Publishing Releases

Releases are managed by creating new [Git tags](https://git-scm.com/book/en/v2/Git-Basics-Tagging).
A tag in Git used to capture a snapshot of the project at a particular point in time, with the ability to roll back to a previous version.

The build system uses [GitVersion.Tool](https://gitversion.net/docs/) to automatically determine the Release version based on the Git history and tags. 
If a tag is present on the current commit, the version will match the tag. If no tag is specified, the tool automatically generates a release version based on the branch name and commit history.

You can also specify a fixed version by setting the `Version` property in the `build/appsettings.json` file. This will override the version determined by GitVersion.Tool.

Tags can follow the format `version` or `version-stage.n.date` for pre-releases, where:

- **version** specifies the version of the release:
    - `1.0.0`
    - `2.3.0`
- **stage** specifies the release stage:
    - `alpha` - represents early iterations that may be unstable or incomplete.
    - `beta` - represents a feature-complete version but may still contain some bugs.
- **n** prerelease increment (optional):
    - `1` - first alpha prerelease
    - `2` - second alpha prerelease
- **date** specifies the date of the pre-release (optional):
    - `250101`
    - `20250101`

For example:

| Stage   | Version                |
|---------|------------------------|
| Alpha   | 1.0.0-alpha            |
| Alpha   | 1.0.0-alpha.1.20250101 |
| Beta    | 1.0.0-beta.2.20250101  |
| Release | 1.0.0                  |

### Updating the Changelog

Updating the changelog is optional. If you provide a changelog, the build system will use it for the release notes. 
If no entry is found for the current version, GitHub will automatically generate release notes based on your pull requests and commits.

To update the changelog manually:

1. Navigate to the solution root.
2. Open the file **CHANGELOG.md**.
3. Add a section for your version. The version separator is the `#` symbol.
4. Specify the release number e.g. `# 1.0.0` or `# 25.01.01 v1.0.0`, the format does not matter, the main thing is that it contains the version.
5. In the lines below, write a changelog for your version, style to your taste.
6. Commit your changes.

### Creating a new Release from the JetBrains Rider

JetBrains provides a handy UI for creating a tag, it can be created in a few steps:

1. Open JetBrains Rider.
2. Navigate to the **Git** tab.
3. Click **New Tag...** and create a new tag.

   ![image](https://github.com/user-attachments/assets/19c11322-9f95-45e5-8fe6-defa36af59c4)

4. Navigate to the **Git** panel.
5. Expand the **Tags** section.
6. Right-click on the newly created tag and select **Push to origin**.

   ![image](https://github.com/user-attachments/assets/b2349264-dd76-4c21-b596-93110f1f16cb)

   This process will trigger the Release workflow and create a new Release on GitHub.

### Creating a new Release from the Terminal

Alternatively, you can create and push tags using the terminal:

1. Navigate to the repository root and open the terminal.
2. Use the following command to create a new tag:
   ```shell
   git tag 'version'
   ```

   Replace `version` with the desired version, e.g., `1.0.0`.
3. Push the newly created tag to the remote repository using the following command:
   ```shell
   git push origin 'version'
   ```

> [!NOTE]  
> The tag will reference your current commit, so verify you're on the correct branch and have fetched latest changes from remote first.

### Creating a new Release on GitHub

To create releases directly on GitHub:

1. Navigate to the **Actions** section on the repository page.
2. Select **Publish Release** workflow.
3. Click **Run workflow** button.
4. (Optional) Specify the release version. If not specified, the system will automatically determine the version based on your Git history.
5. Click **Run**.

    ![image](https://github.com/user-attachments/assets/088388c1-6055-4d21-8d22-70f047d8f104)

## Compiling a solution on GitHub

Pushing commits to the remote repository will start a pipeline compiling the solution for all specified Revit versions. 
That way, you can check if the plugin is compatible with different API versions without having to spend time building it locally.

## Conditional compilation for a specific Revit version

To write code compatible with different Revit versions, use the directives **#if**, **#elif**, **#else**, **#endif**.

```c#
#if REVIT2027
    //Your code here
#endif
```

To target a specific Revit version, set the solution configuration in your IDE interface to match that version.
E.g., select the `Debug.R27` configuration for the Revit 2027 API.

The project has available constants such as `REVIT2027`, `REVIT2027_OR_GREATER`. 
Create conditions, experiment to achieve the desired result.

> [!NOTE]  
> For generating directives, a Revit MSBuild SDK is used.
> You can find more detailed documentation about it here: [Revit MSBuild SDK](https://github.com/Nice3point/Revit.Build.Tasks)

To support the latest APIs in legacy Revit versions:

```c#
#if REVIT2021_OR_GREATER
    UnitUtils.ConvertFromInternalUnits(69, UnitTypeId.Millimeters);
#else
    UnitUtils.ConvertFromInternalUnits(69, DisplayUnitType.DUT_MILLIMETERS);
#endif
```

`#if REVIT2021_OR_GREATER` сompiles a block of code for Revit versions 21, 22, 23 and greater.

To support removed APIs in newer versions of Revit, you can invert the constant:

```c#
#if !REVIT2023_OR_GREATER
    var builtinCategory = (BuiltInCategory) category.Id.IntegerValue;
#endif
```

`#if !REVIT2023_OR_GREATER` сompiles a block of code for Revit versions 22, 21, 20 and lower.

## Managing Supported Revit Versions

To extend or reduce the range of supported Revit API versions, you need to update the solution and project configurations.

### Solution configurations

Solution configurations determine which projects are built and how they are configured.

To support multiple Revit versions:
- Open the `.sln` file.
- Add or remove configurations for each Revit version.

Example:

```text
GlobalSection(SolutionConfigurationPlatforms) = preSolution
    Debug.R25|Any CPU = Debug.R25|Any CPU
    Debug.R26|Any CPU = Debug.R26|Any CPU
    Debug.R27|Any CPU = Debug.R27|Any CPU
    Release.R25|Any CPU = Release.R25|Any CPU
    Release.R26|Any CPU = Release.R26|Any CPU
    Release.R27|Any CPU = Release.R27|Any CPU
EndGlobalSection
```

For example `Debug.R27` is the Debug configuration for Revit 2027 version.

> [!TIP]  
> If you are just ending maintenance for some version, removing the Solution configurations without modifying the Project configurations is enough.

### Project configurations

Project configurations define build conditions for specific versions.

To add or remove support:
- Open `.csproj` file
- Add or remove configurations for Debug and Release builds.

Example:

```xml
<PropertyGroup>
    <Configurations>Debug.R25;Debug.R26;Debug.R27</Configurations>
    <Configurations>$(Configurations);Release.R25;Release.R26;Release.R27</Configurations>
</PropertyGroup>
```

> [!IMPORTANT]  
> Edit the `.csproj` file only manually, IDEs often break configurations.

Revit MSBuild SDK automatically sets the required `TargetFramework` based on the `RevitVersion`, extracted from the solution configuration name. 

If you need to add support for an unreleased or unsupported version of Revit that the SDK doesn't yet know about, you can add a conditional block to specify the `TargetFramework` manually:

```xml
<PropertyGroup>
    <TargetFramework Condition="$(RevitVersion) == '2027'">net10.0-windows7.0</TargetFramework>
</PropertyGroup>
```

## API references

To support CI/CD pipelines and build a project for Revit versions not installed on your computer, use Nuget packages.

> [!NOTE]  
> Revit API dependencies are available in the [Revit.API](https://github.com/Nice3point/RevitApi) repository.

The Nuget package version must include wildcards `Version="$(RevitVersion).*"` to automatically include adding a specific package version, depending on the selected solution configuration.

```xml
<ItemGroup>
    <PackageReference Include="Nice3point.Revit.Api.RevitAPI" Version="$(RevitVersion).*"/>
    <PackageReference Include="Nice3point.Revit.Api.RevitAPIUI" Version="$(RevitVersion).*"/>
</ItemGroup>
```

## Development Guidelines

Information about the project architecture, core principles, please refer to the [guidelines](AGENTS.md) file.
