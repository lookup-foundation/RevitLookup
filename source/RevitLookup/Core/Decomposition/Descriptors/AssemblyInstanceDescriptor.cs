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

public sealed class AssemblyInstanceDescriptor(AssemblyInstance assemblyInstance) : ElementDescriptor(assemblyInstance)
{
    public override void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define("AcquireViews").Map(nameof(AssemblyViewUtils.AcquireAssemblyViews)).AsNotSupported();
        manager.Define(nameof(AssemblyViewUtils.Create3DOrthographic)).AsNotSupported();
        manager.Define(nameof(AssemblyViewUtils.CreateDetailSection)).AsNotSupported();
        manager.Define(nameof(AssemblyViewUtils.CreateMaterialTakeoff)).AsNotSupported();
        manager.Define(nameof(AssemblyViewUtils.CreatePartList)).AsNotSupported();
        manager.Define(nameof(AssemblyViewUtils.CreateSheet)).AsNotSupported();
        manager.Define(nameof(AssemblyViewUtils.CreateSingleCategorySchedule)).AsNotSupported();
    }

}