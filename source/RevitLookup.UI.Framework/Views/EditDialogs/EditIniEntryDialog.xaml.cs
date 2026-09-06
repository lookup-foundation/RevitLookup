// Copyright (c) Lookup Foundation and Contributors
// 
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
// 
// THIS PROGRAM IS PROVIDED "AS IS" AND WITH ALL FAULTS.
// NO IMPLIED WARRANTY OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE IS PROVIDED.
// THERE IS NO GUARANTEE THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.

using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.Tools;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace RevitLookup.UI.Framework.Views.EditDialogs;

/// <summary>
///     Represents a dialog that creates or updates a Revit INI settings entry.
/// </summary>
public sealed partial class EditSettingsEntryDialog
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EditSettingsEntryDialog" /> class.
    /// </summary>
    /// <param name="dialogService">The service that supplies the dialog host this dialog is shown on.</param>
    /// <param name="themeWatcherService">The service that applies and tracks the current theme for this dialog.</param>
    public EditSettingsEntryDialog(
        IContentDialogService dialogService,
        IThemeWatcherService themeWatcherService)
        : base(dialogService.GetDialogHostEx())
    {
        InitializeComponent();
        themeWatcherService.Watch(this);
    }

    /// <summary>
    ///     Gets the entry being created or updated by this dialog.
    /// </summary>
    /// <exception cref="InvalidOperationException">The dialog has not been shown through <see cref="ShowCreateDialogAsync" /> or <see cref="ShowUpdateDialogAsync" /> yet.</exception>
    public ObservableIniEntry Entry
    {
        get => field ?? throw new InvalidOperationException("Entry was never set");
        private set;
    }

    /// <summary>
    ///     Shows the dialog configured to create a new entry.
    /// </summary>
    /// <param name="selectedEntry">The entry whose category is copied to the new entry, or <see langword="null" /> to leave the category unset.</param>
    /// <returns>A task that represents the asynchronous show operation. The result is the button the user closed the dialog with.</returns>
    public async Task<ContentDialogResult> ShowCreateDialogAsync(ObservableIniEntry? selectedEntry)
    {
        Title = "Create the entry";
        PrimaryButtonText = "Create";

        Entry = new ObservableIniEntry
        {
            IsActive = true
        };

        if (selectedEntry is not null)
        {
            Entry.Category = selectedEntry.Category;
        }

        DataContext = Entry;
        return await ShowAsync();
    }

    /// <summary>
    ///     Shows the dialog configured to update an existing entry.
    /// </summary>
    /// <param name="entry">The entry to edit.</param>
    /// <returns>A task that represents the asynchronous show operation. The result is the button the user closed the dialog with.</returns>
    public async Task<ContentDialogResult> ShowUpdateDialogAsync(ObservableIniEntry entry)
    {
        Title = "Update the entry";
        PrimaryButtonText = "Update";

        Entry = entry;
        DataContext = entry;
        return await ShowAsync();
    }

    /// <inheritdoc />
    /// <remarks>
    ///     Blocks the primary button when <see cref="Entry" /> fails validation.
    /// </remarks>
    protected override void OnButtonClick(ContentDialogButton button)
    {
        if (button == ContentDialogButton.Primary)
        {
            Entry.Validate();
            if (Entry.HasErrors)
            {
                return;
            }
        }

        base.OnButtonClick(button);
    }
}
