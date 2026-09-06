using System.Windows;
using System.Windows.Controls;
using LookupEngine.Abstractions.Configuration;
using RevitLookup.Abstractions.Decomposition;

namespace RevitLookup.UI.Playground.Mocks.Styles.MembersGrid;

/// <summary>
///     Selects the members grid row style for a decomposed member, based on whether its descriptor has already been handled.
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
