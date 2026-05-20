using Build.Komac;
using Build.Komac.Options;
using Build.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.GitHub.Attributes;
using ModularPipelines.GitHub.Extensions;
using ModularPipelines.Modules;
using Shouldly;

namespace Build.Modules;

/// <summary>
///     Publish the add-in updates to the WinGet community repository.
/// </summary>
/// <remarks>
///     Only existing packages are updated. First-time registration of a new
///     <c>LookupFoundation.RevitLookup.{year}</c> package is a one-time manual
///     step (see <c>docs/winget-publishing.md</c>).
/// </remarks>
[SkipIfNoGitHubToken]
[DependsOn<ResolveVersioningModule>]
[DependsOn<PublishGithubModule>]
public sealed class PublishWinGetModule(IOptions<BuildOptions> buildOptions, IOptions<PublishOptions> publishOptions) : Module
{
    private const string PackageIdPrefix = "LookupFoundation.RevitLookup";
    private const string CreatedWithLabel = "RevitLookup CI/CD";
    private const string CreatedWithUrl = "https://github.com/lookup-foundation/RevitLookup";

    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var wingetToken = publishOptions.Value.WinGetToken;
        if (string.IsNullOrEmpty(wingetToken))
        {
            context.Logger.LogInformation("Skipping WinGet publication: WinGetToken is not provided");
            return;
        }

        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.ValueOrDefault!;

        if (versioning.IsPrerelease)
        {
            context.Logger.LogInformation("Skipping WinGet publication for prerelease: {Version}", versioning.Version);
            return;
        }

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
                context.Logger.LogWarning("Package {PackageId} is not registered in WinGet, skipping (first release must be published manually)", packageId);
                continue;
            }

            var assetUrls = matchingInstallers
                .Select(file => $"https://github.com/{repositoryInfo.Owner}/{repositoryInfo.RepositoryName}/releases/download/{releaseTag}/{file.Name}")
                .ToArray();

            context.Logger.LogInformation("Submitting WinGet update: {PackageId} v{Version}", packageId, packageVersion);

            await context.Komac().Update(new KomacUpdateOptions
            {
                PackageIdentifier = packageId,
                Version = packageVersion,
                Urls = assetUrls,
                CreatedWith = CreatedWithLabel,
                CreatedWithUrl = CreatedWithUrl,
                Submit = true,
                Token = wingetToken
            }, cancellationToken);

            context.Summary.KeyValue("Deployment", "WinGet", packageVersion);
        }
    }
}
