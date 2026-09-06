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

using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.Settings;
using RevitLookup.Abstractions.ViewModels.Decomposition;

namespace RevitLookup.UI.Framework.Views.Decomposition;

/// <summary>
///     Represents a page that shows the decomposed members of a snooped object in a searchable tree and grid.
/// </summary>
public sealed partial class DecompositionSummaryPage
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DecompositionSummaryPage" /> class.
    /// </summary>
    /// <param name="serviceProvider">The container used to resolve dependencies for descriptor context-menu commands.</param>
    /// <param name="viewModel">The view model that provides the data for the Decomposition Summary view.</param>
    /// <param name="settingsService">The service that provides the decomposition display settings.</param>
    /// <param name="intercomService">The service that gives access to the hosting window.</param>
    /// <param name="notificationService">The service used to notify the user of errors encountered while refreshing members.</param>
    /// <param name="themeWatcherService">The service that applies and tracks the current theme for this page.</param>
    /// <param name="loggerFactory">The factory used to create the logger for this page.</param>
    public DecompositionSummaryPage(
        IServiceProvider serviceProvider,
        IDecompositionSummaryViewModel viewModel,
        ISettingsService settingsService,
        IWindowIntercomService intercomService,
        INotificationService notificationService,
        IThemeWatcherService themeWatcherService,
        ILoggerFactory loggerFactory)
        : base(serviceProvider, settingsService, intercomService, notificationService, loggerFactory)
    {
        themeWatcherService.Watch(this);

        DataContext = this;
        ViewModel = viewModel;
        InitializeComponent();

        SearchBoxControl = SummarySearchBox;
        TreeViewControl = SummaryTreeView;
        DataGridControl = SummaryDataGrid;
        InitializeControls();
    }
}
