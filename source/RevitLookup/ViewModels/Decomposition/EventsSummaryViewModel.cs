using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.ViewModels.Decomposition;
using RevitLookup.UI.Framework.Extensions;
using RevitLookup.UI.Framework.Views.Decomposition;
using EventsMonitoringService = RevitLookup.Decomposition.EventsMonitor.EventsMonitoringService;

namespace RevitLookup.ViewModels.Decomposition;

/// <summary>
///     Represents the view model for the Events Summary view, monitoring and decomposing Revit events as they occur.
/// </summary>
/// <param name="serviceProvider">The service provider used to resolve navigation targets when drilling into a decomposed event.</param>
/// <param name="notificationService">The service used to report decomposition, search, and event parsing failures.</param>
/// <param name="decompositionService">The service that evaluates individual members on demand and decomposes incoming Revit events.</param>
/// <param name="searchService">The service that filters decomposed objects and members by search text.</param>
/// <param name="monitoringService">The service that raises an event each time a monitored Revit event fires.</param>
/// <param name="logger">The logger used to record decomposition, search, and event parsing failures.</param>
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
    public partial ObservableCollection<ObservableDecomposedObject> FilteredDecomposedObjects { get; private set; } = [];

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
    public Task OnNavigatedToAsync()
    {
        monitoringService.EventInvoked += OnEventInvoked;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task OnNavigatedFromAsync()
    {
        monitoringService.EventInvoked -= OnEventInvoked;
        return Task.CompletedTask;
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
        catch (Exception exception)
        {
            LogSearchError(logger, exception);
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
            LogSearchError(logger, exception);
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
            // Invoked on the Revit thread, where DecomposeAsync resolves synchronously through its direct-invocation external event
            var decomposedObject = decompositionService.DecomposeAsync(value).Result;
            _synchronizationContext.Post(state =>
            {
                var viewModel = (EventsSummaryViewModel)state!;
                viewModel.PushEvent(eventName, decomposedObject);
            }, this);
        }
        catch (Exception exception)
        {
            LogEventsDataParsingError(logger, exception);
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
            LogEventsDataParsingError(logger, exception);
            notificationService.ShowError("Events data parsing error", exception);
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

    [LoggerMessage(LogLevel.Error, "Member evaluation failed")]
    private static partial void LogMemberEvaluationFailed(ILogger<DecompositionSummaryViewModel> logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Members decomposing failed")]
    private static partial void LogMembersDecomposingFailed(ILogger<DecompositionSummaryViewModel> logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Search error")]
    private static partial void LogSearchError(ILogger<DecompositionSummaryViewModel> logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Events data parsing error")]
    private static partial void LogEventsDataParsingError(ILogger<DecompositionSummaryViewModel> logger, Exception exception);
}
