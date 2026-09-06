using ModularPipelines.Attributes;
using ModularPipelines.Options;

namespace Build.Komac.Options;

/// <summary>
///     Represents the command-line options for the Komac <c>list-versions</c> command.
/// </summary>
/// <remarks>
///     The command lists the published versions of a WinGet package.
/// </remarks>
[PublicAPI]
[CliSubCommand("list-versions")]
public sealed record KomacListVersionsOptions : CommandLineToolOptions
{
    /// <summary>
    ///     Gets or sets the WinGet package identifier to query.
    /// </summary>
    [CliArgument(0, Placement = ArgumentPlacement.ImmediatelyAfterCommand)]
    public required string PackageIdentifier { get; set; }

    /// <summary>
    ///     Gets or sets the GitHub token used to authenticate the WinGet package repository request.
    /// </summary>
    [CliOption("--token")]
    public string? Token { get; set; }
}
