using CommunityToolkit.Mvvm.ComponentModel;
using LookupEngine.Abstractions;
using LookupEngine.Abstractions.Enums;

namespace RevitLookup.Abstractions.ObservableModels.Decomposition;

/// <summary>
///     Represents the observable model for the LookupEngine decomposed member.
/// </summary>
public sealed partial class ObservableDecomposedMember : ObservableObject
{
    public required int Depth { get; set; }
    public required string Name { get; set; }
    public required string DeclaringTypeName { get; set; }
    public required string DeclaringTypeFullName { get; set; }
    public MemberAttributes MemberAttributes { get; set; }
    public required ObservableDecomposedValue Value { get; set; }

    [ObservableProperty]
    public partial double ComputationTime { get; set; }

    [ObservableProperty]
    public partial long AllocatedBytes { get; set; }

    [ObservableProperty]
    public partial MemberEvaluationPolicy EvaluationPolicy { get; set; }

    /// <summary>
    ///     The engine origin used to force the evaluation of a deferred member.
    /// </summary>
    public DecomposedMember? Member { get; set; }
}