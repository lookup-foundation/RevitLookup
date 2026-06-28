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

public sealed class CylindricalFaceDescriptor(CylindricalFace face) : FaceDescriptor(face)
{
    public override void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(CylindricalFace.Dispose)).Disable();
        configuration.Member("Radius").Resolve(ResolveRadius);
        return;

        IVariant ResolveRadius()
        {
            return Variants.Values<XYZ>(2)
                .Add(face.get_Radius(0), "Radius 0")
                .Add(face.get_Radius(1), "Radius 1")
                .Consume();
        }
    }
}