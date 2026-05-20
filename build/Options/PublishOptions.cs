namespace Build.Options;

[Serializable]
public sealed record PublishOptions
{
    /// <summary>
    ///     Product release version
    /// </summary>
    public string? Version { get; init; }

    /// <summary>
    ///     Path to the release changelog file
    /// </summary>
    public string ChangelogFile { get; init; } = "CHANGELOG.md";

    /// <summary>
    ///     Classic GitHub PAT with public_repo scope, to create PRs in microsoft/winget-pkgs.
    /// </summary>
    public string? WinGetToken { get; init; }
}