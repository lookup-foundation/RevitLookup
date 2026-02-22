using System.Collections;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.ObservableModels.Decomposition;

namespace RevitLookup.Abstractions.Services.Decomposition;

/// <summary>
///     A visual shell for displaying decomposition result in the UI.
/// </summary>
public interface IVisualDecompositionService
{
    /// <summary>
    ///     Visualize the known Revit object in the UI.
    /// </summary>
    Task VisualizeDecompositionAsync(KnownDecompositionObject decompositionObject);
    
    /// <summary>
    ///     Visualize the CLR object in the UI.
    /// </summary>
    Task VisualizeDecompositionAsync(object? obj);
    
    /// <summary>
    ///     Visualize the collection of CLR objects in the UI.
    /// </summary>
    Task VisualizeDecompositionAsync(IEnumerable objects);
    
    /// <summary>
    ///     Visualize the already decomposed object in the UI.
    /// </summary>
    Task VisualizeDecompositionAsync(ObservableDecomposedObject decomposedObject);
    
    /// <summary>
    ///     Visualize the collection of already decomposed objects in the UI.
    /// </summary>
    Task VisualizeDecompositionAsync(List<ObservableDecomposedObject> decomposedObjects);
}