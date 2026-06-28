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

public sealed class ReferencePointDescriptor(ReferencePoint referencePoint) : ElementDescriptor(referencePoint)
{
    public override void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(ReferencePoint.Dispose)).Disable();
        configuration.Extension(nameof(AdaptiveComponentFamilyUtils.IsAdaptivePoint)).Register(() => AdaptiveComponentFamilyUtils.IsAdaptivePoint(referencePoint.Document, referencePoint.Id));
        configuration.Extension(nameof(AdaptiveComponentFamilyUtils.MakeAdaptivePoint)).NotSupported();

        var isAdaptivePlacementPoint = SafeEvaluate(() => AdaptiveComponentFamilyUtils.IsAdaptivePlacementPoint(referencePoint.Document, referencePoint.Id));
        configuration.Extension(nameof(AdaptiveComponentFamilyUtils.IsAdaptivePlacementPoint)).Register(() => isAdaptivePlacementPoint);
        
        if (isAdaptivePlacementPoint)
        {
            configuration.Extension("GetAdaptivePlacementNumber").Map(nameof(AdaptiveComponentFamilyUtils.GetPlacementNumber)).Register(() => AdaptiveComponentFamilyUtils.GetPlacementNumber(referencePoint.Document, referencePoint.Id));
            configuration.Extension("SetAdaptivePlacementNumber").Map(nameof(AdaptiveComponentFamilyUtils.SetPlacementNumber)).NotSupported();
            configuration.Extension("GetAdaptivePointOrientationType").Map(nameof(AdaptiveComponentFamilyUtils.GetPointOrientationType)).Register(() => AdaptiveComponentFamilyUtils.GetPointOrientationType(referencePoint.Document, referencePoint.Id));
            configuration.Extension("SetAdaptivePointOrientationType").Map(nameof(AdaptiveComponentFamilyUtils.SetPointOrientationType)).NotSupported();
        }

        var isAdaptiveShapeHandlePoint = SafeEvaluate(() => AdaptiveComponentFamilyUtils.IsAdaptiveShapeHandlePoint(referencePoint.Document, referencePoint.Id));
        configuration.Extension(nameof(AdaptiveComponentFamilyUtils.IsAdaptiveShapeHandlePoint)).Register(() => isAdaptiveShapeHandlePoint);
        
        if (isAdaptiveShapeHandlePoint)
        {
            configuration.Extension("GetAdaptivePointConstraintType").Map(nameof(AdaptiveComponentFamilyUtils.GetPointConstraintType)).Register(() => AdaptiveComponentFamilyUtils.GetPointConstraintType(referencePoint.Document, referencePoint.Id));
            configuration.Extension("SetAdaptivePointConstraintType").Map(nameof(AdaptiveComponentFamilyUtils.SetPointConstraintType)).NotSupported();
        }
    }
}