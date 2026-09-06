using RevitLookup.UI.Playground.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace RevitLookup.UI.Playground.Views.Pages;

/// <summary>
///     Represents a page that lists the windows available to open in the Playground.
/// </summary>
public sealed partial class WindowsPage : INavigableView<WindowsViewModel>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsPage" /> class.
    /// </summary>
    /// <param name="viewModel">The view model that supplies data for the page.</param>
    public WindowsPage(WindowsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    /// <inheritdoc />
    public WindowsViewModel ViewModel { get; }
}
