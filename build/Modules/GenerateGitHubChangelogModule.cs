using System.Text;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.GitHub.Extensions;
using ModularPipelines.Modules;

namespace Build.Modules;

/// <summary>
///     Generate and format the changelog for publishing on the GitHub.
/// </summary>
[DependsOn<GenerateChangelogModule>]
[DependsOn<ResolveVersioningModule>]
public sealed class GenerateGitHubChangelogModule : Module<string>
{
    protected override async Task<string?> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var changelogResult = await context.GetModule<GenerateChangelogModule>();
        var versioning = versioningResult.ValueOrDefault!;
        var changelog = changelogResult.ValueOrDefault!;

        return AppendExtraUrls(context, changelog, versioning);
    }

    /// <summary>
    ///     Append extra links for GitHub release.
    /// </summary>
    private static string AppendExtraUrls(IModuleContext context, string changelog, ResolveVersioningResult versioning)
    {
        var repositoryInfo = context.GitHub().RepositoryInfo;
        StringBuilder? changelogBuilder = null;

        if (!versioning.IsPrerelease)
        {
            changelogBuilder ??= new StringBuilder(changelog);
            changelogBuilder.AppendLine()
                .AppendLine()
                .AppendLine("## Installation")
                .AppendLine()
                .AppendLine("```ps1")
                .AppendLine("winget configure -f https://raw.githubusercontent.com/lookup-foundation/RevitLookup/main/.config/winget/configuration.winget --accept-configuration-agreements")
                .Append("```")
                .AppendLine()
                .AppendLine()
                .Append("Other installation methods: ")
                .Append("https://github.com/lookup-foundation/RevitLookup/wiki/Versions");
        }

        if (!changelog.Contains("Full changelog", StringComparison.OrdinalIgnoreCase))
        {
            changelogBuilder ??= new StringBuilder(changelog);
            changelogBuilder.AppendLine()
                .AppendLine()
                .Append("**Full changelog**: ")
                .AppendLine($"https://github.com/{repositoryInfo.Identifier}/compare/{versioning.PreviousVersion}...{versioning.Version}");
        }

        return changelogBuilder?.ToString() ?? changelog;
    }
}