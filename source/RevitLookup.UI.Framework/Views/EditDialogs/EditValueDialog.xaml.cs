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
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace RevitLookup.UI.Framework.Views.EditDialogs;

/// <summary>
///     Represents a dialog that shows a single named value in an editable text box.
/// </summary>
public sealed partial class EditValueDialog
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EditValueDialog" /> class.
    /// </summary>
    /// <param name="dialogService">The service that supplies the dialog host this dialog is shown on.</param>
    /// <param name="themeWatcherService">The service that applies and tracks the current theme for this dialog.</param>
    public EditValueDialog(IContentDialogService dialogService, IThemeWatcherService themeWatcherService) : base(dialogService.GetDialogHostEx())
    {
        InitializeComponent();
        themeWatcherService.Watch(this);
    }

    /// <summary>
    ///     Gets the current text of the value box.
    /// </summary>
    public string Value => ValueBox.Text;

    /// <summary>
    ///     Shows the dialog with the default title.
    /// </summary>
    /// <param name="name">The label for the value.</param>
    /// <param name="value">The value shown and offered for editing.</param>
    /// <returns>A task that represents the asynchronous show operation. The result is the button the user closed the dialog with.</returns>
    public async Task<ContentDialogResult> ShowAsync(string name, string value)
    {
        ValueLabel.Content = name;
        ValueBox.Text = value;
        ValueBox.PlaceholderText = value;

        return await ShowAsync();
    }

    /// <summary>
    ///     Shows the dialog with the specified title.
    /// </summary>
    /// <param name="name">The label for the value.</param>
    /// <param name="value">The value shown and offered for editing.</param>
    /// <param name="caption">The title shown on the dialog.</param>
    /// <returns>A task that represents the asynchronous show operation. The result is the button the user closed the dialog with.</returns>
    public async Task<ContentDialogResult> ShowAsync(string name, string value, string caption)
    {
        Title = caption;

        ValueLabel.Content = name;
        ValueBox.Text = value;
        ValueBox.PlaceholderText = value;

        return await ShowAsync();
    }
}
