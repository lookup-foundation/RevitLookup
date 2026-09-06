using System.Windows;
using System.Windows.Controls;
using LookupEngine.Abstractions.Configuration;
using RevitLookup.Abstractions.Decomposition;

namespace RevitLookup.Views.Styles.MembersGrid;

/// <summary>
///     Represents a style selector that chooses the data grid row style for a decomposed member.
/// </summary>
public sealed class DataGridRowStyleSelector : StyleSelector
{
    /// <inheritdoc />
    public override Style? SelectStyle(object item, DependencyObject container)
    {
        var member = (ObservableDecomposedMember)item;
        var presenter = (FrameworkElement)container;

        var styleName = member.Value.Descriptor switch
        {
            IDescriptorEnumerator { IsEmpty: false } => "HandledDataGridRowStyle",
            IDescriptorEnumerator => "DefaultLookupDataGridRowStyle",
            IDescriptorCollector => "HandledDataGridRowStyle",
            _ => "DefaultLookupDataGridRowStyle"
        };

        return (Style)presenter.FindResource(styleName);
    }
}
