using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Autodesk.Revit.Exceptions;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.ViewModels.Decomposition;
using RevitLookup.UI.Framework.Extensions;
using RevitLookup.UI.Framework.Views.Decomposition;

namespace RevitLookup.ViewModels.Decomposition;

/// <summary>
///     Represents the view model for the Decomposition Summary view, decomposing and filtering Revit objects for inspection.
/// </summary>
/// <param name="serviceProvider">The service provider used to resolve navigation targets when drilling into a decomposed object.</param>
/// <param name="decompositionService">The service that evaluates individual members on demand.</param>
/// <param name="searchService">The service that filters decomposed objects and members by search text.</param>
/// <param name="notificationService">The service used to report decomposition and search failures.</param>
/// <param name="logger">The logger used to record decomposition, evaluation, and search failures.</param>
[UsedImplicitly]
public sealed partial class DecompositionSummaryViewModel(
    IServiceProvider serviceProvider,
    IDecompositionService decompositionService,
    IDecompositionSearchService searchService,
    INotificationService notificationService,
    ILogger<DecompositionSummaryViewModel> logger)
    : ObservableObject, IDecompositionSummaryViewModel
{
    /// <inheritdoc />
    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    /// <inheritdoc />
    [ObservableProperty]
    public partial ObservableDecomposedObject? SelectedDecomposedObject { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial List<ObservableDecomposedObject> DecomposedObjects { get; set; } = [];

    /// <inheritdoc />
    [ObservableProperty]
    public partial ObservableCollection<ObservableDecomposedObjectsGroup> FilteredDecomposedObjects { get; private set; } = [];

    /// <inheritdoc />
    public void Navigate(object? value)
    {
        Host.GetService<IUiOrchestratorService>()
            .AddParent(serviceProvider)
            .AddStackHistory(SelectedDecomposedObject!)
            .Decompose(value)
            .Show<DecompositionSummaryPage>();
    }

    /// <inheritdoc />
    public void Navigate(ObservableDecomposedObject value)
    {
        Host.GetService<IUiOrchestratorService>()
            .AddParent(serviceProvider)
            .Decompose(value)
            .Show<DecompositionSummaryPage>();
    }

    /// <inheritdoc />
    public void Navigate(List<ObservableDecomposedObject> values)
    {
        Host.GetService<IUiOrchestratorService>()
            .AddParent(serviceProvider)
            .Decompose(values)
            .Show<DecompositionSummaryPage>();
    }

    /// <inheritdoc />
    public async Task RefreshMembersAsync()
    {
        foreach (var decomposedObject in DecomposedObjects)
        {
            decomposedObject.Members.Clear();
        }

        try
        {
            if (SelectedDecomposedObject is null)
            {
                return;
            }

            await FetchMembersAsync(SelectedDecomposedObject);
            SelectedDecomposedObject.FilteredMembers = searchService.SearchMembers(SearchText, SelectedDecomposedObject);
        }
        catch (Exception exception)
        {
            LogMembersDecomposingFailed(logger, exception);
            notificationService.ShowError("Lookup engine error", exception);
        }
    }

    /// <inheritdoc />
    public void RemoveItem(object target)
    {
        switch (target)
        {
            case ObservableDecomposedObject decomposedObject:
                for (var i = FilteredDecomposedObjects.Count - 1; i >= 0; i--)
                {
                    var groupToRemove = FilteredDecomposedObjects[i];
                    if (!groupToRemove.GroupItems.Remove(decomposedObject))
                    {
                        continue;
                    }

                    //Remove the empty group
                    if (groupToRemove.GroupItems.Count == 0)
                    {
                        FilteredDecomposedObjects.Remove(groupToRemove);
                    }
                }

                if (DecomposedObjects.Remove(decomposedObject))
                {
                    //Notify UI to update placeholders
                    if (DecomposedObjects.Count == 0)
                    {
                        OnPropertyChanged(nameof(DecomposedObjects));
                    }
                }

                break;
            case ObservableDecomposedMember:
                //Do nothing ??
                break;
        }
    }

    [RelayCommand]
    private async Task ForceEvaluateMemberAsync(ObservableDecomposedMember member)
    {
        try
        {
            await decompositionService.EvaluateMemberAsync(member);
        }
        catch (Exception exception)
        {
            LogMemberEvaluationFailed(logger, exception);
            notificationService.ShowError("Lookup engine error", exception);
        }
    }

    [RelayCommand]
    private async Task EvaluateMemberWithTransactionAsync(ObservableDecomposedMember member)
    {
        try
        {
            await decompositionService.EvaluateMemberWithTransactionAsync(member);
        }
        catch (Exception exception)
        {
            LogMemberEvaluationFailed(logger, exception);
            notificationService.ShowError("Lookup engine error", exception);
        }
    }

    partial void OnDecomposedObjectsChanged(List<ObservableDecomposedObject> value)
    {
        SearchText = string.Empty;
        FilteredDecomposedObjects.Clear();

        OnSearchTextChanged(SearchText);
    }

    async partial void OnSelectedDecomposedObjectChanged(ObservableDecomposedObject? value)
    {
        try
        {
            if (value is null)
            {
                return;
            }

            await FetchMembersAsync(value);
            if (FilteredDecomposedObjects.Count > 1)
            {
                value.FilteredMembers = value.Members;
                return;
            }

            value.FilteredMembers = searchService.SearchMembers(SearchText, value);
        }
        catch (InvalidObjectException exception)
        {
            notificationService.ShowError("Invalid object", exception);
        }
        catch (InternalException)
        {
            notificationService.ShowError(
                "Invalid object",
                "A problem in the Revit code. Usually occurs when a managed API object is no longer valid and is unloaded from memory");
        }
        catch (SEHException)
        {
            notificationService.ShowError(
                "Revit API internal error",
                "A problem in the Revit code. Usually occurs when a managed API object is no longer valid and is unloaded from memory");
        }
        catch (Exception exception)
        {
            LogMembersDecomposingFailed(logger, exception);
            notificationService.ShowError("Lookup engine error", exception);
        }
    }

    async partial void OnSearchTextChanged(string value)
    {
        try
        {
            var results = await Task.Run(() => searchService.Search(value, SelectedDecomposedObject, DecomposedObjects));

            if (FilteredDecomposedObjects.Sum(static group => group.GroupItems.Count) != results.FilteredObjects.Count)
            {
                FilteredDecomposedObjects = await Task.Run(() => ApplyGrouping(results.FilteredObjects));
            }

            if (SelectedDecomposedObject is not null)
            {
                if (results.FilteredObjects.Contains(SelectedDecomposedObject))
                {
                    SelectedDecomposedObject.FilteredMembers = results.FilteredMembers;
                }
            }
        }
        catch (Exception exception)
        {
            LogSearchError(logger, exception);
            notificationService.ShowError("Search error", exception);
        }
    }

    private async Task FetchMembersAsync(ObservableDecomposedObject? value)
    {
        if (value is null)
        {
            return;
        }

        if (value.Members.Count > 0)
        {
            return;
        }

        value.Members = await decompositionService.DecomposeMembersAsync(value);
    }

    private static ObservableCollection<ObservableDecomposedObjectsGroup> ApplyGrouping(List<ObservableDecomposedObject> objects)
    {
        return objects
            .OrderBy(static data => data.TypeName)
            .ThenBy(static data => data.Name)
            .GroupBy(static data => data.TypeName)
            .Select(static group => new ObservableDecomposedObjectsGroup
            {
                GroupName = group.Key,
                GroupItems = group.ToObservableCollection()
            })
            .ToObservableCollection();
    }

    [LoggerMessage(LogLevel.Error, "Members decomposing failed")]
    private static partial void LogMembersDecomposingFailed(ILogger<DecompositionSummaryViewModel> logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Member evaluation failed")]
    private static partial void LogMemberEvaluationFailed(ILogger<DecompositionSummaryViewModel> logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Search error")]
    private static partial void LogSearchError(ILogger<DecompositionSummaryViewModel> logger, Exception exception);
}
