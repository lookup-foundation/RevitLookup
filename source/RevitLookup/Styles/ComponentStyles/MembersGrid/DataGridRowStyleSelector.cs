using System.Windows;
using System.Windows.Controls;
using LookupEngine.Abstractions.Configuration;
using RevitLookup.Abstractions.ObservableModels.Decomposition;

namespace RevitLookup.Styles.ComponentStyles.MembersGrid;

/// <summary>
///     Data grid row style selector
/// </summary>
public sealed class DataGridRowStyleSelector : StyleSelector
{
    public override Style? SelectStyle(object item, DependencyObject container)
    {
        var member = (ObservableDecomposedMember) item;
        var presenter = (FrameworkElement) container;

        var styleName = member.Value.Descriptor switch
        {
            IDescriptorEnumerator {IsEmpty: false} => "HandledDataGridRowStyle",
            IDescriptorEnumerator => "DefaultLookupDataGridRowStyle",
            IDescriptorCollector => "HandledDataGridRowStyle",
            _ => "DefaultLookupDataGridRowStyle"
        };

        return (Style) presenter.FindResource(styleName);
    }
}