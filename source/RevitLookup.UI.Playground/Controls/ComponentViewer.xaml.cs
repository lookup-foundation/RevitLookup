using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace RevitLookup.UI.Playground.Controls;

/// <summary>
///     Represents a modal window that hosts and displays a single resolved <see cref="UIElement" /> component.
/// </summary>
[PublicAPI]
public sealed partial class ComponentViewer
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ComponentViewer" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve the hosted component.</param>
    public ComponentViewer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    /// <summary>
    ///     Resolves a <typeparamref name="T" /> component and displays it as a modal dialog.
    /// </summary>
    /// <typeparam name="T">The type of component to resolve and display.</typeparam>
    /// <returns>
    ///     <see langword="true" /> if the dialog was accepted, <see langword="false" /> if it was cancelled, or <see langword="null" /> if it closed without a result.
    /// </returns>
    public bool? ShowComponent<T>() where T : UIElement
    {
        var page = _serviceProvider.GetRequiredService<T>();
        Viewer.Content = page;

        return ShowDialog();
    }
}
