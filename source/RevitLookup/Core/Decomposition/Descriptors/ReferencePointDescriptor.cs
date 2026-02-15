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
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class ReferencePointDescriptor(ReferencePoint referencePoint) : ElementDescriptor(referencePoint)
{
    public override void RegisterExtensions(IExtensionManager manager)
    {
        manager.Register(nameof(AdaptiveComponentFamilyUtils.GetPlacementNumber), () => Variants.Value(AdaptiveComponentFamilyUtils.GetPlacementNumber(referencePoint.Document, referencePoint.Id)));
        manager.Register(nameof(AdaptiveComponentFamilyUtils.GetPointConstraintType), () => Variants.Value(AdaptiveComponentFamilyUtils.GetPointConstraintType(referencePoint.Document, referencePoint.Id)));
        manager.Register(nameof(AdaptiveComponentFamilyUtils.GetPointOrientationType), () => Variants.Value(AdaptiveComponentFamilyUtils.GetPointOrientationType(referencePoint.Document, referencePoint.Id)));
        manager.Register(nameof(AdaptiveComponentFamilyUtils.IsAdaptivePlacementPoint), () => Variants.Value(AdaptiveComponentFamilyUtils.IsAdaptivePlacementPoint(referencePoint.Document, referencePoint.Id)));
        manager.Register(nameof(AdaptiveComponentFamilyUtils.IsAdaptivePoint), () => Variants.Value(AdaptiveComponentFamilyUtils.IsAdaptivePoint(referencePoint.Document, referencePoint.Id)));
        manager.Register(nameof(AdaptiveComponentFamilyUtils.IsAdaptiveShapeHandlePoint), () => Variants.Value(AdaptiveComponentFamilyUtils.IsAdaptiveShapeHandlePoint(referencePoint.Document, referencePoint.Id)));
        manager.Register(nameof(AdaptiveComponentFamilyUtils.MakeAdaptivePoint), Variants.NotSupported);
        manager.Register(nameof(AdaptiveComponentFamilyUtils.SetPlacementNumber), Variants.NotSupported);
        manager.Register(nameof(AdaptiveComponentFamilyUtils.SetPointConstraintType), Variants.NotSupported);
        manager.Register(nameof(AdaptiveComponentFamilyUtils.SetPointOrientationType), Variants.NotSupported);
    }
}
