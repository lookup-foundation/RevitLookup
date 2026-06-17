using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using RevitLookup.Abstractions.ObservableModels.Entries;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.ViewModels.Tools;
using RevitLookup.Common.Utils;
using RevitLookup.UI.Framework.Views.EditDialogs;
using Wpf.Ui.Controls;

namespace RevitLookup.UI.Playground.Mocks.ViewModels.Tools;

[UsedImplicitly]
public sealed partial class MockRevitSettingsViewModel(
    IServiceProvider serviceProvider,
    INotificationService notificationService)
    : ObservableObject, IRevitSettingsViewModel
{
    private TaskNotifier<List<ObservableIniEntry>>? _initializationTask;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ClearFiltersCommand))]
    public partial bool Filtered { get; set; }

    [ObservableProperty]
    public partial string CategoryFilter { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string PropertyFilter { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ValueFilter { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool ShowUserSettingsFilter { get; set; }

    [ObservableProperty]
    public partial ObservableIniEntry? SelectedEntry { get; set; }

    [ObservableProperty]
    public partial List<ObservableIniEntry> Entries { get; set; } = [];

    [ObservableProperty]
    public partial ObservableCollection<ObservableIniEntry> FilteredEntries { get; set; } = [];

    public Task<List<ObservableIniEntry>>? InitializationTask
    {
        get => _initializationTask!;
        private set => SetPropertyAndNotifyOnCompletion(ref _initializationTask, value);
    }

    public async Task InitializeAsync()
    {
        InitializationTask = Task.Run(async () =>
        {
            await Task.Delay(1000);
            return new Faker<ObservableIniEntry>()
                .RuleFor(entry => entry.Property, faker => faker.Internet.DomainName())
                .RuleFor(entry => entry.Value, faker => faker.Internet.UrlRootedPath())
                .RuleFor(entry => entry.DefaultValue, faker => faker.Internet.UrlRootedPath().OrNull(faker, 0.3f))
                .RuleFor(entry => entry.Category, faker => faker.Company.CompanyName("{{name.lastName}}"))
                .RuleFor(entry => entry.IsActive, faker => faker.Random.Bool())
                .Generate(10000);
        });

        Entries = await InitializationTask;
    }

    [RelayCommand]
    private async Task CreateEntry()
    {
        var dialog = serviceProvider.GetRequiredService<EditSettingsEntryDialog>();
        var result = await dialog.ShowCreateDialogAsync(SelectedEntry);
        if (result == ContentDialogResult.Primary)
        {
            if (string.IsNullOrWhiteSpace(dialog.Entry.Category)) return;
            if (string.IsNullOrWhiteSpace(dialog.Entry.Property)) return;

            Entries.Add(dialog.Entry);
            FilteredEntries.Add(dialog.Entry);
        }
    }

    [RelayCommand]
    private void ActivateEntry(ObservableIniEntry entry)
    {
        //Saving
    }

    [RelayCommand]
    private void DeleteEntry(ObservableIniEntry entry)
    {
        Entries.Remove(entry);
        FilteredEntries.Remove(entry);
    }

    [RelayCommand]
    private void RestoreDefault(ObservableIniEntry entry)
    {
        entry.Value = entry.DefaultValue ?? string.Empty;
    }

    [RelayCommand]
    private void ShowHelp()
    {
        ProcessTasks.StartShell($"https://help.autodesk.com/view/RVT/{DateTime.Today.Year}/ENU/?guid=GUID-9ECD669E-81D3-43E5-9970-9FA1C38E8507");
    }

    [RelayCommand]
    private void OpenSettings()
    {
        notificationService.ShowSuccess("Settings", "Successfully opened");
    }

    [RelayCommand(CanExecute = nameof(CanClearFiltersExecute))]
    private void ClearFilters()
    {
        CategoryFilter = string.Empty;
        PropertyFilter = string.Empty;
        ValueFilter = string.Empty;
        ShowUserSettingsFilter = false;

        ApplyFilters();
    }

    partial void OnEntriesChanged(List<ObservableIniEntry> value)
    {
        ApplyFilters();
    }

    partial void OnCategoryFilterChanged(string value)
    {
        ApplyFilters();
    }

    partial void OnPropertyFilterChanged(string value)
    {
        ApplyFilters();
    }

    partial void OnValueFilterChanged(string value)
    {
        ApplyFilters();
    }

    partial void OnShowUserSettingsFilterChanged(bool value)
    {
        ApplyFilters();
    }

    public async Task UpdateEntryAsync()
    {
        if (SelectedEntry is null) return;

        var editingValue = SelectedEntry.Clone();
        var dialog = serviceProvider.GetRequiredService<EditSettingsEntryDialog>();
        var result = await dialog.ShowUpdateDialogAsync(editingValue);
        if (result == ContentDialogResult.Primary) UpdateEntry(editingValue);
    }

    private void UpdateEntry(ObservableIniEntry entry)
    {
        if (SelectedEntry is null) return;

        var forceRefresh = SelectedEntry.Category != entry.Category || SelectedEntry.Property != entry.Property;

        SelectedEntry.Category = entry.Category;
        SelectedEntry.Property = entry.Property;
        SelectedEntry.Value = entry.Value;
        SelectedEntry.IsActive = true;

        if (forceRefresh)
        {
            ApplyFilters();
        }
    }

    private void ApplyFilters()
    {
        var expressions = new List<Expression<Func<ObservableIniEntry, bool>>>(4);

        if (!string.IsNullOrWhiteSpace(CategoryFilter))
        {
            expressions.Add(entry => entry.Category.Contains((string) CategoryFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(PropertyFilter))
        {
            expressions.Add(entry => entry.Property.Contains((string) PropertyFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(ValueFilter))
        {
            expressions.Add(entry => entry.Value.Contains((string) ValueFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (ShowUserSettingsFilter)
        {
            expressions.Add(entry => entry.IsActive);
        }

        if (expressions.Count == 0)
        {
            FilteredEntries = new ObservableCollection<ObservableIniEntry>(Entries);
            Filtered = false;
        }
        else
        {
            IEnumerable<ObservableIniEntry> filtered = Entries;

            foreach (var expression in expressions)
            {
                filtered = filtered.Where(expression.Compile());
            }

            FilteredEntries = new ObservableCollection<ObservableIniEntry>(filtered.ToList());
            Filtered = true;
        }
    }

    private bool CanClearFiltersExecute()
    {
        return Filtered;
    }
}