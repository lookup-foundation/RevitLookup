using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;

namespace RevitLookup.UI.Playground.Controls;

/// <summary>
///     Represents a swatch that displays a single theme color and its brush name.
/// </summary>
public sealed class ColorTile : UserControl
{
    /// <summary>
    ///     Identifies the <see cref="TileRadius" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty TileRadiusProperty = DependencyProperty.Register(nameof(TileRadius), typeof(CornerRadius), typeof(ColorTile), new PropertyMetadata(new CornerRadius(0)));

    /// <summary>
    ///     Identifies the <see cref="ColorName" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColorNameProperty = DependencyProperty.Register(nameof(ColorName), typeof(string), typeof(ColorTile), new PropertyMetadata(string.Empty));

    /// <summary>
    ///     Identifies the <see cref="ColorExplanation" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColorExplanationProperty = DependencyProperty.Register(nameof(ColorExplanation), typeof(string), typeof(ColorTile), new PropertyMetadata(string.Empty));

    /// <summary>
    ///     Identifies the <see cref="ColorBrushName" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColorBrushNameProperty = DependencyProperty.Register(nameof(ColorBrushName), typeof(string), typeof(ColorTile), new PropertyMetadata(string.Empty));

    /// <summary>
    ///     Identifies the <see cref="ColorValue" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColorValueProperty = DependencyProperty.Register(nameof(ColorValue), typeof(string), typeof(ColorTile), new PropertyMetadata(string.Empty));

    // Using a DependencyProperty as the backing store for ShowSeparator.  This enables animation, styling, binding, etc...
    /// <summary>
    ///     Identifies the <see cref="ShowSeparator" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowSeparatorProperty = DependencyProperty.Register(nameof(ShowSeparator), typeof(bool), typeof(ColorTile), new PropertyMetadata(true));

    // Using a DependencyProperty as the backing store for ShowSeparator.  This enables animation, styling, binding, etc...
    /// <summary>
    ///     Identifies the <see cref="ShowWarning" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowWarningProperty = DependencyProperty.Register(nameof(ShowWarning), typeof(bool), typeof(ColorTile), new PropertyMetadata(false));

    static ColorTile()
    {
        CommandManager.RegisterClassCommandBinding(typeof(ColorTile), new CommandBinding(ApplicationCommands.Copy, OnCopyColorBrushClicked));
    }

    /// <summary>
    ///     Gets or sets the corner radius of the tile.
    /// </summary>
    public CornerRadius TileRadius
    {
        get => (CornerRadius)GetValue(TileRadiusProperty);
        set => SetValue(TileRadiusProperty, value);
    }

    /// <summary>
    ///     Gets or sets the display name of the color.
    /// </summary>
    public string ColorName
    {
        get => (string)GetValue(ColorNameProperty);
        set => SetValue(ColorNameProperty, value);
    }

    /// <summary>
    ///     Gets or sets the explanatory text describing the color's usage.
    /// </summary>
    public string ColorExplanation
    {
        get => (string)GetValue(ColorExplanationProperty);
        set => SetValue(ColorExplanationProperty, value);
    }

    /// <summary>
    ///     Gets or sets the name of the brush resource copied to the clipboard.
    /// </summary>
    public string ColorBrushName
    {
        get => (string)GetValue(ColorBrushNameProperty);
        set => SetValue(ColorBrushNameProperty, value);
    }

    /// <summary>
    ///     Gets or sets the textual representation of the color value.
    /// </summary>
    public string ColorValue
    {
        get => (string)GetValue(ColorValueProperty);
        set => SetValue(ColorValueProperty, value);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the separator below the tile is visible.
    /// </summary>
    public bool ShowSeparator
    {
        get => (bool)GetValue(ShowSeparatorProperty);
        set => SetValue(ShowSeparatorProperty, value);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the tile displays a warning indicator.
    /// </summary>
    public bool ShowWarning
    {
        get => (bool)GetValue(ShowWarningProperty);
        set => SetValue(ShowWarningProperty, value);
    }

    private static void OnCopyColorBrushClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not ColorTile colorTile)
        {
            return;
        }

        if (string.IsNullOrEmpty(colorTile.ColorBrushName))
        {
            return;
        }

        try
        {
            Clipboard.SetText(colorTile.ColorBrushName);
            var peer = UIElementAutomationPeer.CreatePeerForElement((ColorTile)e.OriginalSource);
            peer.RaiseNotificationEvent(
                AutomationNotificationKind.Other,
                AutomationNotificationProcessing.ImportantMostRecent,
                "Color Brush Name Copied",
                "ButtonClickedActivity"
            );
        }
        catch (Exception exception)
        {
            MessageBox.Show("Error copying to clipboard: " + exception.Message);
        }
    }
}
