using CommunityToolkit.Mvvm.ComponentModel;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Abstractions.ObservableModels.Decomposition;

/// <summary>
///     Represents the observable model for the LookupEngine decomposed object.
/// </summary>
public sealed partial class ObservableDecomposedObject : ObservableObject
{
    public required object? RawValue { get; init; }
    public required string Name { get; set; }
    public required string TypeName { get; set; }
    public required string TypeFullName { get; set; }
    public string? Description { get; set; }
    public Descriptor? Descriptor { get; init; }
    
    [ObservableProperty]
    public partial List<ObservableDecomposedMember> Members { get; set; } = [];

    [ObservableProperty]
    public partial List<ObservableDecomposedMember> FilteredMembers { get; set; } = [];

    partial void OnMembersChanged(List<ObservableDecomposedMember> value)
    {
        FilteredMembers = value;
    }
}