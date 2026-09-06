using System.Windows;
using System.Windows.Controls;
using RevitLookup.Abstractions.Decomposition;

namespace RevitLookup.Views.Styles.ObjectsTree;

/// <summary>
///     Represents a template selector that chooses the tree view item template for a decomposed object.
/// </summary>
public sealed class TreeViewItemTemplateSelector : DataTemplateSelector
{
    /// <inheritdoc />
    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (item is null)
        {
            return null;
        }

        var presenter = (FrameworkElement)container;
        var decomposedObject = (ObservableDecomposedObject)item;
        var templateName = decomposedObject.RawValue switch
        {
            Color { IsValid: true } => "SummaryMediaColorItemTemplate",
            System.Windows.Media.Color => "SummaryMediaColorItemTemplate",
            _ => "DefaultSummaryTreeItemTemplate"
        };

        return (DataTemplate)presenter.FindResource(templateName);
    }
}
