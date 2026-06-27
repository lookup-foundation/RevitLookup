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

using System.Reflection;
using System.Windows;
using RevitLookup.UI.Framework.Utils;
using Wpf.Ui.Controls;
using DataGrid = Wpf.Ui.Controls.DataGrid;

namespace RevitLookup.UI.Framework.Views.Decomposition;

public partial class SummaryViewBase
{
    private static readonly FieldInfo InternalGridScrollHostField =
        typeof(System.Windows.Controls.DataGrid).GetField("_internalScrollHost",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)!;

    private static readonly PropertyInfo InternalGridColumnsProperty =
        typeof(System.Windows.Controls.DataGrid).GetProperty("InternalColumns",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)!;

    private static readonly MethodInfo InternalGridInvalidateColumnWidthsComputationMethod =
        InternalGridColumnsProperty.PropertyType.GetMethod("InvalidateColumnWidthsComputation",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;

    private static readonly MethodInfo InternalGridOnViewportSizeChangedMethod =
        typeof(System.Windows.Controls.DataGrid).GetMethod("OnViewportSizeChanged",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;

    /// <summary>
    ///      By default, WPF calculates the column width after adding items to the ItemSource. This fix calculates it on loading
    /// </summary>
    /// <remarks>
    ///     https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/DataGrid.cs#L98
    /// </remarks>
    private static void FixInitialGridColumnSize(object sender, RoutedEventArgs args)
    {
        var dataGrid = (DataGrid) sender;
        var passiveScrollViewer = dataGrid.FindVisualChild<PassiveScrollViewer>();
        if (passiveScrollViewer is null)
        {
            dataGrid.ApplyTemplate();
            passiveScrollViewer = dataGrid.FindVisualChild<PassiveScrollViewer>()!;
        }

        var gridColumns = InternalGridColumnsProperty.GetValue(dataGrid);
        InternalGridScrollHostField.SetValue(dataGrid, passiveScrollViewer);
        InternalGridInvalidateColumnWidthsComputationMethod.Invoke(gridColumns, null);

        passiveScrollViewer.SizeChanged += FixCanContentScrollResizing;
    }

    /// <summary>
    ///      By default, WPF doesn't recalculate column widths if ScrollViewer.CanContentScroll is enabled
    /// </summary>
    /// <remarks>
    ///     https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/DataGrid.cs#L1961-L1968
    /// </remarks>
    private static void FixCanContentScrollResizing(object sender, SizeChangedEventArgs e)
    {
        var scrollViewer = (PassiveScrollViewer) sender;
        var dataGrid = scrollViewer.FindVisualParent<System.Windows.Controls.DataGrid>(); //find parent to avoid closure allocations
        InternalGridOnViewportSizeChangedMethod.Invoke(dataGrid, [e.PreviousSize, e.NewSize]);
    }
}