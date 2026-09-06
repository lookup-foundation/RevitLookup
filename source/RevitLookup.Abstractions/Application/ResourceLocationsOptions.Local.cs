namespace RevitLookup.Abstractions.Application;

/// <summary>
///     Resource locations for application data specific to the local machine.
/// </summary>
/// <remarks>
///     Used for storing machine-specific data that does not need to be synchronized between devices.
///     Typical usage includes cache files, logs, temporary data, or large datasets relevant only to the current system.
/// </remarks>
public sealed partial class ResourceLocationsOptions
{
    /// <summary>
    ///     Gets or sets the temporary folder used as the download cache.
    /// </summary>
    public required string DownloadsFolder { get; set; }
}
