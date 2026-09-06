using System.Collections.ObjectModel;
using RevitLookup.Abstractions.Decomposition;

namespace RevitLookup.Abstractions.ViewModels.Decomposition;

/// <summary>
///     Defines a contract that represents the data for the Decomposition Summary view.
/// </summary>
public interface IDecompositionSummaryViewModel : ISummaryViewModel
{
    /// <summary>
    ///     Gets the list of filtered decomposed objects, grouped for display.
    /// </summary>
    ObservableCollection<ObservableDecomposedObjectsGroup> FilteredDecomposedObjects { get; }

    /// <summary>
    ///     Removes the specified item from the decomposed objects.
    /// </summary>
    /// <param name="target">The item to remove.</param>
    void RemoveItem(object target);
}
