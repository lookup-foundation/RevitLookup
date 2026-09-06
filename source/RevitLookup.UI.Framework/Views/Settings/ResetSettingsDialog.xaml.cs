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

namespace RevitLookup.UI.Framework.Views.Settings;

/// <summary>
///     Represents a dialog that lets the user choose which settings categories to reset to their defaults.
/// </summary>
public sealed partial class ResetSettingsDialog
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ResetSettingsDialog" /> class.
    /// </summary>
    /// <param name="dialogService">The service that supplies the dialog host this dialog is shown on.</param>
    /// <param name="themeWatcherService">The service that applies and tracks the current theme for this dialog.</param>
    public ResetSettingsDialog(IContentDialogService dialogService, IThemeWatcherService themeWatcherService) : base(dialogService.GetDialogHostEx())
    {
        InitializeComponent();
        themeWatcherService.Watch(this);
    }

    /// <summary>
    ///     Gets a value indicating whether the user selected the application settings for reset.
    /// </summary>
    public bool CanResetApplicationSettings => ApplicationBox.IsChecked == true;

    /// <summary>
    ///     Gets a value indicating whether the user selected the decomposition settings for reset.
    /// </summary>
    public bool CanResetDecompositionSettings => DecompositionBox.IsChecked == true;

    /// <summary>
    ///     Gets a value indicating whether the user selected the visualization settings for reset.
    /// </summary>
    public bool CanResetVisualizationSettings => VisualizationBox.IsChecked == true;
}
