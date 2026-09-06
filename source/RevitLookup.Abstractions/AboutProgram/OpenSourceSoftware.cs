namespace RevitLookup.Abstractions.AboutProgram;

/// <summary>
///     Represents information about a third-party open-source software dependency.
/// </summary>
public sealed class OpenSourceSoftware
{
    /// <summary>
    ///     Gets or sets the name of the software.
    /// </summary>
    public required string SoftwareName { get; set; }

    /// <summary>
    ///     Gets or sets the URI of the software's project page.
    /// </summary>
    public required string SoftwareUri { get; set; }

    /// <summary>
    ///     Gets or sets the name of the software's license.
    /// </summary>
    public required string LicenseName { get; set; }

    /// <summary>
    ///     Gets or sets the URI of the license's full text.
    /// </summary>
    public required string LicenseUri { get; set; }
}
