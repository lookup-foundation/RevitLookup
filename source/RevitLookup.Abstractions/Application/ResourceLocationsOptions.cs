namespace RevitLookup.Abstractions.Application;

/// <summary>
///     Represents the application's resource locations.
/// </summary>
public sealed partial class ResourceLocationsOptions
{
    /// <summary>
    ///     Gets or sets the root directory for storing application data that should roam with the user across multiple devices.
    /// </summary>
    /// <remarks>
    ///     This folder is typically used for user-specific settings, configurations, or data that needs to be synchronized between devices in a domain environment (e.g., Active Directory).
    /// </remarks>
    /// <example>User preferences, saved profiles, or small data files that should be available on all devices.</example>
    public required string ApplicationDataDirectory { get; set; }

    /// <summary>
    ///     Gets or sets the root directory for storing application data that is specific to the local machine.
    /// </summary>
    /// <remarks>
    ///     This folder is typically used for machine-specific data, cache, or temporary files that do not need to be synchronized across devices.
    /// </remarks>
    /// <example>Cache files, logs, or large data files that are only relevant to the current device.</example>
    public required string LocalApplicationDataDirectory { get; set; }
}
