using System.Text;
using Build.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Context;
using ModularPipelines.Git.Extensions;
using ModularPipelines.GitHub.Extensions;
using ModularPipelines.Modules;
using Octokit;
using Shouldly;
using File = ModularPipelines.FileSystem.File;

namespace Build.Modules;

/// <summary>
///     Generate the changelog for publishing the add-in.
/// </summary>
[DependsOn<ResolveVersioningModule>]
public sealed partial class GenerateChangelogModule(IOptions<PublishOptions> publishOptions) : Module<string>
{
    protected override async Task<string?> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var versioningResult = await context.GetModule<ResolveVersioningModule>();
        var versioning = versioningResult.ValueOrDefault!;

        if (string.IsNullOrEmpty(publishOptions.Value.ChangelogFile))
        {
            LogChangelogNotSpecified(context.Logger);
            return await GenerateReleaseNotesAsync(context, versioning);
        }

        var changelogFile = context.Git().RootDirectory.GetFile(publishOptions.Value.ChangelogFile);
        if (!changelogFile.Exists)
        {
            LogChangelogNotFound(context.Logger);
            return await GenerateReleaseNotesAsync(context, versioning);
        }

        var changelog = await ParseChangelog(changelogFile, versioning.Version);
        if (changelog.Length == 0)
        {
            versioning.IsPrerelease.ShouldBeTrue($"No version entry exists in the changelog: {versioning.Version}");
            
            LogNoVersionEntry(context.Logger, versioning.Version);
            return await GenerateReleaseNotesAsync(context, versioning);
        }

        return changelog.ToString();
    }

    /// <summary>
    ///     Parse the changelog file to extract the entries for a specific version.
    /// </summary>
    private static async Task<StringBuilder> ParseChangelog(File changelogFile, string version)
    {
        const string separator = "# ";

        var isChangelogEntryFound = false;
        var changelog = new StringBuilder();

        await foreach (var line in changelogFile.ReadLinesAsync())
        {
            if (isChangelogEntryFound)
            {
                if (line.StartsWith(separator)) break;

                changelog.AppendLine(line);
                continue;
            }

            if (line.StartsWith(separator) && line.Contains(version))
            {
                isChangelogEntryFound = true;
            }
        }

        TrimEmptyLines(changelog);
        return changelog;
    }

    /// <summary>
    ///     Remove empty lines from the beginning and end of the changelog builder.
    /// </summary>
    private static void TrimEmptyLines(StringBuilder changelog)
    {
        if (changelog.Length == 0) return;

        var start = 0;
        var end = changelog.Length - 1;

        while (start < changelog.Length && (changelog[start] == '\r' || changelog[start] == '\n')) start++;
        while (end >= start && (changelog[end] == '\r' || changelog[end] == '\n')) end--;

        if (end < changelog.Length - 1)
        {
            changelog.Remove(end + 1, changelog.Length - (end + 1));
        }

        if (start > 0)
        {
            changelog.Remove(0, start);
        }
    }

    /// <summary>
    ///     Call the GitHub API to generate release notes for a specific version.
    /// </summary>
    private static async Task<string?> GenerateReleaseNotesAsync(IModuleContext context, ResolveVersioningResult versioning)
    {
        var repositoryId = long.Parse(context.GitHub().EnvironmentVariables.RepositoryId!);

        var previousVersion = versioning.PreviousVersion;
        var isHashedVersion = previousVersion.Length >= 40 && previousVersion.All(c => char.IsDigit(c) || c is >= 'a' and <= 'f');

        var releaseNotes = await context.GitHub().Client.Repository.Release.GenerateReleaseNotes(repositoryId,
            new GenerateReleaseNotesRequest(versioning.Version)
            {
                PreviousTagName = isHashedVersion ? null : previousVersion
            });

        return releaseNotes.Body;
    }

    [LoggerMessage(LogLevel.Information, "Changelog file not specified")]
    private static partial void LogChangelogNotSpecified(ILogger logger);

    [LoggerMessage(LogLevel.Warning, "Changelog specified but not found")]
    private static partial void LogChangelogNotFound(ILogger logger);

    [LoggerMessage(LogLevel.Warning, "No version entry exists in the changelog: {Version}")]
    private static partial void LogNoVersionEntry(ILogger logger, string version);
}