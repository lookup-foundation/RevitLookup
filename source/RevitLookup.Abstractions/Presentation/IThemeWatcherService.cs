using System.Windows;

namespace RevitLookup.Abstractions.Presentation;

/// <summary>
///     Defines a contract that applies themes to UI components and watches for theme changes.
/// </summary>
public interface IThemeWatcherService
{
    /// <summary>
    ///     Initializes the UI components and resources.
    /// </summary>
    void Initialize();

    /// <summary>
    ///     Applies the current theme to the whole application and monitors for changes.
    /// </summary>
    void ApplyTheme();

    /// <summary>
    ///     Watches for theme changes on the specified <see cref="FrameworkElement" />.
    /// </summary>
    /// <param name="frameworkElement">The element to update when the theme changes.</param>
    void Watch(FrameworkElement frameworkElement);

    /// <summary>
    ///     Stops watching for theme changes.
    /// </summary>
    void Unwatch();
}
