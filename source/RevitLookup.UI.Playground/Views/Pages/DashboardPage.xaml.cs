using RevitLookup.UI.Playground.ViewModels.Pages;

namespace RevitLookup.UI.Playground.Views.Pages;

/// <summary>
///     Represents the page that shows the Playground's dashboard of available demos.
/// </summary>
public sealed partial class DashboardPage
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DashboardPage" /> class.
    /// </summary>
    /// <param name="viewModel">The view model that supplies data for the page.</param>
    public DashboardPage(DashboardViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
