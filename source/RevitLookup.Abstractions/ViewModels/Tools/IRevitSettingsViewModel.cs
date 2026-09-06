using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using RevitLookup.Abstractions.Tools;

namespace RevitLookup.Abstractions.ViewModels.Tools;

/// <summary>
///     Defines a contract that represents the data for the Revit Settings view.
/// </summary>
public interface IRevitSettingsViewModel
{
    /// <summary>
    ///     Gets or sets a value indicating whether the entries are filtered.
    /// </summary>
    bool Filtered { get; set; }

    /// <summary>
    ///     Gets or sets the category filter for entries.
    /// </summary>
    string CategoryFilter { get; set; }

    /// <summary>
    ///     Gets or sets the property filter for entries.
    /// </summary>
    string PropertyFilter { get; set; }

    /// <summary>
    ///     Gets or sets the value filter for entries.
    /// </summary>
    string ValueFilter { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the user settings filter.
    /// </summary>
    bool ShowUserSettingsFilter { get; set; }

    /// <summary>
    ///     Gets or sets the selected settings entry, or <see langword="null" /> if none is selected.
    /// </summary>
    ObservableIniEntry? SelectedEntry { get; set; }

    /// <summary>
    ///     Gets or sets the list of all settings entries.
    /// </summary>
    List<ObservableIniEntry> Entries { get; set; }

    /// <summary>
    ///     Gets or sets the list of filtered settings entries.
    /// </summary>
    ObservableCollection<ObservableIniEntry> FilteredEntries { get; set; }

    /// <summary>
    ///     Gets the command that shows the help page.
    /// </summary>
    IRelayCommand ShowHelpCommand { get; }

    /// <summary>
    ///     Gets the command that opens the settings popup.
    /// </summary>
    IRelayCommand OpenSettingsCommand { get; }

    /// <summary>
    ///     Gets the command that clears all filters.
    /// </summary>
    IRelayCommand ClearFiltersCommand { get; }

    /// <summary>
    ///     Gets the command that creates a new settings entry.
    /// </summary>
    IAsyncRelayCommand CreateEntryCommand { get; }

    /// <summary>
    ///     Gets the command that sets the selected settings entry as active.
    /// </summary>
    IRelayCommand<ObservableIniEntry> ActivateEntryCommand { get; }

    /// <summary>
    ///     Gets the command that deletes the selected settings entry.
    /// </summary>
    IRelayCommand<ObservableIniEntry> DeleteEntryCommand { get; }

    /// <summary>
    ///     Gets the command that restores the default value for the selected settings entry.
    /// </summary>
    IRelayCommand<ObservableIniEntry> RestoreDefaultCommand { get; }

    /// <summary>
    ///     Gets the task that initializes the settings entries, or <see langword="null" /> if initialization has not started.
    /// </summary>
    Task<List<ObservableIniEntry>>? InitializationTask { get; }

    /// <summary>
    ///     Initializes the settings entries.
    /// </summary>
    /// <returns>A task that represents the asynchronous initialize operation.</returns>
    Task InitializeAsync();

    /// <summary>
    ///     Updates the value of the selected settings entry.
    /// </summary>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateEntryAsync();
}
