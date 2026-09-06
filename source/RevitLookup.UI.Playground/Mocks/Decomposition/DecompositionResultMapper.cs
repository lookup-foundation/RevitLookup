using LookupEngine.Abstractions;
using RevitLookup.Abstractions.Decomposition;
using Riok.Mapperly.Abstractions;

namespace RevitLookup.UI.Playground.Mocks.Decomposition;

/// <summary>
///     Provides Mapperly-generated conversions from LookupEngine decomposition results to their observable Playground counterparts.
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Source)]
public static partial class DecompositionResultMapper
{
    /// <summary>
    ///     Converts a decomposed object into its observable equivalent.
    /// </summary>
    /// <param name="decomposedObject">The decomposed object to convert.</param>
    /// <returns>The observable equivalent of <paramref name="decomposedObject" />.</returns>
    public static partial ObservableDecomposedObject Convert(DecomposedObject decomposedObject);

    /// <summary>
    ///     Converts a decomposed value into its observable equivalent.
    /// </summary>
    /// <param name="decomposedValue">The decomposed value to convert.</param>
    /// <returns>The observable equivalent of <paramref name="decomposedValue" />.</returns>
    public static partial ObservableDecomposedValue Convert(DecomposedValue decomposedValue);

    /// <summary>
    ///     Converts a decomposed member into its observable equivalent.
    /// </summary>
    /// <param name="decomposedMember">The decomposed member to convert.</param>
    /// <returns>The observable equivalent of <paramref name="decomposedMember" />, with its <see cref="ObservableDecomposedMember.Member" /> set to <paramref name="decomposedMember" />.</returns>
    [UserMapping(Default = true)]
    public static ObservableDecomposedMember Convert(DecomposedMember decomposedMember)
    {
        var member = MapMember(decomposedMember);
        member.Member = decomposedMember;
        return member;
    }

    [MapperIgnoreSource(nameof(DecomposedMember.Evaluator))]
    private static partial ObservableDecomposedMember MapMember(DecomposedMember decomposedMember);

    /// <summary>
    ///     Copies the mapped fields of a decomposed member onto an existing observable member.
    /// </summary>
    /// <param name="source">The decomposed member holding the updated values.</param>
    /// <param name="target">The observable member to update in place.</param>
    [MapperIgnoreSource(nameof(DecomposedMember.Evaluator))]
    public static partial void Update(DecomposedMember source, ObservableDecomposedMember target);
}
