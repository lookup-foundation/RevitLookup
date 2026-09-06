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
using RevitLookup.Abstractions.ViewModels.Visualization;
using Wpf.Ui;

namespace RevitLookup.UI.Framework.Views.Visualization;

/// <summary>
///     Represents a dialog that visualizes a bounding box in the active Revit view.
/// </summary>
public sealed partial class BoundingBoxVisualizationDialog
{
    private readonly IBoundingBoxVisualizationViewModel _viewModel;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BoundingBoxVisualizationDialog" /> class.
    /// </summary>
    /// <param name="dialogService">The service that supplies the dialog host this dialog is shown on.</param>
    /// <param name="viewModel">The view model that renders the bounding box in the active Revit view.</param>
    /// <param name="themeWatcherService">The service that applies and tracks the current theme for this dialog.</param>
    public BoundingBoxVisualizationDialog(
        IContentDialogService dialogService,
        IBoundingBoxVisualizationViewModel viewModel,
        IThemeWatcherService themeWatcherService)
        : base(dialogService.GetDialogHostEx())
    {
        _viewModel = viewModel;

        DataContext = _viewModel;
        InitializeComponent();

        themeWatcherService.Watch(this);
    }

    /// <summary>
    ///     Registers the bounding box for visualization and shows the dialog.
    /// </summary>
    /// <param name="boundingBoxXyz">The Revit <c>BoundingBoxXYZ</c> to visualize.</param>
    /// <returns>A task that represents the asynchronous show operation.</returns>
    public async Task ShowDialogAsync(object boundingBoxXyz)
    {
        _viewModel.RegisterServer(boundingBoxXyz);
        MonitorServerConnection();

        await ShowAsync();
    }

    private void MonitorServerConnection()
    {
        Unloaded += (_, _) => _viewModel.UnregisterServer();
    }
}
