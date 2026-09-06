using System.Collections.ObjectModel;
using RevitLookup.Abstractions.Decomposition;
using Wpf.Ui.Abstractions.Controls;

namespace RevitLookup.Abstractions.ViewModels.Decomposition;

/// <summary>
///     Defines a contract that represents the data for the Events Summary view.
/// </summary>
public interface IEventsSummaryViewModel : ISummaryViewModel, INavigationAware
{
    /// <summary>
    ///     Gets the list of filtered decomposed objects.
    /// </summary>
    ObservableCollection<ObservableDecomposedObject> FilteredDecomposedObjects { get; }
}
