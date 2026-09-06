using RevitLookup.UI.Playground.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace RevitLookup.UI.Playground.Views.Pages;

/// <summary>
///     Represents a page that demonstrates nested page navigation in the Playground.
/// </summary>
public sealed partial class PagesPage : INavigableView<PagesViewModel>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PagesPage" /> class.
    /// </summary>
    /// <param name="viewModel">The view model that supplies data for the page.</param>
    public PagesPage(PagesViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    /// <inheritdoc />
    public PagesViewModel ViewModel { get; }
}
