namespace Build.Options;

/// <summary>
///     Represents the publish settings used to release and distribute the add-in.
/// </summary>
[PublicAPI]
public sealed record PublishOptions
{
    /// <summary>
    ///     Gets the product release version.
    /// </summary>
    public string? Version { get; init; }

    /// <summary>
    ///     Gets the path to the release changelog file.
    /// </summary>
    public string ChangelogFile { get; init; } = "CHANGELOG.md";

    /// <summary>
    ///     Gets the classic GitHub personal access token with <c>public_repo</c> scope used to create pull requests in <c>microsoft/winget-pkgs</c>.
    /// </summary>
    public string? WinGetToken { get; init; }
}
