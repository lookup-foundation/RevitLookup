using LookupEngine.Abstractions;
using RevitLookup.Abstractions.Decomposition;
using Riok.Mapperly.Abstractions;

namespace RevitLookup.Decomposition;

/// <summary>
///     Provides mapping between LookupEngine decomposition results and their observable UI models.
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Source)]
public static partial class DecompositionResultMapper
{
    /// <summary>
    ///     Converts a <see cref="DecomposedObject" /> to its observable representation.
    /// </summary>
    /// <param name="decomposedObject">The decomposed object to convert.</param>
    /// <returns>The observable representation of <paramref name="decomposedObject" />.</returns>
    public static partial ObservableDecomposedObject Convert(DecomposedObject decomposedObject);

    /// <summary>
    ///     Converts a <see cref="DecomposedValue" /> to its observable representation.
    /// </summary>
    /// <param name="decomposedValue">The decomposed value to convert.</param>
    /// <returns>The observable representation of <paramref name="decomposedValue" />.</returns>
    public static partial ObservableDecomposedValue Convert(DecomposedValue decomposedValue);

    /// <summary>
    ///     Converts a <see cref="DecomposedMember" /> to its observable representation.
    /// </summary>
    /// <param name="decomposedMember">The decomposed member to convert.</param>
    /// <returns>The observable representation of <paramref name="decomposedMember" />, with <see cref="ObservableDecomposedMember.Member" /> set to <paramref name="decomposedMember" />.</returns>
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
    ///     Copies the evaluated state from <paramref name="source" /> onto <paramref name="target" />.
    /// </summary>
    /// <param name="source">The decomposed member holding the newly evaluated result.</param>
    /// <param name="target">The observable member to update in place.</param>
    [MapperIgnoreSource(nameof(DecomposedMember.Evaluator))]
    public static partial void Update(DecomposedMember source, ObservableDecomposedMember target);
}
