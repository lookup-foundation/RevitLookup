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

using LookupEngine.Abstractions.Configuration;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class CompoundStructureDescriptor(CompoundStructure compoundStructure) : ResolvingDescriptor, IDescriptorConfigurator
{
    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(CompoundStructure.CanLayerBeStructuralMaterial)).Resolve(() => ResolveRange(compoundStructure.LayerCount, compoundStructure.CanLayerBeStructuralMaterial));
        configuration.Member(nameof(CompoundStructure.CanLayerBeVariable)).Resolve(() => ResolveRange(compoundStructure.LayerCount, compoundStructure.CanLayerBeVariable));
        configuration.Member(nameof(CompoundStructure.CanLayerWidthBeNonZero)).Resolve(() => ResolveRange(compoundStructure.LayerCount, compoundStructure.CanLayerWidthBeNonZero));
        configuration.Member(nameof(CompoundStructure.GetAdjacentRegions)).Resolve(() => ResolveRange(compoundStructure.GetRegionIds().Count, compoundStructure.GetAdjacentRegions));
        configuration.Member(nameof(CompoundStructure.GetCoreBoundaryLayerIndex)).Resolve(() => ResolveEnum<ShellLayerType, int>(compoundStructure.GetCoreBoundaryLayerIndex));
        configuration.Member(nameof(CompoundStructure.GetDeckEmbeddingType)).Resolve(() => ResolveRange(compoundStructure.LayerCount, compoundStructure.GetDeckEmbeddingType));
        configuration.Member(nameof(CompoundStructure.GetDeckProfileId)).Resolve(() => ResolveRange(compoundStructure.LayerCount, compoundStructure.GetDeckProfileId));
        configuration.Member(nameof(CompoundStructure.GetLayerAssociatedToRegion)).Resolve(() => ResolveRange(compoundStructure.GetRegionIds().Count, compoundStructure.GetLayerAssociatedToRegion));
        configuration.Member(nameof(CompoundStructure.GetLayerFunction)).Resolve(() => ResolveRange(compoundStructure.LayerCount, compoundStructure.GetLayerFunction));
        configuration.Member(nameof(CompoundStructure.GetLayerWidth)).Resolve(() => ResolveRange(compoundStructure.LayerCount, compoundStructure.GetLayerWidth));
        configuration.Member(nameof(CompoundStructure.GetMaterialId)).Resolve(() => ResolveRange(compoundStructure.LayerCount, compoundStructure.GetMaterialId));
        configuration.Member(nameof(CompoundStructure.GetNumberOfShellLayers)).Resolve(() => ResolveEnum<ShellLayerType, int>(compoundStructure.GetNumberOfShellLayers));
        configuration.Member(nameof(CompoundStructure.GetOffsetForLocationLine)).Resolve(() => ResolveEnum<WallLocationLine, double>(compoundStructure.GetOffsetForLocationLine));
        configuration.Member(nameof(CompoundStructure.GetPreviousNonZeroLayerIndex)).Resolve(() => ResolveRange(compoundStructure.LayerCount, compoundStructure.GetPreviousNonZeroLayerIndex));
        configuration.Member(nameof(CompoundStructure.GetRegionEnvelope)).Resolve(() => ResolveRange(compoundStructure.GetRegionIds().Count, compoundStructure.GetRegionEnvelope));
        configuration.Member(nameof(CompoundStructure.GetRegionsAssociatedToLayer)).Resolve(() => ResolveRange(compoundStructure.LayerCount, compoundStructure.GetRegionsAssociatedToLayer));
        configuration.Member(nameof(CompoundStructure.GetSegmentCoordinate)).Resolve(() => ResolveRange(compoundStructure.GetSegmentIds().Count, compoundStructure.GetSegmentCoordinate));
        configuration.Member(nameof(CompoundStructure.GetSegmentOrientation)).Resolve(() => ResolveRange(compoundStructure.GetSegmentIds().Count, compoundStructure.GetSegmentOrientation));
        configuration.Member(nameof(CompoundStructure.GetWallSweepsInfo)).Resolve(() => ResolveEnum<WallSweepType, IList<WallSweepInfo>>(compoundStructure.GetWallSweepsInfo));
        configuration.Member(nameof(CompoundStructure.GetWidth)).When(parameters => parameters.Length == 1).Resolve(() => ResolveRange(compoundStructure.GetRegionIds().Count, compoundStructure.GetWidth));
        configuration.Member(nameof(CompoundStructure.IsCoreLayer)).Resolve(() => ResolveRange(compoundStructure.LayerCount, compoundStructure.IsCoreLayer));
        configuration.Member(nameof(CompoundStructure.IsRectangularRegion)).Resolve(() => ResolveRange(compoundStructure.GetRegionIds().Count, compoundStructure.IsRectangularRegion));
        configuration.Member(nameof(CompoundStructure.IsSimpleRegion)).Resolve(() => ResolveRange(compoundStructure.GetRegionIds().Count, compoundStructure.IsSimpleRegion));
        configuration.Member(nameof(CompoundStructure.IsStructuralDeck)).Resolve(() => ResolveRange(compoundStructure.LayerCount, compoundStructure.IsStructuralDeck));
        configuration.Member(nameof(CompoundStructure.ParticipatesInWrapping)).Resolve(() => ResolveRange(compoundStructure.LayerCount, compoundStructure.ParticipatesInWrapping));
    }
}