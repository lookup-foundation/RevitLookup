using System.Collections;
using RevitLookup.Abstractions.ObservableModels.Decomposition;

namespace RevitLookup.Abstractions.Services.Decomposition;

/// <summary>
///     Service for proving observable models of the LookupEngine results.
/// </summary>
public interface IDecompositionService
{
    /// <summary>
    ///     Get the navigation history of the decomposition stack.
    /// </summary>
    List<ObservableDecomposedObject> DecompositionStackHistory { get; }

    /// <summary>
    ///     Decompose the object into an observable model.
    /// </summary>
    Task<ObservableDecomposedObject> DecomposeAsync(object? obj);

    /// <summary>
    ///     Decompose the objects into observable models.
    /// </summary>
    Task<List<ObservableDecomposedObject>> DecomposeAsync(IEnumerable objects);

    /// <summary>
    ///     Decompose the object members into observable models.
    /// </summary>
    Task<List<ObservableDecomposedMember>> DecomposeMembersAsync(ObservableDecomposedObject decomposedObject);

    /// <summary>
    ///     Force the evaluation of a deferred member, updating its observable model in place.
    /// </summary>
    Task EvaluateMemberAsync(ObservableDecomposedMember decomposedMember);
}