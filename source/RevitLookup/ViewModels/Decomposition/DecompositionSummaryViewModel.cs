using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Autodesk.Revit.Exceptions;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.ObservableModels.Decomposition;
using RevitLookup.Abstractions.Services.Application;
using RevitLookup.Abstractions.Services.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.ViewModels.Decomposition;
using RevitLookup.UI.Framework.Extensions;
using RevitLookup.UI.Framework.Views.Decomposition;

namespace RevitLookup.ViewModels.Decomposition;

[UsedImplicitly]
public sealed partial class DecompositionSummaryViewModel(
    IServiceProvider serviceProvider,
    IDecompositionService decompositionService,
    IDecompositionSearchService searchService,
    INotificationService notificationService,
    ILogger<DecompositionSummaryViewModel> logger)
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
            logger.LogError(exception, "Members decomposing failed");
            notificationService.ShowError("Lookup engine error", exception);
        }
    }

    [RelayCommand]
    private async Task ForceEvaluateMember(ObservableDecomposedMember member)
    {
        try
        {
            await decompositionService.EvaluateMemberAsync(member);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Member evaluation failed");
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
            logger.LogError(exception, "Members decomposing failed");
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
            logger.LogError(exception, "Search error");
            notificationService.ShowError("Search error", exception);
        }
    }

    private async Task FetchMembersAsync(ObservableDecomposedObject? value)
    {
        if (value is null) return;
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
}