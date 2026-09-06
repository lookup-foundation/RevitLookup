using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RevitLookup.Abstractions.Decomposition;

/// <summary>
///     Represents the observable model for grouped decomposed objects.
/// </summary>
public sealed class ObservableDecomposedObjectsGroup : ObservableObject
{
    /// <summary>
    ///     Gets or sets the group name.
    /// </summary>
    public required string GroupName { get; set; }

    /// <summary>
    ///     Gets or sets the decomposed objects in the group.
    /// </summary>
    public required ObservableCollection<ObservableDecomposedObject> GroupItems { get; set; }
}
