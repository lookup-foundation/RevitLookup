namespace RevitLookup.Abstractions.Application;

/// <summary>
///     Defines storage locations for application data that roams with the user across multiple devices.
/// </summary>
/// <remarks>
///     Used for storing user-specific settings, configurations, or small data files
///     that need to be available on all devices in a domain environment (e.g., Active Directory).
///     Typical usage includes user preferences, UI settings, or lightweight configuration files.
/// </remarks>
public sealed partial class ResourceLocationsOptions
{
    /// <summary>
    ///     Gets or sets the add-in configurations directory.
    /// </summary>
    public required string SettingsDirectory { get; set; }

    /// <summary>
    ///     Gets or sets the application settings file path.
    /// </summary>
    public required string ApplicationSettingsPath { get; set; }

    /// <summary>
    ///     Gets or sets the LookupEngine settings file path.
    /// </summary>
    public required string DecompositionSettingsPath { get; set; }

    /// <summary>
    ///     Gets or sets the visualization settings file path.
    /// </summary>
    public required string VisualizationSettingsPath { get; set; }
}
