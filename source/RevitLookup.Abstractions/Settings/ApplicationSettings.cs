using System.Text.Json.Serialization;
using Wpf.Ui.Animations;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace RevitLookup.Abstractions.Settings;

/// <summary>
///     Represents the application settings.
/// </summary>
[Serializable]
public sealed class ApplicationSettings
{
    /// <summary>
    ///     Gets or sets the application theme.
    /// </summary>
    /// <value>The default is <see cref="ApplicationTheme.Auto" /> on Revit 2024 and later, otherwise <see cref="ApplicationTheme.Light" />.</value>
    [JsonPropertyName("Theme")]
    public ApplicationTheme Theme { get; set; }

    /// <summary>
    ///     Gets or sets the window backdrop effect.
    /// </summary>
    /// <value>The default is <see cref="WindowBackdropType.None" />.</value>
    [JsonPropertyName("Background")]
    public WindowBackdropType Background { get; set; }

    /// <summary>
    ///     Gets or sets the page navigation transition.
    /// </summary>
    /// <value>The default is <see cref="Wpf.Ui.Animations.Transition.None" />.</value>
    [JsonPropertyName("Transition")]
    public Transition Transition { get; set; }

    /// <summary>
    ///     Gets or sets the last saved main window width.
    /// </summary>
    [JsonPropertyName("WindowWidth")]
    public double WindowWidth { get; set; }

    /// <summary>
    ///     Gets or sets the last saved main window height.
    /// </summary>
    [JsonPropertyName("WindowHeight")]
    public double WindowHeight { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether hardware-accelerated rendering is used.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("UseHardwareRendering")]
    public bool UseHardwareRendering { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the main window size is restored between sessions.
    /// </summary>
    [JsonPropertyName("UseSizeRestoring")]
    public bool UseSizeRestoring { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the Snoop Selection command is placed on the Modify tab instead of the add-ins panel.
    /// </summary>
    [JsonPropertyName("UseModifyTab")]
    public bool UseModifyTab { get; set; }
}
