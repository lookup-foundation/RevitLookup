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

using System.ComponentModel;
using System.Windows.Controls;
using RevitLookup.Abstractions.ObservableModels.Decomposition;

namespace RevitLookup.UI.Framework.Views.Decomposition;

public partial class SummaryViewBase
{
    /// <summary>
    ///     Track a row member so its style can be refreshed after the member is force evaluated.
    /// </summary>
    /// <remarks>
    ///     The row style is chosen once by the style selector.
    /// </remarks>
    private void MonitorRowValueChanges(DataGridRow row)
    {
        if (row.Item is not ObservableDecomposedMember member) return;

        member.PropertyChanged -= OnRowMemberEvaluated;
        member.PropertyChanged += OnRowMemberEvaluated;
    }

    /// <summary>
    ///     Refresh the row after a member is (re-)evaluated: re-apply the row style (an exception turns the row red) and re-render the value cell.
    /// </summary>
    private void OnRowMemberEvaluated(object? sender, PropertyChangedEventArgs args)
    {
        if (sender is not ObservableDecomposedMember member) return;
        if (args.PropertyName != nameof(ObservableDecomposedMember.Value) && args.PropertyName != nameof(ObservableDecomposedMember.EvaluationPolicy)) return;
        if (DataGridControl.ItemContainerGenerator.ContainerFromItem(member) is not DataGridRow row) return;

        if (DataGridControl.RowStyleSelector is { } styleSelector)
        {
            row.Style = styleSelector.SelectStyle(member, row);
        }

        UpdateValueCellTemplate(row);
    }

    /// <summary>
    ///     Force the value cell to update its template selector.
    /// </summary>
    private void UpdateValueCellTemplate(DataGridRow row)
    {
        const int valueColumnIndex = 1;
        if (DataGridControl.Columns.Count <= valueColumnIndex) return;
        if (DataGridControl.Columns[valueColumnIndex].GetCellContent(row) is not ContentControl content) return;

        var selector = content.ContentTemplateSelector;
        content.ContentTemplateSelector = null;
        content.ContentTemplateSelector = selector;
    }
}