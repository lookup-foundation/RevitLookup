using System.Collections;

namespace RevitLookup.Abstractions.Decomposition;

/// <summary>
///     Defines a contract that provides observable models for LookupEngine decomposition results.
/// </summary>
public interface IDecompositionService
{
    /// <summary>
    ///     Gets the navigation history of the decomposition stack.
    /// </summary>
    List<ObservableDecomposedObject> DecompositionStackHistory { get; }

    /// <summary>
    ///     Decomposes <paramref name="obj" /> into an observable model.
    /// </summary>
    /// <param name="obj">The object to decompose.</param>
    /// <returns>A task that represents the asynchronous decompose operation. The task result contains the observable model of <paramref name="obj" />.</returns>
    Task<ObservableDecomposedObject> DecomposeAsync(object? obj);

    /// <summary>
    ///     Decomposes <paramref name="objects" /> into observable models.
    /// </summary>
    /// <param name="objects">The objects to decompose.</param>
    /// <returns>A task that represents the asynchronous decompose operation. The task result contains the observable models of <paramref name="objects" />.</returns>
    Task<List<ObservableDecomposedObject>> DecomposeAsync(IEnumerable objects);

    /// <summary>
    ///     Decomposes the members of <paramref name="decomposedObject" /> into observable models.
    /// </summary>
    /// <param name="decomposedObject">The object whose members are decomposed.</param>
    /// <returns>A task that represents the asynchronous decompose operation. The task result contains the observable models of the members.</returns>
    Task<List<ObservableDecomposedMember>> DecomposeMembersAsync(ObservableDecomposedObject decomposedObject);

    /// <summary>
    ///     Evaluates a deferred member and updates <paramref name="decomposedMember" /> in place with the result.
    /// </summary>
    /// <param name="decomposedMember">The member to evaluate.</param>
    /// <returns>A task that represents the asynchronous evaluate operation.</returns>
    Task EvaluateMemberAsync(ObservableDecomposedMember decomposedMember);

    /// <summary>
    ///     Evaluates a deferred member inside a Revit transaction and updates <paramref name="decomposedMember" /> in place with the result.
    /// </summary>
    /// <param name="decomposedMember">The member to evaluate.</param>
    /// <returns>A task that represents the asynchronous evaluate operation.</returns>
    Task EvaluateMemberWithTransactionAsync(ObservableDecomposedMember decomposedMember);
}
