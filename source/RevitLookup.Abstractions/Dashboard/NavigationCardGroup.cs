namespace RevitLookup.Abstractions.Dashboard;

/// <summary>
///     Represents a named group of navigation cards.
/// </summary>
public sealed class NavigationCardGroup
{
    /// <summary>
    ///     Gets or sets the group name.
    /// </summary>
    public required string GroupName { get; set; }

    /// <summary>
    ///     Gets or sets the navigation card items in the group.
    /// </summary>
    public required List<NavigationCardItem> Items { get; set; }
}
