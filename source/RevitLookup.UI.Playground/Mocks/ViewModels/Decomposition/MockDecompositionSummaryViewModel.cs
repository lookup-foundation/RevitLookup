using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.ObservableModels.Decomposition;
using RevitLookup.Abstractions.Services.Application;
using RevitLookup.Abstractions.Services.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.ViewModels.Decomposition;
using RevitLookup.UI.Framework.Extensions;
using RevitLookup.UI.Framework.Views.Decomposition;
#if NETFRAMEWORK
using RevitLookup.UI.Framework.Extensions;
#endif

namespace RevitLookup.UI.Playground.Mocks.ViewModels.Decomposition;

[UsedImplicitly]
public sealed partial class MockDecompositionSummaryViewModel(
    IServiceProvider serviceProvider,
    IDecompositionService decompositionService,
    IDecompositionSearchService searchService,
    INotificationService notificationService,
    ILogger<MockDecompositionSummaryViewModel> logger)
    : ObservableObject, IDecompositionSummaryViewModel
{
    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ObservableDecomposedObject? SelectedDecomposedObject { get; set; }

    [ObservableProperty]
    public partial List<ObservableDecomposedObject> DecomposedObjects { get; set; } = [];

    [ObservableProperty]
    public partial ObservableCollection<ObservableDecomposedObjectsGroup> FilteredDecomposedObjects { get; private set; } = [];

    public void Navigate(object? value)
    {
        Host.GetService<IUiOrchestratorService>()
            .AddParent(serviceProvider)
            .AddStackHistory(SelectedDecomposedObject!)
            .Decompose(value)
            .Show<DecompositionSummaryPage>();
    }

    public void Navigate(ObservableDecomposedObject value)
    {
        Host.GetService<IUiOrchestratorService>()
            .AddParent(serviceProvider)
            .Decompose(value)
            .Show<DecompositionSummaryPage>();
    }

    public void Navigate(List<ObservableDecomposedObject> values)
    {
        Host.GetService<IUiOrchestratorService>()
            .AddParent(serviceProvider)
            .Decompose(values)
            .Show<DecompositionSummaryPage>();
    }

    public async Task RefreshMembersAsync()
    {
        foreach (var decomposedObject in DecomposedObjects)
        {
            decomposedObject.Members.Clear();
        }

        try
        {
            if (SelectedDecomposedObject is null) return;

            await FetchMembersAsync(SelectedDecomposedObject);
            SelectedDecomposedObject.FilteredMembers = searchService.SearchMembers(SearchText, SelectedDecomposedObject);
        }
        catch (Exception exception)
        {
            LogMembersDecomposingFailed(logger, exception);
            notificationService.ShowError("Lookup engine error", exception);
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

    public void RemoveItem(object target)
    {
        switch (target)
        {
            case ObservableDecomposedObject decomposedObject:
                for (var i = FilteredDecomposedObjects.Count - 1; i >= 0; i--)
                {
                    var groupToRemove = FilteredDecomposedObjects[i];
                    if (!groupToRemove.GroupItems.Remove(decomposedObject)) continue;

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
            if (value is null) return;

            await FetchMembersAsync(value);
            if (FilteredDecomposedObjects.Count > 1)
            {
                value.FilteredMembers = value.Members;
                return;
            }

            value.FilteredMembers = searchService.SearchMembers(SearchText, value);
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
            if (FilteredDecomposedObjects.Sum(group => group.GroupItems.Count) != results.FilteredObjects.Count)
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

    private async Task FetchMembersAsync(ObservableDecomposedObject value)
    {
        if (value.Members.Count > 0) return;

        value.Members = await decompositionService.DecomposeMembersAsync(value);
    }

    private static ObservableCollection<ObservableDecomposedObjectsGroup> ApplyGrouping(List<ObservableDecomposedObject> objects)
    {
        return objects
            .OrderBy(data => data.TypeName)
            .ThenBy(data => data.Name)
            .GroupBy(data => data.TypeName)
            .Select(group => new ObservableDecomposedObjectsGroup
            {
                GroupName = group.Key,
                GroupItems = group.ToObservableCollection()
            })
            .ToObservableCollection();
    }

    [LoggerMessage(LogLevel.Error, "Members decomposing failed")]
    private static partial void LogMembersDecomposingFailed(ILogger<MockDecompositionSummaryViewModel> logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Member evaluation failed")]
    private static partial void LogMemberEvaluationFailed(ILogger<MockDecompositionSummaryViewModel> logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Search error")]
    private static partial void LogSearchError(ILogger<MockDecompositionSummaryViewModel> logger, Exception exception);
}