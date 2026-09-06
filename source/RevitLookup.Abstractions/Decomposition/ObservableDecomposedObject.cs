using CommunityToolkit.Mvvm.ComponentModel;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Abstractions.Decomposition;

/// <summary>
///     Represents the observable model for the LookupEngine decomposed object.
/// </summary>
public sealed partial class ObservableDecomposedObject : ObservableObject
{
    /// <summary>
    ///     Gets or sets the underlying CLR object this model represents.
    /// </summary>
    public required object? RawValue { get; init; }

    /// <summary>
    ///     Gets or sets the object's display name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets the simple name of the object's type.
    /// </summary>
    public required string TypeName { get; set; }

    /// <summary>
    ///     Gets or sets the fully qualified name of the object's type.
    /// </summary>
    public required string TypeFullName { get; set; }

    /// <summary>
    ///     Gets or sets the object's description, or <see langword="null" /> when none is available.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Gets or sets the engine descriptor used to decompose this object, or <see langword="null" /> when none applies.
    /// </summary>
    public Descriptor? Descriptor { get; init; }

    /// <summary>
    ///     Gets or sets the object's decomposed members.
    /// </summary>
    [ObservableProperty]
    public partial List<ObservableDecomposedMember> Members { get; set; } = [];

    /// <summary>
    ///     Gets or sets the members that remain after the current filter is applied.
    /// </summary>
    [ObservableProperty]
    public partial List<ObservableDecomposedMember> FilteredMembers { get; set; } = [];

    partial void OnMembersChanged(List<ObservableDecomposedMember> value)
    {
        FilteredMembers = value;
    }
}
