using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RevitLookup.Abstractions.Decomposition;

namespace RevitLookup.UI.Playground.Mocks.Styles.ObjectsTree;

/// <summary>
///     Selects the objects-tree item template for a decomposed object, giving a WPF media color its own swatch template.
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
            Color => "SummaryMediaColorItemTemplate",
            _ => "DefaultSummaryTreeItemTemplate"
        };

        return (DataTemplate)presenter.FindResource(templateName);
    }
}
