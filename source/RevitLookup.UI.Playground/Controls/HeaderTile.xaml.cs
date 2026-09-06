using System.Windows;
using RevitLookup.UI.Framework.Processes;

namespace RevitLookup.UI.Playground.Controls;

/// <summary>
///     Represents a header tile that displays a title, description, and an external link.
/// </summary>
public sealed partial class HeaderTile
{
    /// <summary>
    ///     Identifies the <see cref="Title" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(HeaderTile), new PropertyMetadata(string.Empty));

    /// <summary>
    ///     Identifies the <see cref="Description" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("ColorExplanation", typeof(string), typeof(HeaderTile), new PropertyMetadata(string.Empty));

    /// <summary>
    ///     Identifies the <see cref="Link" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty LinkProperty = DependencyProperty.Register(nameof(Link), typeof(string), typeof(HeaderTile), new PropertyMetadata(null));

    /// <summary>
    ///     Identifies the <see cref="Source" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(HeaderTile), new PropertyMetadata(null));

    /// <summary>
    ///     Initializes a new instance of the <see cref="HeaderTile" /> class.
    /// </summary>
    public HeaderTile()
    {
        InitializeComponent();
    }

    /// <summary>
    ///     Gets or sets the tile's title.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    ///     Gets or sets the tile's description text.
    /// </summary>
    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    ///     Gets or sets the URL opened when the tile is clicked.
    /// </summary>
    public string Link
    {
        get => (string)GetValue(LinkProperty);
        set => SetValue(LinkProperty, value);
    }

    /// <summary>
    ///     Gets or sets the source object bound to the tile's icon.
    /// </summary>
    public object Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }


    private void OnTileClicked(object sender, RoutedEventArgs e)
    {
        ProcessTasks.StartShell(Link);
    }
}
