using System.Collections;
using System.Diagnostics.CodeAnalysis;
using LookupEngine.Abstractions.Configuration;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.ObservableModels.Decomposition;
using RevitLookup.Abstractions.Services.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.ViewModels.Decomposition;
using RevitLookup.Core.Decomposition;
using RevitLookup.Services.Application;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
using Visibility = System.Windows.Visibility;

namespace RevitLookup.Services.Decomposition;

[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
[SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
public sealed class VisualDecompositionService(
    IWindowIntercomService intercomService,
    INotificationService notificationService,
    IDecompositionService decompositionService,
    IDecompositionSummaryViewModel summaryViewModel)
    : IVisualDecompositionService
{
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

            var objects = await EventHandlers.AsyncCollectionHandler.RaiseAsync(_ => RevitObjectsCollector.GetObjects(decompositionObject));
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

    private void ShowHost()
    {
        var host = intercomService.GetHost();
        if (!host.IsLoaded) return;

        host.Visibility = Visibility.Visible;
    }

    private void HideHost()
    {
        var host = intercomService.GetHost();
        if (!host.IsLoaded) return;

        host.Visibility = Visibility.Hidden;
    }

    public async Task VisualizeDecompositionAsync(object? obj)
    {
        summaryViewModel.DecomposedObjects = obj switch
        {
            ObservableDecomposedValue {Descriptor: IDescriptorEnumerator} decomposedValue => await decompositionService.DecomposeAsync((IEnumerable) decomposedValue.RawValue!),
            ObservableDecomposedValue decomposedValue => [await decompositionService.DecomposeAsync(decomposedValue.RawValue)],
            _ => [await decompositionService.DecomposeAsync(obj)]
        };
    }

    public async Task VisualizeDecompositionAsync(IEnumerable objects)
    {
        summaryViewModel.DecomposedObjects = await decompositionService.DecomposeAsync(objects);
    }

    public async Task VisualizeDecompositionAsync(ObservableDecomposedObject decomposedObject)
    {
        summaryViewModel.DecomposedObjects = [decomposedObject];
        await Task.CompletedTask;
    }

    public async Task VisualizeDecompositionAsync(List<ObservableDecomposedObject> decomposedObjects)
    {
        summaryViewModel.DecomposedObjects = decomposedObjects;
        await Task.CompletedTask;
    }
}