using System.Collections.ObjectModel;
using System.IO;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nice3point.Revit.Extensions.Runtime;
using RevitLookup.Abstractions.ObservableModels.Entries;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.ViewModels.Tools;
using RevitLookup.Common.Utils;
using RevitLookup.Core.RevitSettings;
using RevitLookup.UI.Framework.Views.EditDialogs;
using Wpf.Ui.Controls;

namespace RevitLookup.ViewModels.Tools;

[UsedImplicitly]
public sealed partial class RevitSettingsViewModel(
    IServiceProvider serviceProvider,
    INotificationService notificationService,
    ILogger<RevitSettingsViewModel> logger)
    : ObservableObject, IRevitSettingsViewModel
{
    private readonly RevitConfigurator _configurator = new();
    private TaskNotifier<List<ObservableIniEntry>>? _initializationTask;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(ClearFiltersCommand))] private bool _filtered;
    [ObservableProperty] private string _categoryFilter = string.Empty;
    [ObservableProperty] private string _propertyFilter = string.Empty;
    [ObservableProperty] private string _valueFilter = string.Empty;
    [ObservableProperty] private bool _showUserSettingsFilter;
    [ObservableProperty] private ObservableIniEntry? _selectedEntry;

    [ObservableProperty] private List<ObservableIniEntry> _entries = [];
    [ObservableProperty] private ObservableCollection<ObservableIniEntry> _filteredEntries = [];

    public Task<List<ObservableIniEntry>>? InitializationTask
    {
        get => _initializationTask!;
        private set => SetPropertyAndNotifyOnCompletion(ref _initializationTask, value);
    }

    public async Task InitializeAsync()
    {
        try
        {
            InitializationTask = _configurator.ReadAsync();
            Entries = await InitializationTask;
        }
        catch (Exception exception)
        {
            const string message = "Unavailable to parse Revit configuration";

            logger.LogError(exception, message);
            notificationService.ShowError(message, exception);
        }
    }

    [RelayCommand]
    private async Task CreateEntry()
    {
        try
        {
            var dialog = serviceProvider.GetRequiredService<EditSettingsEntryDialog>();
            var result = await dialog.ShowCreateDialogAsync(SelectedEntry);
            if (result == ContentDialogResult.Primary)
            {
                if (dialog.Entry.Category.IsNullOrWhiteSpace()) return;
                if (dialog.Entry.Property.IsNullOrWhiteSpace()) return;

                Entries.Add(dialog.Entry);
                FilteredEntries.Add(dialog.Entry);
                _ = Task.Run(SaveAsync);
            }
        }
        catch (Exception exception)
        {
            const string message = "Failed to create a new entry";

            logger.LogError(exception, message);
            notificationService.ShowError(message, exception);
        }
    }

    [RelayCommand]
    private void ActivateEntry(ObservableIniEntry entry)
    {
        Task.Run(SaveAsync);
    }

    [RelayCommand]
    private void DeleteEntry(ObservableIniEntry entry)
    {
        Entries.Remove(entry);
        FilteredEntries.Remove(entry);
        Task.Run(SaveAsync);
    }

    [RelayCommand]
    private void RestoreDefault(ObservableIniEntry entry)
    {
        entry.Value = entry.DefaultValue ?? string.Empty;
        Task.Run(SaveAsync);
    }

    [RelayCommand]
    private void ShowHelp()
    {
        var version = RevitApiContext.Application.VersionNumber;
        ProcessTasks.StartShell($"https://help.autodesk.com/view/RVT/{version}/ENU/?guid=GUID-9ECD669E-81D3-43E5-9970-9FA1C38E8507");
    }

    [RelayCommand]
    private void OpenSettings()
    {
        var iniFile = Path.Combine(RevitApiContext.Application.CurrentUsersDataFolderPath, "Revit.ini");
        if (!File.Exists(iniFile))
        {
            notificationService.ShowWarning("Missing settings", "Revit.ini file does not exists");
            return;
        }

        ProcessTasks.StartShell(iniFile);
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

        try
        {
            var editingValue = SelectedEntry.Clone();
            var dialog = serviceProvider.GetRequiredService<EditSettingsEntryDialog>();
            var result = await dialog.ShowUpdateDialogAsync(editingValue);
            if (result == ContentDialogResult.Primary) UpdateEntry(editingValue);
        }
        catch (Exception exception)
        {
            const string message = "Unavailable to update Revit configuration";

            logger.LogError(exception, message);
            notificationService.ShowError(message, exception);
        }
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

        Task.Run(SaveAsync);
    }

    private void ApplyFilters()
    {
        var expressions = new List<Expression<Func<ObservableIniEntry, bool>>>(4);

        if (!string.IsNullOrWhiteSpace(CategoryFilter))
        {
            expressions.Add(entry => entry.Category.Contains(CategoryFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(PropertyFilter))
        {
            expressions.Add(entry => entry.Property.Contains(PropertyFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(ValueFilter))
        {
            expressions.Add(entry => entry.Value.Contains(ValueFilter, StringComparison.OrdinalIgnoreCase));
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

    private async Task SaveAsync()
    {
        try
        {
            await _configurator.WriteAsync(Entries);
        }
        catch (Exception exception)
        {
            const string message = "Failed to save configuration file";

            logger.LogError(exception, message);
            notificationService.ShowError(message, exception);
        }
    }
}