using Build.Options;
using EnumerableAsyncProcessor.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.Git.Options;
using ModularPipelines.GitHub.Attributes;
using ModularPipelines.GitHub.Extensions;
using ModularPipelines.Modules;
using Octokit;
using Shouldly;

namespace Build.Modules;

/// <summary>
///     Publish the add-in to GitHub.
/// </summary>
[SkipIfNoGitHubToken]
[DependsOn<ResolveVersioningModule>]
[DependsOn<GenerateGitHubChangelogModule>]
[DependsOn<CreateBundleModule>(Optional = true)]
[DependsOn<CreateInstallerModule>(Optional = true)]
[DependsOn<SignInstallerModule>(Optional = true)]
public sealed partial class PublishGithubModule(IOptions<BuildOptions> buildOptions) : Module
{
    protected override async Task ExecuteModuleAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var changelogResult = await context.GetModule<GenerateGitHubChangelogModule>();
        var versioning = versioningResult.ValueOrDefault!;
        var changelog = changelogResult.ValueOrDefault!;

        var outputFolder = context.Git().RootDirectory.GetFolder(buildOptions.Value.OutputDirectory);
        var targetFiles = outputFolder.ListFiles().ToArray();
        targetFiles.ShouldNotBeEmpty("No artifacts were found to create the Release");

        var repositoryInfo = context.GitHub().RepositoryInfo;
        var newRelease = new NewRelease(versioning.Version)
        {
            Name = versioning.Version,
            Body = changelog,
            TargetCommitish = context.Git().Information.LastCommitSha,
            Prerelease = versioning.IsPrerelease,
            DiscussionCategoryName = !versioning.IsPrerelease ? "Announcements" : null
        };

        var release = await context.GitHub().Client.Repository.Release.Create(repositoryInfo.Owner, repositoryInfo.RepositoryName, newRelease);
        await targetFiles
            .ForEachAsync(async file =>
            {
                var asset = new ReleaseAssetUpload
                {
                    ContentType = "application/x-binary",
                    FileName = file.Name,
                    RawData = file.GetStream()
                };

                LogUploadingAsset(context.Logger, asset.FileName);

                await context.GitHub().Client.Repository.Release.UploadAsset(release, asset, cancellationToken);
            }, cancellationToken)
            .ProcessInParallel();
        
        context.Summary.KeyValue("Deployment", "GitHub", release.HtmlUrl);
    }

    protected override async Task OnFailedAsync(IModuleContext context, Exception exception, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.ValueOrDefault!;

        await context.Git().Commands.Push(new GitPushOptions
        {
            Delete = true,
            Arguments = ["origin", versioning.Version]
        }, token: cancellationToken);
    }

    [LoggerMessage(LogLevel.Information, "Uploading asset: {Asset}")]
    private static partial void LogUploadingAsset(ILogger logger, string asset);
}