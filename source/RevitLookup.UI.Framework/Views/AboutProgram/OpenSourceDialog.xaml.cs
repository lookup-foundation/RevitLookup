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

using System.Windows;
using System.Windows.Documents;
using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.ViewModels.AboutProgram;
using RevitLookup.UI.Framework.Processes;
using Wpf.Ui;

namespace RevitLookup.UI.Framework.Views.AboutProgram;

/// <summary>
///     Represents a dialog that lists the third-party software used by the application and its licenses.
/// </summary>
public sealed partial class OpenSourceDialog
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="OpenSourceDialog" /> class.
    /// </summary>
    /// <param name="dialogService">The service that supplies the dialog host this dialog is shown on.</param>
    /// <param name="viewModel">The view model that provides the data for the OpenSource view.</param>
    /// <param name="themeWatcherService">The service that applies and tracks the current theme for this dialog.</param>
    public OpenSourceDialog(
        IContentDialogService dialogService,
        IOpenSourceViewModel viewModel,
        IThemeWatcherService themeWatcherService)
        : base(dialogService.GetDialogHostEx())
    {
        DataContext = viewModel;
        InitializeComponent();

        themeWatcherService.Watch(this);
    }

    private void OpenLink(object sender, RoutedEventArgs args)
    {
        var link = (Hyperlink)args.OriginalSource;
        ProcessTasks.StartShell(link.NavigateUri.OriginalString);
    }
}
