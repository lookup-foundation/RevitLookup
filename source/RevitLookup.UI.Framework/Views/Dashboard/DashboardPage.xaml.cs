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
using RevitLookup.Abstractions.ViewModels.Dashboard;
using Wpf.Ui.Abstractions.Controls;

namespace RevitLookup.UI.Framework.Views.Dashboard;

/// <summary>
///     Represents a page that shows the entry points to the application tools, grouped by category.
/// </summary>
public sealed partial class DashboardPage : INavigableView<IDashboardViewModel>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DashboardPage" /> class.
    /// </summary>
    /// <param name="viewModel">The view model that provides the data for the Dashboard view.</param>
    /// <param name="themeWatcherService">The service that applies and tracks the current theme for this page.</param>
    public DashboardPage(IDashboardViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        themeWatcherService.Watch(this);

        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    /// <inheritdoc />
    public IDashboardViewModel ViewModel { get; }
}
