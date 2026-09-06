using System.Windows.Input;
using Wpf.Ui.Controls;

namespace RevitLookup.Abstractions.Dashboard;

/// <summary>
///     Represents a navigation card shown on the dashboard.
/// </summary>
public sealed class NavigationCardItem
{
    /// <summary>
    ///     Gets or sets the card title.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    ///     Gets or sets the card description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Gets or sets the card icon.
    /// </summary>
    public required SymbolRegular Icon { get; set; }

    /// <summary>
    ///     Gets or sets the command to execute when the card is clicked.
    /// </summary>
    public required ICommand Command { get; set; }

    /// <summary>
    ///     Gets or sets the parameter to pass to <see cref="Command" />.
    /// </summary>
    public object? CommandParameter { get; set; }
}
