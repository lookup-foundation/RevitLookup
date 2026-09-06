using System.Windows;
using System.Windows.Controls;
using RevitLookup.Abstractions.Decomposition;

namespace RevitLookup.Views.Styles.MembersGrid;

/// <summary>
///     Represents a template selector that chooses the data grid cell template for a decomposed member's value.
/// </summary>
public sealed class DataGridCellTemplateSelector : DataTemplateSelector
{
    /// <inheritdoc />
    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (item is null)
        {
            return null;
        }

        var member = (ObservableDecomposedMember)item;
        var presenter = (FrameworkElement)container;

        string templateName;
        if (member.Value.TypeFullName == "System.Void")
        {
            templateName = "VoidSummaryCellTemplate";
        }
        else
        {
            templateName = member.Value.RawValue switch
            {
                null => "NullSummaryCellTemplate",
                string { Length: 0 } => "InvalidSummaryCellTemplate",
                Color { IsValid: true } => "SummaryMediaColorCellTemplate",
                System.Windows.Media.Color => "SummaryMediaColorCellTemplate",
                Exception => "ExceptionSummaryCellTemplate",
                _ => "DefaultSummaryCellTemplate"
            };
        }

        return (DataTemplate)presenter.FindResource(templateName);
    }
}
