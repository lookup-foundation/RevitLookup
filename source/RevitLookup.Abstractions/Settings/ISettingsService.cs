namespace RevitLookup.Abstractions.Settings;

/// <summary>
///     Defines a contract that manages the application settings.
/// </summary>
public interface ISettingsService
{
    /// <summary>
    ///     Gets the application settings.
    /// </summary>
    ApplicationSettings ApplicationSettings { get; }

    /// <summary>
    ///     Gets the LookupEngine decomposition settings.
    /// </summary>
    DecompositionSettings DecompositionSettings { get; }

    /// <summary>
    ///     Gets the visualization settings.
    /// </summary>
    VisualizationSettings VisualizationSettings { get; }

    /// <summary>
    ///     Saves the settings to storage.
    /// </summary>
    void SaveSettings();

    /// <summary>
    ///     Loads the settings from storage.
    /// </summary>
    void LoadSettings();

    /// <summary>
    ///     Resets the application settings to their default values.
    /// </summary>
    /// <remarks>
    ///     Only the in-memory settings are affected; storage is left unchanged until <see cref="SaveSettings" /> is called.
    /// </remarks>
    void ResetApplicationSettings();

    /// <summary>
    ///     Resets the LookupEngine decomposition settings to their default values.
    /// </summary>
    /// <remarks>
    ///     Only the in-memory settings are affected; storage is left unchanged until <see cref="SaveSettings" /> is called.
    /// </remarks>
    void ResetDecompositionSettings();

    /// <summary>
    ///     Resets the visualization settings to their default values.
    /// </summary>
    /// <remarks>
    ///     Only the in-memory settings are affected; storage is left unchanged until <see cref="SaveSettings" /> is called.
    /// </remarks>
    void ResetVisualizationSettings();
}
