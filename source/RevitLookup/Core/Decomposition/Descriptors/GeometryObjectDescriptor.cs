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

#pragma warning disable CS9113 // Parameter is unread.
public sealed class GeometryObjectDescriptor(GeometryObject geometryObject) : Descriptor, IDescriptorExtension
#pragma warning restore CS9113 // Parameter is unread.
{
    public void RegisterExtensions(IExtensionManager manager)
    {
#if REVIT2022_OR_GREATER
        manager.Define(nameof(ExternallyTaggedGeometryValidation.IsNonSolid)).Register(() => Variants.Value(ExternallyTaggedGeometryValidation.IsNonSolid(geometryObject)));
        manager.Define(nameof(ExternallyTaggedGeometryValidation.IsSolid)).Register(() => Variants.Value(ExternallyTaggedGeometryValidation.IsSolid(geometryObject)));
#endif
#if REVIT2024_OR_GREATER
        manager.Define(nameof(ExternallyTaggedGeometryValidation.LacksSubnodes)).Register(() => Variants.Value(ExternallyTaggedGeometryValidation.LacksSubnodes(geometryObject)));
#endif
    }
}