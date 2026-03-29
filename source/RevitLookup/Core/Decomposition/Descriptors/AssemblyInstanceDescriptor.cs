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

public sealed class AssemblyInstanceDescriptor(AssemblyInstance assemblyInstance) : ElementDescriptor(assemblyInstance)
{
    public override void RegisterExtensions(IExtensionManager manager)
    {
        RegisterNotSupportedExtensions(manager);
    }

    // Indicates API methods that exist but cannot produce a read-only value in RevitLookup
    private void RegisterNotSupportedExtensions(IExtensionManager manager)
    {
        _ = nameof(AssemblyViewUtils.AcquireAssemblyViews);
        manager.Register("AcquireViews", Variants.NotSupported);

        manager.Register(nameof(AssemblyViewUtils.Create3DOrthographic), Variants.NotSupported);
        manager.Register(nameof(AssemblyViewUtils.CreateDetailSection), Variants.NotSupported);
        manager.Register(nameof(AssemblyViewUtils.CreateMaterialTakeoff), Variants.NotSupported);
        manager.Register(nameof(AssemblyViewUtils.CreatePartList), Variants.NotSupported);
        manager.Register(nameof(AssemblyViewUtils.CreateSheet), Variants.NotSupported);
        manager.Register(nameof(AssemblyViewUtils.CreateSingleCategorySchedule), Variants.NotSupported);
    }
}