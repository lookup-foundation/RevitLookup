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
using System.Windows.Controls;
using System.Windows.Input;
using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.Tools;
using RevitLookup.Abstractions.ViewModels.Tools;
using RevitLookup.UI.Framework.Menus;
using RevitLookup.UI.Framework.Views.Decomposition;
using Wpf.Ui;
using Visibility = System.Windows.Visibility;

namespace RevitLookup.UI.Framework.Views.Tools;

/// <summary>
///     Represents a dialog that lists the Revit built-in parameters, built-in categories, or forge schema units.
/// </summary>
public sealed partial class UnitsDialog
{
    private readonly INavigationService _navigationService;
    private readonly IUnitsViewModel _viewModel;

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnitsDialog" /> class.
    /// </summary>
    /// <param name="dialogService">The service that supplies the dialog host this dialog is shown on.</param>
    /// <param name="viewModel">The view model that provides the data for the Units view.</param>
    /// <param name="navigationService">The service used to navigate to the decomposition summary when an entry is explored.</param>
    /// <param name="themeWatcherService">The service that applies and tracks the current theme for this dialog.</param>
    public UnitsDialog(
        IContentDialogService dialogService,
        IUnitsViewModel viewModel,
        INavigationService navigationService,
        IThemeWatcherService themeWatcherService)
        : base(dialogService.GetDialogHostEx())
    {
        _viewModel = viewModel;
        _navigationService = navigationService;

        DataContext = _viewModel;
        InitializeComponent();

        themeWatcherService.Watch(this);
    }

    /// <summary>
    ///     Shows the dialog listing the Revit built-in parameters.
    /// </summary>
    /// <returns>A task that represents the asynchronous show operation.</returns>
    public async Task ShowParametersDialogAsync()
    {
        _viewModel.InitializeParameters();

        Title = "BuiltIn Parameters";
        DialogMaxWidth = 1000;

        await ShowAsync();
    }

    /// <summary>
    ///     Shows the dialog listing the Revit built-in categories.
    /// </summary>
    /// <returns>A task that represents the asynchronous show operation.</returns>
    public async Task ShowCategoriesDialogAsync()
    {
        _viewModel.InitializeCategories();

        Title = "BuiltIn Categories";
        DialogMaxWidth = 600;

        await ShowAsync();
    }

    /// <summary>
    ///     Shows the dialog listing the forge schema units.
    /// </summary>
    /// <returns>A task that represents the asynchronous show operation.</returns>
    public async Task ShowForgeSchemaDialogAsync()
    {
        _viewModel.InitializeForgeSchema();

        ClassColumn.Visibility = Visibility.Visible;
        Title = "Forge Schema";
        DialogMaxWidth = 1100;

        await ShowAsync();
    }

    private void OnMouseEnter(object sender, RoutedEventArgs routedEventArgs)
    {
        var element = (FrameworkElement)sender;
        var unitInfo = (UnitInfo)element.DataContext;
        CreateRowContextMenu(unitInfo, element);
    }

    private void CreateRowContextMenu(UnitInfo info, FrameworkElement row)
    {
        var contextMenu = new ContextMenu
        {
            Resources = UiApplication.Current.Resources,
            PlacementTarget = row
        };

        var copyMenu = contextMenu.AddMenuItem("CopyMenuItem")
            .SetHeader("Copy");

        copyMenu.AddMenuItem()
            .SetHeader("Unit identifier")
            .SetCommand(info, static unitInfo => Clipboard.SetDataObject(unitInfo.Unit))
            .SetShortcut(ModifierKeys.Control, Key.C);

        copyMenu.AddMenuItem()
            .SetHeader("Display label")
            .SetCommand(info, unitInfo => Clipboard.SetDataObject(unitInfo.Label));

        if (info.Class is not null)
        {
            copyMenu.AddMenuItem()
                .SetHeader("Type class")
                .SetCommand(info, unitInfo => Clipboard.SetDataObject(unitInfo.Class!))
                .SetShortcut(ModifierKeys.Control | ModifierKeys.Shift, Key.C);
        }

        contextMenu.AddMenuItem("SnoopMenuItem")
            .SetHeader("Explore members")
            .SetCommand(info, async unitInfo =>
            {
                Hide();
                await _viewModel.DecomposeAsync(unitInfo);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
            });

        row.ContextMenu = contextMenu;
    }
}
