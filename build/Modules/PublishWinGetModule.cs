using Build.Komac;
using Build.Komac.Options;
using Build.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Configuration;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.GitHub.Attributes;
using ModularPipelines.GitHub.Extensions;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using Shouldly;

namespace Build.Modules;

/// <summary>
///     Publish the add-in updates to the WinGet community repository.
/// </summary>
/// <remarks>
///     Only existing packages are updated.
///     First-time registration of a new <c>LookupFoundation.RevitLookup.{year}</c> package is a one-time manual step.
/// </remarks>
[SkipIfNoGitHubToken]
[DependsOn<ResolveVersioningModule>]
[DependsOn<PublishGithubModule>]
public sealed partial class PublishWinGetModule(IOptions<BuildOptions> buildOptions, IOptions<PublishOptions> publishOptions) : Module
{
    private const string PackageIdPrefix = "LookupFoundation.RevitLookup";

    protected override ModuleConfiguration Configure() => ModuleConfiguration.Create()
        .WithSkipWhen(async context =>
        {
            if (string.IsNullOrEmpty(publishOptions.Value.WinGetToken))
            {
                return SkipDecision.Skip("WinGetToken is not provided");
            }

            var versioningResult = await context.GetModule<ResolveVersioningModule>();
            var versioning = versioningResult.ValueOrDefault!;
            if (versioning.IsPrerelease)
            {
                return SkipDecision.Skip($"Prerelease version: {versioning.Version}");
            }

            return SkipDecision.DoNotSkip;
        })
        .Build();

    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var wingetToken = publishOptions.Value.WinGetToken;

        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.ValueOrDefault!;

        var outputFolder = context.Git().RootDirectory.GetFolder(buildOptions.Value.OutputDirectory);
        var installers = outputFolder.GetFiles(file => file.Extension == ".msi").ToArray();
        installers.ShouldNotBeEmpty("No installers were found to publish to WinGet");

        var repositoryInfo = context.GitHub().RepositoryInfo;
        var releaseTag = versioning.Version;

        foreach (var (_, packageVersion) in buildOptions.Value.Versions)
        {
            var revitYear = packageVersion.Split('.')[0];
            var packageId = $"{PackageIdPrefix}.{revitYear}";

            var matchingInstallers = installers
                .Where(file => file.Name.Contains($"-{packageVersion}-", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            matchingInstallers.ShouldNotBeEmpty($"No installers for version {packageVersion} were found to publish to WinGet");

            var versionsResult = await context.Komac().ListVersions(new KomacListVersionsOptions
            {
                PackageIdentifier = packageId,
                Token = wingetToken
            }, cancellationToken);

            if (versionsResult.ExitCode != 0)
            {
                LogPackageNotRegistered(context.Logger, packageId);
                continue;
            }

            var assetUrls = matchingInstallers
                .Select(file => $"https://github.com/{repositoryInfo.Owner}/{repositoryInfo.RepositoryName}/releases/download/{releaseTag}/{file.Name}")
                .ToArray();

            LogSubmittingUpdate(context.Logger, packageId, packageVersion);

            await context.Komac().Update(new KomacUpdateOptions
            {
                PackageIdentifier = packageId,
                Version = packageVersion,
                Urls = assetUrls,
                CreatedWith = "RevitLookup CI/CD",
                CreatedWithUrl = "https://github.com/lookup-foundation/RevitLookup",
                Submit = true,
                Token = wingetToken
            }, cancellationToken);

            context.Summary.KeyValue("Deployment", "WinGet", packageVersion);
        }
    }

    [LoggerMessage(LogLevel.Warning, "Package {PackageId} is not registered in WinGet, skipping (first release must be published manually)")]
    private static partial void LogPackageNotRegistered(ILogger logger, string packageId);

    [LoggerMessage(LogLevel.Information, "Submitting WinGet update: {PackageId} v{Version}")]
    private static partial void LogSubmittingUpdate(ILogger logger, string packageId, string version);
}