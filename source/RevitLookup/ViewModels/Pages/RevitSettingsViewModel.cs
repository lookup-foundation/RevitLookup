// Copyright 2003-2024 by Autodesk, Inc.
// 
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
// 
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
// 
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.

using System.Collections.ObjectModel;
using System.IO;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nice3point.Revit.Extensions.Runtime;
using RevitLookup.Core.Modules.Configuration;
using RevitLookup.Services;
using RevitLookup.Utils;
using RevitLookup.ViewModels.ObservableObjects;
using RevitLookup.Views.Dialogs;
using Wpf.Ui.Controls;

namespace RevitLookup.ViewModels.Pages;

#nullable enable
public sealed partial class RevitSettingsViewModel(
    ILogger<RevitSettingsViewModel> logger,
    NotificationService notificationService,
    IServiceProvider serviceProvider)
    : ObservableObject
{
    private readonly RevitConfigurator _configurator = new();
    private TaskNotifier<List<ObservableRevitSettingsEntry>>? _initializationTask;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(ClearFiltersCommand))] private bool _filtered;
    [ObservableProperty] private string _categoryFilter = string.Empty;
    [ObservableProperty] private string _propertyFilter = string.Empty;
    [ObservableProperty] private string _valueFilter = string.Empty;
    [ObservableProperty] private bool _showUserSettingsFilter;
    [ObservableProperty] private ObservableRevitSettingsEntry? _selectedEntry;

    [ObservableProperty] private List<ObservableRevitSettingsEntry> _entries = [];
    [ObservableProperty] private ObservableCollection<ObservableRevitSettingsEntry> _filteredEntries = [];

    public Task<List<ObservableRevitSettingsEntry>>? InitializationTask
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
                if (string.IsNullOrWhiteSpace(dialog.Entry.Category)) return;
                if (string.IsNullOrWhiteSpace(dialog.Entry.Property)) return;

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
    private void ActivateEntry(ObservableRevitSettingsEntry entry)
    {
        Task.Run(SaveAsync);
    }

    [RelayCommand]
    private void DeleteEntry(ObservableRevitSettingsEntry entry)
    {
        Entries.Remove(entry);
        FilteredEntries.Remove(entry);
        Task.Run(SaveAsync);
    }

    [RelayCommand]
    private void RestoreDefault(ObservableRevitSettingsEntry entry)
    {
        entry.Value = entry.DefaultValue ?? string.Empty;
        Task.Run(SaveAsync);
    }

    [RelayCommand]
    private void ShowHelp()
    {
        var version = Context.Application.VersionNumber;
        ProcessTasks.StartShell($"https://help.autodesk.com/view/RVT/{version}/ENU/?guid=GUID-9ECD669E-81D3-43E5-9970-9FA1C38E8507");
    }

    [RelayCommand]
    private void OpenSettings()
    {
        var iniFile = Context.Application.CurrentUsersDataFolderPath.AppendPath("Revit.ini");
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

    partial void OnEntriesChanged(List<ObservableRevitSettingsEntry>? value)
    {
        if (value is null) return;

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

    private void UpdateEntry(ObservableRevitSettingsEntry entry)
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
        var expressions = new List<Expression<Func<ObservableRevitSettingsEntry, bool>>>(4);

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
            FilteredEntries = new ObservableCollection<ObservableRevitSettingsEntry>(Entries);
            Filtered = false;
        }
        else
        {
            IEnumerable<ObservableRevitSettingsEntry> filtered = Entries;

            foreach (var expression in expressions)
            {
                filtered = filtered.Where(expression.Compile());
            }

            FilteredEntries = new ObservableCollection<ObservableRevitSettingsEntry>(filtered.ToList());
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