using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using LookupEngine.Abstractions.Configuration;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.ViewModels.Decomposition;

namespace RevitLookup.UI.Playground.Mocks.Decomposition;

/// <summary>
///     Represents a Playground mock of <see cref="IVisualDecompositionService" /> that decomposes Playground sample data and pushes it into <paramref name="summaryViewModel" /> instead of a live Revit selection.
/// </summary>
/// <param name="intercomService">The service used to show or hide the host window around the simulated visualization delay.</param>
/// <param name="notificationService">The service used to report a cancelled or failed decomposition.</param>
/// <param name="decompositionService">The service used to decompose the requested object into an observable model.</param>
/// <param name="summaryViewModel">The view model that receives the decomposed objects.</param>
[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
[SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
public sealed class MockVisualDecompositionService(
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
                    await Task.Delay(1000);
                    break;
            }

            summaryViewModel.DecomposedObjects = await decompositionService.DecomposeAsync(new object[] { decompositionObject });
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
