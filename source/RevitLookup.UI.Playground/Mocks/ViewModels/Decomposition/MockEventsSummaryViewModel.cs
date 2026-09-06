using System.Collections.ObjectModel;
using System.Windows.Media;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.ViewModels.Decomposition;
using RevitLookup.UI.Framework.Extensions;
using RevitLookup.UI.Framework.Views.Decomposition;

namespace RevitLookup.UI.Playground.Mocks.ViewModels.Decomposition;

/// <summary>
///     Represents a Playground mock of <see cref="IEventsSummaryViewModel" /> that synthesizes a stream of random sample objects instead of monitoring live Revit events.
/// </summary>
/// <param name="serviceProvider">The service provider used to resolve navigation targets when drilling into a decomposed object.</param>
/// <param name="notificationService">The service used to report decomposition and search failures.</param>
/// <param name="searchService">The service that filters decomposed objects and members by search text.</param>
/// <param name="decompositionService">The service that evaluates individual members on demand and decomposes generated sample objects.</param>
/// <param name="logger">The logger used to record decomposition, evaluation, and search failures.</param>
[UsedImplicitly]
public sealed partial class MockEventsSummaryViewModel(
    IServiceProvider serviceProvider,
    INotificationService notificationService,
    IDecompositionSearchService searchService,
    IDecompositionService decompositionService,
    ILogger<MockDecompositionSummaryViewModel> logger)
    : ObservableObject, IEventsSummaryViewModel
{
    private CancellationTokenSource? _cancellationTokenSource;

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
    public partial ObservableCollection<ObservableDecomposedObject> FilteredDecomposedObjects { get; set; } = [];

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
            await FetchMembersAsync(SelectedDecomposedObject);
        }
        catch (Exception exception)
        {
            LogMembersDecomposingFailed(logger, exception);
            notificationService.ShowError("Lookup engine error", exception);
        }
    }

    /// <inheritdoc />
    public async Task OnNavigatedToAsync()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        var faker = new Faker();
        var cancellationToken = _cancellationTokenSource.Token;
        while (!cancellationToken.IsCancellationRequested)
        {
            var decomposedObject = await GenerateRandomObjectAsync(faker);
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

            await Task.Delay(1000, cancellationToken);
        }
    }

    /// <inheritdoc />
    public Task OnNavigatedFromAsync()
    {
        if (_cancellationTokenSource is null)
        {
            return Task.CompletedTask;
        }

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = null;

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
            LogMembersDecomposingFailed(logger, exception);
            notificationService.ShowError("Lookup engine error", exception);
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

    private async Task<ObservableDecomposedObject> GenerateRandomObjectAsync(Faker faker)
    {
        object item = faker.Random.Int(0, 100) switch
        {
            < 10 => faker.Random.Int(0, 100),
            < 20 => faker.Random.Bool(),
            < 30 => faker.Random.Uuid(),
            < 40 => faker.Random.Hexadecimal(),
            < 50 => faker.Date.Future(),
            < 60 => faker.Internet.Url(),
            < 70 => faker.Internet.Email(),
            < 80 => Color.FromArgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte()),
            < 90 => faker.Random.Double(0, 100),
            _ => faker.Lorem.Word()
        };

        return await decompositionService.DecomposeAsync(item);
    }

    [LoggerMessage(LogLevel.Error, "Member evaluation failed")]
    private static partial void LogMemberEvaluationFailed(ILogger<MockDecompositionSummaryViewModel> logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Members decomposing failed")]
    private static partial void LogMembersDecomposingFailed(ILogger<MockDecompositionSummaryViewModel> logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Search error")]
    private static partial void LogSearchError(ILogger<MockDecompositionSummaryViewModel> logger, Exception exception);
}
