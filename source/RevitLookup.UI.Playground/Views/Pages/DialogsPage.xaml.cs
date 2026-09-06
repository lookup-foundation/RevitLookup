using RevitLookup.UI.Playground.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace RevitLookup.UI.Playground.Views.Pages;

/// <summary>
///     Represents a page that demonstrates content dialogs in the Playground.
/// </summary>
public sealed partial class DialogsPage : INavigableView<DialogsViewModel>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DialogsPage" /> class.
    /// </summary>
    /// <param name="viewModel">The view model that supplies data for the page.</param>
    public DialogsPage(DialogsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    /// <inheritdoc />
    public DialogsViewModel ViewModel { get; }
}
