using System.Collections;

namespace RevitLookup.Abstractions.Decomposition;

/// <summary>
///     Defines a contract that displays a decomposition result in the UI.
/// </summary>
public interface IVisualDecompositionService
{
    /// <summary>
    ///     Decomposes <paramref name="decompositionObject" /> and displays the result in the UI.
    /// </summary>
    /// <param name="decompositionObject">The known Revit object to decompose.</param>
    /// <returns>A task that represents the asynchronous visualize operation.</returns>
    Task VisualizeDecompositionAsync(KnownDecompositionObject decompositionObject);

    /// <summary>
    ///     Decomposes <paramref name="obj" /> and displays the result in the UI.
    /// </summary>
    /// <param name="obj">The object to decompose.</param>
    /// <returns>A task that represents the asynchronous visualize operation.</returns>
    Task VisualizeDecompositionAsync(object? obj);

    /// <summary>
    ///     Decomposes <paramref name="objects" /> and displays the result in the UI.
    /// </summary>
    /// <param name="objects">The objects to decompose.</param>
    /// <returns>A task that represents the asynchronous visualize operation.</returns>
    Task VisualizeDecompositionAsync(IEnumerable objects);

    /// <summary>
    ///     Displays the already decomposed <paramref name="decomposedObject" /> in the UI.
    /// </summary>
    /// <param name="decomposedObject">The decomposed object to display.</param>
    /// <returns>A task that represents the asynchronous visualize operation.</returns>
    Task VisualizeDecompositionAsync(ObservableDecomposedObject decomposedObject);

    /// <summary>
    ///     Displays the already decomposed <paramref name="decomposedObjects" /> in the UI.
    /// </summary>
    /// <param name="decomposedObjects">The decomposed objects to display.</param>
    /// <returns>A task that represents the asynchronous visualize operation.</returns>
    Task VisualizeDecompositionAsync(List<ObservableDecomposedObject> decomposedObjects);
}
