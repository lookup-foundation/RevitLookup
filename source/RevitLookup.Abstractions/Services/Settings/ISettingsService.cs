using RevitLookup.Abstractions.Models.Settings;

namespace RevitLookup.Abstractions.Services.Settings;

/// <summary>
///     Service for managing the application settings.
/// </summary>
public interface ISettingsService
{
    /// <summary>
    ///     Represents the application settings.
    /// </summary>
    ApplicationSettings ApplicationSettings { get; }
    
    /// <summary>
    ///     Represents the LookupEngine settings.
    /// </summary>
    DecompositionSettings DecompositionSettings { get; }
    
    /// <summary>
    ///     Represents the visualization settings.
    /// </summary>
    VisualizationSettings VisualizationSettings { get; }
    
    /// <summary>
    ///     Save the settings to the storage.
    /// </summary>
    void SaveSettings();
    
    /// <summary>
    ///     Load the settings from the storage.
    /// </summary>
    void LoadSettings();
    
    /// <summary>
    ///     Reset the application settings to the default values. Only in-memory settings will be affected.
    /// </summary>
    void ResetApplicationSettings();
    
    /// <summary>
    ///     Reset the LookupEngine settings to the default values. Only in-memory settings will be affected.
    /// </summary>
    void ResetDecompositionSettings();
    
    /// <summary>
    ///     Reset the visualization settings to the default values. Only in-memory settings will be affected.
    /// </summary>
    void ResetVisualizationSettings();
}