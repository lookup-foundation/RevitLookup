using RevitLookup.Abstractions.Services.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.ViewModels.Tools;
using RevitLookup.Core.Search;

namespace RevitLookup.ViewModels.Tools;

[UsedImplicitly]
public sealed partial class SearchElementsViewModel(
    INotificationService notificationService,
    IVisualDecompositionService decompositionService)
    : ObservableObject, ISearchElementsViewModel
{
    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    public async Task<bool> SearchElementsAsync()
    {
        if (!ValidateContext()) return false;

        var result = SearchText != string.Empty;
        if (!result) return false;

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
        if (RevitContext.ActiveUiDocument is not null) return true;

        notificationService.ShowWarning("Invalid context", "There are no open documents");
        return false;
    }
}