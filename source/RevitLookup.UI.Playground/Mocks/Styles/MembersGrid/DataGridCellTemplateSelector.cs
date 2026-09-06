using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RevitLookup.Abstractions.Decomposition;

namespace RevitLookup.UI.Playground.Mocks.Styles.MembersGrid;

/// <summary>
///     Selects the members grid cell template for a decomposed member, based on its value kind.
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
                Color => "SummaryMediaColorCellTemplate",
                Exception => "ExceptionSummaryCellTemplate",
                _ => "DefaultSummaryCellTemplate"
            };
        }

        return (DataTemplate)presenter.FindResource(templateName);
    }
}
