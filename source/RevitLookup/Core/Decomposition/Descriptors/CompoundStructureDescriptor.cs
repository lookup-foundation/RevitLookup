// Copyright (c) Lookup Foundation and Contributors
// 
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
// 
// THIS PROGRAM IS PROVIDED "AS IS" AND WITH ALL FAULTS.
// NO IMPLIED WARRANTY OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE IS PROVIDED.
// THERE IS NO GUARANTEE THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.

using System.Reflection;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class CompoundStructureDescriptor(CompoundStructure compoundStructure) : Descriptor, IDescriptorResolver
{
    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(CompoundStructure.CanLayerBeStructuralMaterial) => () => VariantsResolver.ResolveIndex(compoundStructure.LayerCount, compoundStructure.CanLayerBeStructuralMaterial),
            nameof(CompoundStructure.CanLayerBeVariable) => () => VariantsResolver.ResolveIndex(compoundStructure.LayerCount, compoundStructure.CanLayerBeVariable),
            nameof(CompoundStructure.CanLayerWidthBeNonZero) => () => VariantsResolver.ResolveIndex(compoundStructure.LayerCount, compoundStructure.CanLayerWidthBeNonZero),
            nameof(CompoundStructure.GetAdjacentRegions) => () => VariantsResolver.ResolveIndex(compoundStructure.GetRegionIds().Count, compoundStructure.GetAdjacentRegions),
            nameof(CompoundStructure.GetCoreBoundaryLayerIndex) => () => VariantsResolver.ResolveEnum<ShellLayerType, int>(compoundStructure.GetCoreBoundaryLayerIndex),
            nameof(CompoundStructure.GetDeckEmbeddingType) => () => VariantsResolver.ResolveIndex(compoundStructure.LayerCount, compoundStructure.GetDeckEmbeddingType),
            nameof(CompoundStructure.GetDeckProfileId) => () => VariantsResolver.ResolveIndex(compoundStructure.LayerCount, compoundStructure.GetDeckProfileId),
            nameof(CompoundStructure.GetLayerAssociatedToRegion) => () => VariantsResolver.ResolveIndex(compoundStructure.GetRegionIds().Count, compoundStructure.GetLayerAssociatedToRegion),
            nameof(CompoundStructure.GetLayerFunction) => () => VariantsResolver.ResolveIndex(compoundStructure.LayerCount, compoundStructure.GetLayerFunction),
            nameof(CompoundStructure.GetLayerWidth) => () => VariantsResolver.ResolveIndex(compoundStructure.LayerCount, compoundStructure.GetLayerWidth),
            nameof(CompoundStructure.GetMaterialId) => () => VariantsResolver.ResolveIndex(compoundStructure.LayerCount, compoundStructure.GetMaterialId),
            nameof(CompoundStructure.GetNumberOfShellLayers) => () => VariantsResolver.ResolveEnum<ShellLayerType, int>(compoundStructure.GetNumberOfShellLayers),
            nameof(CompoundStructure.GetOffsetForLocationLine) => () => VariantsResolver.ResolveEnum<WallLocationLine, double>(compoundStructure.GetOffsetForLocationLine),
            nameof(CompoundStructure.GetPreviousNonZeroLayerIndex) => () => VariantsResolver.ResolveIndex(compoundStructure.LayerCount, compoundStructure.GetPreviousNonZeroLayerIndex),
            nameof(CompoundStructure.GetRegionEnvelope) => () => VariantsResolver.ResolveIndex(compoundStructure.GetRegionIds().Count, compoundStructure.GetRegionEnvelope),
            nameof(CompoundStructure.GetRegionsAssociatedToLayer) => () => VariantsResolver.ResolveIndex(compoundStructure.LayerCount, compoundStructure.GetRegionsAssociatedToLayer),
            nameof(CompoundStructure.GetSegmentCoordinate) => () => VariantsResolver.ResolveIndex(compoundStructure.GetSegmentIds().Count, compoundStructure.GetSegmentCoordinate),
            nameof(CompoundStructure.GetSegmentOrientation) => () => VariantsResolver.ResolveIndex(compoundStructure.GetSegmentIds().Count, compoundStructure.GetSegmentOrientation),
            nameof(CompoundStructure.GetWallSweepsInfo) => () => VariantsResolver.ResolveEnum<WallSweepType, IList<WallSweepInfo>>(compoundStructure.GetWallSweepsInfo),
            nameof(CompoundStructure.GetWidth) when parameters.Length == 1 => () => VariantsResolver.ResolveIndex(compoundStructure.GetRegionIds().Count, compoundStructure.GetWidth),
            nameof(CompoundStructure.IsCoreLayer) => () => VariantsResolver.ResolveIndex(compoundStructure.LayerCount, compoundStructure.IsCoreLayer),
            nameof(CompoundStructure.IsRectangularRegion) => () => VariantsResolver.ResolveIndex(compoundStructure.GetRegionIds().Count, compoundStructure.IsRectangularRegion),
            nameof(CompoundStructure.IsSimpleRegion) => () => VariantsResolver.ResolveIndex(compoundStructure.GetRegionIds().Count, compoundStructure.IsSimpleRegion),
            nameof(CompoundStructure.IsStructuralDeck) => () => VariantsResolver.ResolveIndex(compoundStructure.LayerCount, compoundStructure.IsStructuralDeck),
            nameof(CompoundStructure.ParticipatesInWrapping) => () => VariantsResolver.ResolveIndex(compoundStructure.LayerCount, compoundStructure.ParticipatesInWrapping),
            _ => null
        };
    }
}