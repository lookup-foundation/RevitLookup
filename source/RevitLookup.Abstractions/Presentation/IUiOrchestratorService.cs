using System.Collections;
using System.Windows.Controls;
using RevitLookup.Abstractions.Decomposition;

namespace RevitLookup.Abstractions.Presentation;

/// <summary>
///     Defines a contract for the RevitLookup UI orchestration API.
/// </summary>
public interface IUiOrchestratorService : IRelationshipOrchestrator, IDecompositionOrchestrator, INavigationOrchestrator, IInteractionOrchestrator;

/// <summary>
///     Defines a contract for parent-child orchestrator communication.
/// </summary>
public interface IRelationshipOrchestrator
{
    /// <summary>
    ///     Adds a parent service provider to communicate with the child orchestrator.
    /// </summary>
    /// <param name="serviceProvider">The parent's service provider.</param>
    /// <returns>The <see cref="IHistoryOrchestrator" /> for chaining.</returns>
    IHistoryOrchestrator AddParent(IServiceProvider serviceProvider);
}

/// <summary>
///     Defines a contract for the UI navigation history.
/// </summary>
public interface IHistoryOrchestrator : IDecompositionOrchestrator
{
    /// <summary>
    ///     Adds <paramref name="item" /> to the navigation history.
    /// </summary>
    /// <param name="item">The object to add to the history.</param>
    /// <returns>The <see cref="IDecompositionOrchestrator" /> for chaining.</returns>
    IDecompositionOrchestrator AddStackHistory(ObservableDecomposedObject item);
}

/// <summary>
///     Defines a contract that decomposes an object for the UI orchestrator.
/// </summary>
public interface IDecompositionOrchestrator
{
    /// <summary>
    ///     Decomposes the known Revit object.
    /// </summary>
    /// <param name="knownObject">The known Revit object to decompose.</param>
    /// <returns>The <see cref="INavigationOrchestrator" /> for chaining.</returns>
    INavigationOrchestrator Decompose(KnownDecompositionObject knownObject);

    /// <summary>
    ///     Decomposes the CLR object.
    /// </summary>
    /// <param name="input">The object to decompose.</param>
    /// <returns>The <see cref="INavigationOrchestrator" /> for chaining.</returns>
    INavigationOrchestrator Decompose(object? input);

    /// <summary>
    ///     Decomposes the collection of objects.
    /// </summary>
    /// <param name="input">The objects to decompose.</param>
    /// <returns>The <see cref="INavigationOrchestrator" /> for chaining.</returns>
    INavigationOrchestrator Decompose(IEnumerable input);

    /// <summary>
    ///     Decomposes the already decomposed object.
    /// </summary>
    /// <param name="decomposedObject">The decomposed object.</param>
    /// <returns>The <see cref="INavigationOrchestrator" /> for chaining.</returns>
    INavigationOrchestrator Decompose(ObservableDecomposedObject decomposedObject);

    /// <summary>
    ///     Decomposes the collection of already decomposed objects.
    /// </summary>
    /// <param name="decomposedObjects">The decomposed objects.</param>
    /// <returns>The <see cref="INavigationOrchestrator" /> for chaining.</returns>
    INavigationOrchestrator Decompose(List<ObservableDecomposedObject> decomposedObjects);
}

/// <summary>
///     Defines a contract for UI navigation.
/// </summary>
public interface INavigationOrchestrator
{
    /// <summary>
    ///     Opens the RevitLookup instance and navigates to the specified page.
    /// </summary>
    /// <typeparam name="T">The page type to navigate to.</typeparam>
    /// <returns>The <see cref="IInteractionOrchestrator" /> for chaining.</returns>
    IInteractionOrchestrator Show<T>() where T : Page;
}

/// <summary>
///     Defines a contract for running services against the UI orchestrator.
/// </summary>
public interface IInteractionOrchestrator
{
    /// <summary>
    ///     Runs the service on the UI thread.
    /// </summary>
    /// <typeparam name="T">The service type to resolve.</typeparam>
    /// <param name="handler">The action invoked with the resolved service.</param>
    void RunService<T>(Action<T> handler) where T : class;
}
