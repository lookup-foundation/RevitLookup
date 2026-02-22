using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.ObservableModels.Decomposition;
using RevitLookup.Abstractions.Services.Application;
using RevitLookup.Abstractions.Services.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.ViewModels.Decomposition;
using RevitLookup.Services.Decomposition;
using RevitLookup.UI.Framework.Extensions;
using RevitLookup.UI.Framework.Views.Decomposition;

namespace RevitLookup.ViewModels.Decomposition;

[UsedImplicitly]
public sealed partial class EventsSummaryViewModel(
    IServiceProvider serviceProvider,
    INotificationService notificationService,
    IDecompositionService decompositionService,
    IDecompositionSearchService searchService,
    EventsMonitoringService monitoringService,
    ILogger<DecompositionSummaryViewModel> logger)
    : ObservableObject, IEventsSummaryViewModel
{
    private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current!;

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private ObservableDecomposedObject? _selectedDecomposedObject;
    [ObservableProperty] private List<ObservableDecomposedObject> _decomposedObjects = [];
    [ObservableProperty] private ObservableCollection<ObservableDecomposedObject> _filteredDecomposedObjects = [];

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

    public Task OnNavigatedToAsync()
    {
        monitoringService.EventInvoked += OnEventInvoked;
        return Task.CompletedTask;
    }

    public Task OnNavigatedFromAsync()
    {
        monitoringService.EventInvoked -= OnEventInvoked;
        return Task.CompletedTask;
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
            logger.LogError(exception, "Search error");
            notificationService.ShowError("Search error", exception);
        }
    }

    async partial void OnSearchTextChanged(string value)
    {
        try
        {
            var results = await SearchObjectsAsync(value);
            if (results.FilteredObjects.Count != FilteredDecomposedObjects.Count)
            {
                FilteredDecomposedObjects = results.FilteredObjects.ToObservableCollection();
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

    private async Task<(List<ObservableDecomposedObject> FilteredObjects, List<ObservableDecomposedMember> FilteredMembers)> SearchObjectsAsync(string query)
    {
        return await Task.Run(() => searchService.Search(query, SelectedDecomposedObject, DecomposedObjects));
    }

    private void OnEventInvoked(object value, string eventName)
    {
        try
        {
            var decomposedObject = decompositionService.DecomposeAsync(value).Result;
            _synchronizationContext.Post(state =>
            {
                var viewModel = (EventsSummaryViewModel) state!;
                viewModel.PushEvent(eventName, decomposedObject);
            }, this);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Events data parsing error");
            notificationService.ShowError("Events data parsing error", exception);
        }
    }

    private async void PushEvent(string eventName, ObservableDecomposedObject decomposedObject)
    {
        try
        {
            decomposedObject.Name = $"{eventName} {DateTime.Now:HH:mm:ss}";
            DecomposedObjects.Insert(0, decomposedObject);

            var results = await SearchObjectsAsync(SearchText);
            if (results.FilteredObjects.Contains(decomposedObject))
            {
                FilteredDecomposedObjects.Insert(0, decomposedObject);
            }
            else
            {
                OnSearchTextChanged(SearchText);
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Events data parsing error");
            notificationService.ShowError("Events data parsing error", exception);
        }
    }

    private async Task FetchMembersAsync(ObservableDecomposedObject? value)
    {
        if (value is null) return;
        if (value.Members.Count > 0) return;

        value.Members = await decompositionService.DecomposeMembersAsync(value);
    }
}