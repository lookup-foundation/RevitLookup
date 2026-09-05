using System.Diagnostics;
using System.Text.Json;
using Build.Options;
using EnumerableAsyncProcessor.Extensions;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.FileSystem;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Modules;
using ModularPipelines.Options;
using Shouldly;
using Sourcy.DotNet;
using File = ModularPipelines.FileSystem.File;
using InstallerOptions = Build.Options.InstallerOptions;

namespace Build.Modules;

/// <summary>
///     Represents the pipeline module that creates the MSI packages.
/// </summary>
/// <param name="buildOptions">The build settings applied to the packaged output.</param>
/// <param name="installerOptions">The installer settings applied to the produced packages.</param>
[DependsOn<CompileProjectModule>]
[DependsOn<SignAssembliesModule>(Optional = true)]
public sealed class CreateInstallerModule(IOptions<BuildOptions> buildOptions, IOptions<InstallerOptions> installerOptions) : Module
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var wixTarget = new File(Projects.RevitLookup.FullName);
        var wixInstaller = new File(Projects.Installer.FullName);
        var wixToolFolder = await InstallWixAsync(context, cancellationToken);

        await context.DotNet().Build(new DotNetBuildOptions
        {
            ProjectSolution = wixInstaller.Path,
            Configuration = "Release"
        }, cancellationToken: cancellationToken);

        var builderFile = wixInstaller.Folder!
            .GetFolder("bin")
            .FindFile(file => file.NameWithoutExtension == wixInstaller.NameWithoutExtension && file.Extension == ".exe");

        builderFile.ShouldNotBeNull($"No installer builder was found for the project: {wixInstaller.NameWithoutExtension}");

        var targetDirectories = wixTarget.Folder!
            .GetFolder("bin")
            .GetFolders(folder => folder.Name == "publish")
            .ToArray();

        targetDirectories.ShouldNotBeEmpty("No content was found to create an installer");

        var outputFolder = context.Git().RootDirectory.GetFolder(buildOptions.Value.OutputDirectory);
        if (!outputFolder.Exists)
        {
            await outputFolder.CreateAsync(cancellationToken);
        }

        await targetDirectories.ForEachAsync(async targetDirectory =>
            {
                var manifestFile = await WriteManifestAsync(wixTarget.NameWithoutExtension, targetDirectory, outputFolder, cancellationToken);

                await context.Shell.Command.ExecuteCommandLineTool(
                    new GenericCommandLineToolOptions(builderFile.Path)
                    {
                        Arguments = [manifestFile.Path]
                    },
                    new CommandExecutionOptions
                    {
                        WorkingDirectory = context.Git().RootDirectory,
                        EnvironmentVariables = new Dictionary<string, string?>
                        {
                            { "PATH", $"{Environment.GetEnvironmentVariable("PATH")};{wixToolFolder}" }
                        }
                    }, cancellationToken);
            }, cancellationToken)
            .ProcessInParallel();

        var outputFiles = outputFolder.GetFiles(file => file.Extension == ".msi").ToArray();
        outputFiles.ShouldNotBeEmpty("Failed to create an installer");

        foreach (var outputFile in outputFiles)
        {
            context.Summary.KeyValue("Artifacts", "Installer", outputFile.Path);
        }
    }

    /// <summary>
    ///     Writes the installer manifest for a single compiled Revit configuration.
    /// </summary>
    private async Task<File> WriteManifestAsync(string productName, Folder targetDirectory, Folder outputFolder, CancellationToken cancellationToken)
    {
        var contentRoot = targetDirectory.Parent!;
        var configuration = contentRoot.Name;

        buildOptions.Value.Versions
            .TryGetValue(configuration, out var version)
            .ShouldBeTrue($"Can't map version for configuration: {configuration}");

        installerOptions.Value.UpgradeCodes
            .TryGetValue(configuration, out var upgradeCode)
            .ShouldBeTrue($"Can't map upgrade code for configuration: {configuration}");

        var versionPrefix = ResolveVersionPrefix(version);
        var manifest = new
        {
            ProductName = productName,
            ProductVersion = ResolveMsiVersion(versionPrefix),
            UpgradeCode = upgradeCode,
            ReleaseVersion = version,
            OutputDirectory = outputFolder.Path,
            Content = new[]
            {
                new
                {
                    RevitVersion = versionPrefix.Major,
                    Files = new[]
                    {
                        new
                        {
                            Role = "payload",
                            BasePath = targetDirectory.Name,
                            Include = new[] { "**" },
                            Exclude = new[] { "**/*.addin", "**/*.pdb" }
                        },
                        new
                        {
                            Role = "addin",
                            BasePath = targetDirectory.Name,
                            Include = new[] { "**/*.addin" },
                            Exclude = Array.Empty<string>()
                        }
                    }
                }
            }
        };

        var manifestFile = contentRoot.GetFile("installer.manifest.json");
        var manifestContent = JsonSerializer.Serialize(manifest, SerializerOptions);
        await manifestFile.WriteAsync(manifestContent, cancellationToken);

        return manifestFile;
    }

    /// <summary>
    ///     Resolves the normal part of the specified release version.
    /// </summary>
    /// <param name="releaseVersion">The version the release is published under.</param>
    /// <returns>The version number without its pre-release label.</returns>
    private static Version ResolveVersionPrefix(string releaseVersion)
    {
        var labelIndex = releaseVersion.IndexOf('-');
        var versionPrefix = labelIndex < 0 ? releaseVersion.AsSpan() : releaseVersion.AsSpan(0, labelIndex);

        return Version.Parse(versionPrefix);
    }

    /// <summary>
    ///     Resolves the version written into the MSI database.
    /// </summary>
    /// <param name="versionPrefix">The normal part of the release version.</param>
    /// <returns>The version Windows Installer compares against the installed package.</returns>
    /// <remarks>
    ///     The major component of an Autodesk Revit installation is the last two digits of the release year.
    ///     The add-in installer follows the same pattern.
    /// </remarks>
    private static Version ResolveMsiVersion(Version versionPrefix)
    {
        return new Version(versionPrefix.Major % 100, versionPrefix.Minor, versionPrefix.Build);
    }

    /// <summary>
    ///     Installs the WiX toolset required for building installers.
    /// </summary>
    private static async Task<Folder> InstallWixAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var wixToolFolder = Folder.CreateTemporaryFolder();
        await context.DotNet().Tool.Execute(new DotNetToolOptions
        {
            Arguments = ["install", "wix", "--version", "7.*", "--tool-path", wixToolFolder.Path]
        }, cancellationToken: cancellationToken);

        var wixExe = wixToolFolder.GetFile("wix.exe");
        var wixVersion = FileVersionInfo.GetVersionInfo(wixExe.Path).FileVersion!;

        await context.Shell.Command.ExecuteCommandLineTool(
            new GenericCommandLineToolOptions(wixExe.Path)
            {
                Arguments = ["eula", "accept", "wix7"]
            }, cancellationToken: cancellationToken);

        await context.Shell.Command.ExecuteCommandLineTool(
            new GenericCommandLineToolOptions(wixExe.Path)
            {
                Arguments = ["extension", "add", "-g", $"WixToolset.UI.wixext/{wixVersion}"]
            }, cancellationToken: cancellationToken);

        return wixToolFolder;
    }
}
