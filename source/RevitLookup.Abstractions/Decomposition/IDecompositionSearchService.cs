namespace RevitLookup.Abstractions.Decomposition;

/// <summary>
///     Defines a contract that searches decomposed objects and members by a text query.
/// </summary>
public interface IDecompositionSearchService
{
    /// <summary>
    ///     Searches <paramref name="objects" />, and the members of <paramref name="selectedObject" /> when given, for a match against <paramref name="query" />.
    /// </summary>
    /// <param name="query">The text to search for.</param>
    /// <param name="selectedObject">The currently selected object whose members are searched, or <see langword="null" /> to search only <paramref name="objects" />.</param>
    /// <param name="objects">The objects to search.</param>
    /// <returns>The objects and members that match <paramref name="query" />.</returns>
    /// <remarks>
    ///     The service remembers the object selected by the previous call and reuses it while <paramref name="selectedObject" /> is <see langword="null" /> and <paramref name="query" /> is not empty.
    /// </remarks>
    (List<ObservableDecomposedObject> FilteredObjects, List<ObservableDecomposedMember> FilteredMembers) Search(
        string query,
        ObservableDecomposedObject? selectedObject,
        List<ObservableDecomposedObject> objects);

    /// <summary>
    ///     Searches the members of <paramref name="value" /> for a match against <paramref name="query" />.
    /// </summary>
    /// <param name="query">The text to search for.</param>
    /// <param name="value">The object whose members are searched.</param>
    /// <returns>The members that match <paramref name="query" />, or all members of <paramref name="value" /> when none match.</returns>
    List<ObservableDecomposedMember> SearchMembers(string query, ObservableDecomposedObject value);
}
