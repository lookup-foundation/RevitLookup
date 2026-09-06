using System.Windows;
using System.Windows.Automation.Peers;

namespace RevitLookup.UI.Framework.Controls.Automation;

/// <summary>
///     Represents a window automation peer that reports no children, disabling UI Automation for the window.
/// </summary>
/// <remarks>
///     Works around a freeze that occurs when a <see cref="System.Windows.Controls.ToolTip" /> or <see cref="System.Windows.Controls.Primitives.Popup" /> is used together with UI Automation;
///     see <see href="https://github.com/dotnet/wpf/issues/5807">dotnet/wpf#5807</see>.
/// </remarks>
public sealed class NoAutomationWindowPeer(Window owner) : WindowAutomationPeer(owner)
{
    /// <inheritdoc />
    protected override List<AutomationPeer> GetChildrenCore()
    {
        return [];
    }
}
