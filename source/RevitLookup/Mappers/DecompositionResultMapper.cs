using LookupEngine.Abstractions;
using RevitLookup.Abstractions.ObservableModels.Decomposition;
using Riok.Mapperly.Abstractions;

namespace RevitLookup.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Source)]
public static partial class DecompositionResultMapper
{
    public static partial ObservableDecomposedObject Convert(DecomposedObject decomposedObject);
    public static partial ObservableDecomposedValue Convert(DecomposedValue decomposedValue);

    [UserMapping(Default = true)]
    public static ObservableDecomposedMember Convert(DecomposedMember decomposedMember)
    {
        var member = MapMember(decomposedMember);
        member.Member = decomposedMember;
        return member;
    }

    [MapperIgnoreSource(nameof(DecomposedMember.Evaluator))]
    private static partial ObservableDecomposedMember MapMember(DecomposedMember decomposedMember);
}