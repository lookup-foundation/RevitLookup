using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using RevitLookup.Abstractions.Presentation;
using RevitLookup.UI.Framework.Controls.Automation;
using Wpf.Ui;

namespace RevitLookup.UI.Playground.Controls;

/// <summary>
///     Represents a window that hosts and navigates a single resolved <see cref="Page" />.
/// </summary>
[PublicAPI]
public sealed partial class PageViewer
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PageViewer" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve the hosted page.</param>
    /// <param name="snackbarService">The service configured to present snackbars over the hosted page.</param>
    /// <param name="dialogService">The service configured to present content dialogs over the hosted page.</param>
    /// <param name="intercomService">The service configured to expose this window as the active host.</param>
    public PageViewer(
        IServiceProvider serviceProvider,
        ISnackbarService snackbarService,
        IContentDialogService dialogService,
        IWindowIntercomService intercomService)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();

        intercomService.SetHost(this);
        dialogService.SetDialogHost(RootContentDialog);
        snackbarService.SetSnackbarPresenter(RootSnackbar);
    }

    /// <summary>
    ///     Resolves a <typeparamref name="T" /> page and navigates to it.
    /// </summary>
    /// <typeparam name="T">The type of page to resolve and display.</typeparam>
    public void ShowPage<T>() where T : Page
    {
        var page = _serviceProvider.GetRequiredService<T>();
        Viewer.Navigate(page);

        if (WindowStartupLocation == WindowStartupLocation.CenterScreen)
        {
            Viewer.SizeChanged += OnViewerFrameResized;
        }

        Show();
    }

    /// <summary>
    ///     Resolves a <typeparamref name="T" /> page, applies the given configuration, and navigates to it.
    /// </summary>
    /// <typeparam name="T">The type of page to resolve and display.</typeparam>
    /// <param name="configuration">The action invoked with the resolved page and the service provider before navigation.</param>
    public void ShowPage<T>(Action<T, IServiceProvider> configuration) where T : Page
    {
        var page = _serviceProvider.GetRequiredService<T>();
        configuration.Invoke(page, _serviceProvider);
        Viewer.Navigate(page);

        if (WindowStartupLocation == WindowStartupLocation.CenterScreen)
        {
            Viewer.SizeChanged += OnViewerFrameResized;
        }

        Show();
    }

    /// <summary>
    ///     Resolves a <typeparamref name="T" /> page, applies the given asynchronous configuration, and navigates to it.
    /// </summary>
    /// <typeparam name="T">The type of page to resolve and display.</typeparam>
    /// <param name="configuration">The asynchronous function invoked with the resolved page and the service provider before navigation.</param>
    public void ShowPage<T>(Func<T, IServiceProvider, Task> configuration) where T : Page
    {
        var page = _serviceProvider.GetRequiredService<T>();
        configuration.Invoke(page, _serviceProvider);
        Viewer.Navigate(page);

        if (WindowStartupLocation == WindowStartupLocation.CenterScreen)
        {
            Viewer.SizeChanged += OnViewerFrameResized;
        }

        Show();
    }

    private void OnViewerFrameResized(object sender, SizeChangedEventArgs args)
    {
        if (args.PreviousSize.Height == 0 || args.PreviousSize.Width == 0)
        {
            return;
        }

        var self = (Frame)sender;
        self.SizeChanged -= OnViewerFrameResized;

        //Move the owner to the screen center after navigation
        if (SizeToContent is SizeToContent.WidthAndHeight or SizeToContent.Width)
        {
            Left -= (ActualWidth - MinWidth) / 2;
        }

        if (SizeToContent is SizeToContent.WidthAndHeight or SizeToContent.Height)
        {
            Top -= (ActualHeight - MinHeight) / 2;
        }
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new NoAutomationWindowPeer(this);
    }
}
