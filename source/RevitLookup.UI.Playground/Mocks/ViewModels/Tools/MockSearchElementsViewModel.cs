using CommunityToolkit.Mvvm.ComponentModel;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.ViewModels.Tools;

namespace RevitLookup.UI.Playground.Mocks.ViewModels.Tools;

/// <summary>
///     Represents a Playground mock of <see cref="ISearchElementsViewModel" /> that visualizes the search text itself instead of searching a Revit document.
/// </summary>
/// <param name="notificationService">The service used to report an empty search result.</param>
/// <param name="decompositionService">The service that visualizes the decomposition of the search text.</param>
[UsedImplicitly]
public sealed partial class MockSearchElementsViewModel(
    INotificationService notificationService,
    IVisualDecompositionService decompositionService)
    : ObservableObject, ISearchElementsViewModel
{
    /// <inheritdoc />
    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    /// <inheritdoc />
    public async Task<bool> SearchElementsAsync()
    {
        var result = SearchText != string.Empty;
        if (result)
        {
            await decompositionService.VisualizeDecompositionAsync((object)SearchText);
        }
        else
        {
            notificationService.ShowWarning("Search elements", "There are no elements found for your request");
        }

        return result;
    }
}
