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
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class SpatialElementDescriptor(SpatialElement spatialElement) : ElementDescriptor(spatialElement)
{
    public override Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(SpatialElement.GetBoundarySegments) => ResolveGetBoundarySegments,
            _ => null
        };

        IVariant ResolveGetBoundarySegments()
        {
            return Variants.Values<IList<IList<BoundarySegment>>>(8)
                .Add(spatialElement.GetBoundarySegments(new SpatialElementBoundaryOptions
                {
                    SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Center,
                    StoreFreeBoundaryFaces = true
                }), "Center, store free boundary faces")
                .Add(spatialElement.GetBoundarySegments(new SpatialElementBoundaryOptions
                {
                    SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.CoreBoundary,
                    StoreFreeBoundaryFaces = true
                }), "Core boundary, store free boundary faces")
                .Add(spatialElement.GetBoundarySegments(new SpatialElementBoundaryOptions
                {
                    SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish,
                    StoreFreeBoundaryFaces = true
                }), "Finish, store free boundary faces")
                .Add(spatialElement.GetBoundarySegments(new SpatialElementBoundaryOptions
                {
                    SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.CoreCenter,
                    StoreFreeBoundaryFaces = true
                }), "Core center, store free boundary faces")
                .Add(spatialElement.GetBoundarySegments(new SpatialElementBoundaryOptions
                {
                    SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Center,
                    StoreFreeBoundaryFaces = true
                }), "Center")
                .Add(spatialElement.GetBoundarySegments(new SpatialElementBoundaryOptions
                {
                    SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.CoreBoundary,
                    StoreFreeBoundaryFaces = true
                }), "Core boundary")
                .Add(spatialElement.GetBoundarySegments(new SpatialElementBoundaryOptions
                {
                    SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish,
                    StoreFreeBoundaryFaces = true
                }), "Finish")
                .Add(spatialElement.GetBoundarySegments(new SpatialElementBoundaryOptions
                {
                    SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.CoreCenter,
                    StoreFreeBoundaryFaces = true
                }), "Core center")
                .Consume();
        }
    }
}