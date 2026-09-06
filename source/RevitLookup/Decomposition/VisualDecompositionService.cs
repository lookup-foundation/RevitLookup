using System.Collections;
using System.Diagnostics.CodeAnalysis;
using LookupEngine.Abstractions.Configuration;
using Nice3point.Revit.Toolkit.External;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.ViewModels.Decomposition;
using Visibility = System.Windows.Visibility;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace RevitLookup.Decomposition;

/// <summary>
///     Provides the default implementation of <see cref="IVisualDecompositionService" />.
/// </summary>
/// <param name="intercomService">The service that exposes the host window hosting the decomposition UI.</param>
/// <param name="notificationService">The service used to report a cancelled or failed visualization.</param>
/// <param name="decompositionService">The service used to decompose the requested object or objects.</param>
/// <param name="summaryViewModel">The view model that receives the decomposed objects for display.</param>
[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
[SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
public sealed partial class VisualDecompositionService(
    IWindowIntercomService intercomService,
    INotificationService notificationService,
    IDecompositionService decompositionService,
    IDecompositionSummaryViewModel summaryViewModel)
    : IVisualDecompositionService
{
    /// <inheritdoc />
    public async Task VisualizeDecompositionAsync(KnownDecompositionObject decompositionObject)
    {
        try
        {
            switch (decompositionObject)
            {
                case KnownDecompositionObject.Face:
                case KnownDecompositionObject.Edge:
                case KnownDecompositionObject.LinkedElement:
                case KnownDecompositionObject.Point:
                case KnownDecompositionObject.SubElement:
                    HideHost();
                    break;
            }

            var objects = await CollectObjectsAsyncEvent.RaiseAsync(decompositionObject);
            summaryViewModel.DecomposedObjects = await decompositionService.DecomposeAsync(objects);
        }
        catch (OperationCanceledException)
        {
            notificationService.ShowWarning("Operation cancelled", "Operation cancelled by user");
        }
        catch (Exception exception)
        {
            notificationService.ShowError("Operation cancelled", exception);
        }
        finally
        {
            ShowHost();
        }
    }

    /// <inheritdoc />
    public async Task VisualizeDecompositionAsync(object? obj)
    {
        summaryViewModel.DecomposedObjects = obj switch
        {
            ObservableDecomposedValue { Descriptor: IDescriptorEnumerator } decomposedValue => await decompositionService.DecomposeAsync((IEnumerable)decomposedValue.RawValue!),
            ObservableDecomposedValue decomposedValue => [await decompositionService.DecomposeAsync(decomposedValue.RawValue)],
            _ => [await decompositionService.DecomposeAsync(obj)]
        };
    }

    /// <inheritdoc />
    public async Task VisualizeDecompositionAsync(IEnumerable objects)
    {
        summaryViewModel.DecomposedObjects = await decompositionService.DecomposeAsync(objects);
    }

    /// <inheritdoc />
    public async Task VisualizeDecompositionAsync(ObservableDecomposedObject decomposedObject)
    {
        summaryViewModel.DecomposedObjects = [decomposedObject];
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task VisualizeDecompositionAsync(List<ObservableDecomposedObject> decomposedObjects)
    {
        summaryViewModel.DecomposedObjects = decomposedObjects;
        await Task.CompletedTask;
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private static IEnumerable CollectObjects(KnownDecompositionObject decompositionObject)
    {
        return RevitObjectsCollector.GetObjects(decompositionObject);
    }

    private void ShowHost()
    {
        var host = intercomService.GetHost();
        if (!host.IsLoaded)
        {
            return;
        }

        host.Visibility = Visibility.Visible;
    }

    private void HideHost()
    {
        var host = intercomService.GetHost();
        if (!host.IsLoaded)
        {
            return;
        }

        host.Visibility = Visibility.Hidden;
    }
}
