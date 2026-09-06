using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.ViewModels.Tools;
using RevitLookup.Tools.SearchElements;

namespace RevitLookup.ViewModels.Tools;

/// <summary>
///     Represents the view model for the Search Elements view, searching the active document and visualizing the results.
/// </summary>
/// <param name="notificationService">The service used to report search and context validation failures.</param>
/// <param name="decompositionService">The service that visualizes the decomposition of the found elements.</param>
[UsedImplicitly]
public sealed partial class SearchElementsViewModel(
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
        if (!ValidateContext())
        {
            return false;
        }

        var result = SearchText != string.Empty;
        if (!result)
        {
            return false;
        }

        var elements = RevitContext.ActiveDocument!.SearchElements(SearchText);
        if (elements.Count == 0)
        {
            notificationService.ShowWarning("Search elements", "There are no elements found for your request");
            return false;
        }

        await decompositionService.VisualizeDecompositionAsync(elements);
        return true;
    }

    private bool ValidateContext()
    {
        if (RevitContext.ActiveUiDocument is not null)
        {
            return true;
        }

        notificationService.ShowWarning("Invalid context", "There are no open documents");
        return false;
    }
}
