using ModularPipelines.Attributes;
using ModularPipelines.Options;

namespace Build.Komac.Options;

/// <summary>
///     Represents the command-line options for the Komac <c>update</c> command.
/// </summary>
/// <remarks>
///     The command submits a new version of an existing WinGet package manifest.
/// </remarks>
[PublicAPI]
[CliSubCommand("update")]
public sealed record KomacUpdateOptions : CommandLineToolOptions
{
    /// <summary>
    ///     Gets or sets the WinGet package identifier to update.
    /// </summary>
    [CliArgument(0, Placement = ArgumentPlacement.ImmediatelyAfterCommand)]
    public required string PackageIdentifier { get; set; }

    /// <summary>
    ///     Gets or sets the new package version to submit.
    /// </summary>
    [CliOption("--version")]
    public string? Version { get; set; }

    /// <summary>
    ///     Gets or sets the installer download URLs for the new package version.
    /// </summary>
    [CliOption("--urls", AllowMultiple = true)]
    public IEnumerable<string>? Urls { get; set; }

    /// <summary>
    ///     Gets or sets the GitHub token used to authenticate the pull request submitted to the WinGet package repository.
    /// </summary>
    [CliOption("--token")]
    public string? Token { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the generated manifest pull request is submitted automatically.
    /// </summary>
    [CliFlag("--submit")]
    public bool? Submit { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the command runs without submitting any changes.
    /// </summary>
    [CliFlag("--dry-run")]
    public bool? DryRun { get; set; }

    /// <summary>
    ///     Gets or sets the name of the tool credited as the manifest's creator.
    /// </summary>
    [CliOption("--created-with")]
    public string? CreatedWith { get; set; }

    /// <summary>
    ///     Gets or sets the URL of the tool credited as the manifest's creator.
    /// </summary>
    [CliOption("--created-with-url")]
    public string? CreatedWithUrl { get; set; }
}
