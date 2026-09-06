using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace RevitLookup.UI.Playground.Controls;

/// <summary>
///     Represents a themed color documentation page with a title, description, and example content.
/// </summary>
[ContentProperty(nameof(ExampleContent))]
public sealed class ColorPageExample : UserControl
{
    /// <summary>
    ///     Identifies the <see cref="Description" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(ColorPageExample), new PropertyMetadata(string.Empty));

    /// <summary>
    ///     Identifies the <see cref="Title" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(ColorPageExample), new PropertyMetadata(string.Empty));

    /// <summary>
    ///     Identifies the <see cref="ExampleContent" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ExampleContentProperty = DependencyProperty.Register(nameof(ExampleContent), typeof(UIElement), typeof(ColorPageExample), new PropertyMetadata(null));

    /// <summary>
    ///     Gets or sets the description text shown below the page title.
    /// </summary>
    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    ///     Gets or sets the page title.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    ///     Gets or sets the content displayed as the color example.
    /// </summary>
    public UIElement ExampleContent
    {
        get => (UIElement)GetValue(ExampleContentProperty);
        set => SetValue(ExampleContentProperty, value);
    }
}
