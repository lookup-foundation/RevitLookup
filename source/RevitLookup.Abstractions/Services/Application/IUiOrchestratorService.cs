using System.Collections;
using System.Windows.Controls;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.ObservableModels.Decomposition;

namespace RevitLookup.Abstractions.Services.Application;

/// <summary>
///     API for the RevitLookup UI service.
/// </summary>
public interface IUiOrchestratorService : IRelationshipOrchestrator, IDecompositionOrchestrator, INavigationOrchestrator, IInteractionOrchestrator;

/// <summary>
///     API for parent-child communication.
/// </summary>
public interface IRelationshipOrchestrator
{
    /// <summary>
    ///     Add a parent services to communicate with the child services.
    /// </summary>
    IHistoryOrchestrator AddParent(IServiceProvider serviceProvider);
}

/// <summary>
///     API for the UI navigation history.
/// </summary>
public interface IHistoryOrchestrator : IDecompositionOrchestrator
{
    /// <summary>
    ///     Add an item to the navigation history.
    /// </summary>
    IDecompositionOrchestrator AddStackHistory(ObservableDecomposedObject item);
}

/// <summary>
///     API for the LookupEngine.
/// </summary>
public interface IDecompositionOrchestrator
{
    /// <summary>
    ///     Decompose a known Revit object.
    /// </summary>
    INavigationOrchestrator Decompose(KnownDecompositionObject knownObject);
    
    /// <summary>
    ///     Decompose the CLR object.
    /// </summary>
    INavigationOrchestrator Decompose(object? input);
    
    /// <summary>
    ///     Decompose the collection of objects.
    /// </summary>
    INavigationOrchestrator Decompose(IEnumerable input);
    
    /// <summary>
    ///     Decompose the already decomposed object.
    /// </summary>
    INavigationOrchestrator Decompose(ObservableDecomposedObject decomposedObject);
    
    /// <summary>
    ///     Decompose the collection of already decomposed objects.
    /// </summary>
    INavigationOrchestrator Decompose(List<ObservableDecomposedObject> decomposedObjects);
}

/// <summary>
///     API for UI navigation.
/// </summary>
public interface INavigationOrchestrator
{
    /// <summary>
    ///     Open the RevitLookup instance and navigate to the specified page.
    /// </summary>
    IInteractionOrchestrator Show<T>() where T : Page;
    // ILookupServiceRunStage ShowDialog<T>() where T : Page;
}

/// <summary>
///     API for running services.
/// </summary>
public interface IInteractionOrchestrator
{
    /// <summary>
    ///     Run the service in the UI thread.
    /// </summary>
    void RunService<T>(Action<T> handler) where T : class;
}