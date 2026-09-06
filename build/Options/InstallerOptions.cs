namespace Build.Options;

/// <summary>
///     Represents the installer settings used to package the add-in.
/// </summary>
[PublicAPI]
public sealed record InstallerOptions
{
    /// <summary>
    ///     Gets the installer upgrade codes keyed by compile configuration.
    /// </summary>
    /// <remarks>
    ///     The code identifies the product across releases.
    ///     A configuration published under the installed code upgrades it in place, and a configuration published under a new code installs alongside its predecessor.
    /// </remarks>
    public Dictionary<string, Guid> UpgradeCodes { get; init; } = [];
}
