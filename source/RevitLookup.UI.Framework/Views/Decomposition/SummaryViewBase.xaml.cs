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

using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.ObservableModels.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.Services.Settings;
using RevitLookup.Abstractions.ViewModels.Decomposition;
using RevitLookup.UI.Framework.Utils;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;
using DataGrid = Wpf.Ui.Controls.DataGrid;
using TreeView = Wpf.Ui.Controls.TreeView;
using TreeViewItem = System.Windows.Controls.TreeViewItem;
using Visibility = System.Windows.Visibility;

namespace RevitLookup.UI.Framework.Views.Decomposition;

public partial class SummaryViewBase : Page, INavigableView<ISummaryViewModel>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISettingsService _settingsService;
    private readonly IWindowIntercomService _intercomService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<SummaryViewBase> _logger;

    protected SummaryViewBase(
        IServiceProvider serviceProvider,
        ISettingsService settingsService,
        IWindowIntercomService intercomService,
        INotificationService notificationService,
        ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _settingsService = settingsService;
        _intercomService = intercomService;
        _notificationService = notificationService;
        _logger = loggerFactory.CreateLogger<SummaryViewBase>();
    }

    public required UIElement SearchBoxControl { get; init; }
    public required TreeView TreeViewControl { get; init; }
    public required DataGrid DataGridControl { get; init; }
    public required ISummaryViewModel ViewModel { get; init; }

    protected void InitializeControls()
    {
        InitializeTreeView(TreeViewControl);
        InitializeDataGrid(DataGridControl);
    }

    /// <summary>
    ///     Tree view initialization
    /// </summary>
    private void InitializeTreeView(TreeView control)
    {
        control.SelectedItemChanged += OnTreeItemSelected;
        control.ItemsSourceChanged += OnTreeSourceChanged;
        control.MouseMove += OnPresenterCursorInteracted;
        control.ItemContainerGenerator.StatusChanged += OnTreeViewItemGenerated;

        if (control.ItemsSource is not null) OnTreeSourceChanged(control, control.ItemsSource);
    }

    /// <summary>
    ///     Tree view source changed handled. Setup action after the setting source
    /// </summary>
    private static void OnTreeSourceChanged(object? sender, IEnumerable enumerable)
    {
        var treeView = (TreeView) sender!;

        if (treeView.IsLoaded)
        {
            ExpandFirstTreeGroup(treeView);
            return;
        }

        treeView.Loaded += OnLoaded;
        return;

        void OnLoaded(object nestedSender, RoutedEventArgs args)
        {
            var self = (TreeView) nestedSender;
            self.Loaded -= OnLoaded;
            ExpandFirstTreeGroup(treeView);
        }
    }

    /// <summary>
    ///     Expand the first tree view group after setting source
    /// </summary>
    /// <param name="treeView"></param>
    private static async void ExpandFirstTreeGroup(TreeView treeView)
    {
        try
        {
            // Await Frame transition. GetMembers freezes the thread and breaks the animation
            var transitionDuration = (int) NavigationView.TransitionDurationProperty.DefaultMetadata.DefaultValue;
            await Task.Delay(transitionDuration);

            //3 is optimal groups count for expanding
            if (treeView.Items.Count > 3) return;

            var rootItem = (TreeViewItem?) treeView.GetItemAtIndex(0);
            if (rootItem is null) return;

            var nestedItem = (TreeViewItem?) rootItem.GetItemAtIndex(0);
            if (nestedItem is null) return;

            nestedItem.IsSelected = true;
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    ///     Handle tree view item loaded
    /// </summary>
    /// <remarks>
    ///     TreeView item customization after loading
    /// </remarks>
    private void OnTreeViewItemGenerated(object? sender, EventArgs _)
    {
        var generator = (ItemContainerGenerator) sender!;
        if (generator.Status == GeneratorStatus.ContainersGenerated)
        {
            foreach (var item in generator.Items)
            {
                var treeItem = (ItemsControl) generator.ContainerFromItem(item);
                if (treeItem is null) continue;

                treeItem.MouseEnter -= OnTreeItemCaptured;
                treeItem.PreviewMouseLeftButtonUp -= OnTreeItemClicked;

                treeItem.MouseEnter += OnTreeItemCaptured;
                treeItem.PreviewMouseLeftButtonUp += OnTreeItemClicked;

                if (treeItem.Items.Count > 0)
                {
                    treeItem.ItemContainerGenerator.StatusChanged -= OnTreeViewItemGenerated;
                    treeItem.ItemContainerGenerator.StatusChanged += OnTreeViewItemGenerated;
                }
            }
        }
    }

    /// <summary>
    ///     Create tree view tooltips, menus
    /// </summary>
    private void OnTreeItemCaptured(object? sender, RoutedEventArgs args)
    {
        var element = (FrameworkElement) sender!;
        switch (element.DataContext)
        {
            case ObservableDecomposedObjectsGroup decomposedGroup:
                CreateTreeTooltip(decomposedGroup, element);
                break;
            case ObservableDecomposedObject decomposedObject:
                CreateTreeTooltip(decomposedObject, element);
                CreateTreeContextMenu(decomposedObject, element);
                break;
        }
    }

    /// <summary>
    ///     Handle data grid reference changed event
    /// </summary>
    /// <remarks>
    ///     Data grid initialization, validation
    /// </remarks>
    private void InitializeDataGrid(DataGrid dataGrid)
    {
        ApplyGrouping(dataGrid);
        ValidateTimeColumn(dataGrid);
        ValidateAllocatedColumn(dataGrid);
        CreateGridContextMenu(dataGrid);
        dataGrid.LoadingRow += OnGridRowLoading;
        dataGrid.MouseMove += OnPresenterCursorInteracted;
        dataGrid.ItemsSourceChanged += ApplySorting;
        dataGrid.Loaded += FixInitialGridColumnSize;
    }

    /// <summary>
    ///     Set DataGrid grouping rules
    /// </summary>
    private void ApplyGrouping(DataGrid dataGrid)
    {
        dataGrid.Items.GroupDescriptions!.Clear();
        dataGrid.Items.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ObservableDecomposedMember.DeclaringTypeName)));
    }

    /// <summary>
    ///     Set DataGrid sorting rules
    /// </summary>
    private static void ApplySorting(object? sender, EventArgs eventArgs)
    {
        var dataGrid = (DataGrid) sender!;

        dataGrid.Items.SortDescriptions.Add(new SortDescription(nameof(ObservableDecomposedMember.Depth), ListSortDirection.Descending));
        dataGrid.Items.SortDescriptions.Add(new SortDescription(nameof(ObservableDecomposedMember.MemberAttributes), ListSortDirection.Ascending));
        dataGrid.Items.SortDescriptions.Add(new SortDescription(nameof(ObservableDecomposedMember.Name), ListSortDirection.Ascending));
    }

    // <summary>
    //     Handle data grid row loading event
    // </summary>
    // <remarks>
    //     Select row style
    // </remarks>
    private void OnGridRowLoading(object? sender, DataGridRowEventArgs args)
    {
        var row = args.Row;

        row.MouseEnter -= OnGridRowCaptured;
        row.PreviewMouseLeftButtonUp -= OnGridRowClicked;

        row.MouseEnter += OnGridRowCaptured;
        row.PreviewMouseLeftButtonUp += OnGridRowClicked;

        MonitorRowValueChanges(row);
    }

    /// <summary>
    ///     Handle data grid row loaded event
    /// </summary>
    /// <remarks>
    ///     Create tooltips, context menu
    /// </remarks>
    private void OnGridRowCaptured(object sender, RoutedEventArgs args)
    {
        var element = (FrameworkElement) sender;
        var member = (ObservableDecomposedMember) element.DataContext;
        CreateGridRowTooltip(member, element);
        CreateGridRowContextMenu(member, element);
    }

    /// <summary>
    ///     Show/hide time column
    /// </summary>
    private void ValidateTimeColumn(DataGrid control)
    {
        control.Columns[2].Visibility = _settingsService.DecompositionSettings.ShowTimeColumn ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    ///     Show/hide allocated column
    /// </summary>
    private void ValidateAllocatedColumn(DataGrid control)
    {
        control.Columns[3].Visibility = _settingsService.DecompositionSettings.ShowMemoryColumn ? Visibility.Visible : Visibility.Collapsed;
    }
}