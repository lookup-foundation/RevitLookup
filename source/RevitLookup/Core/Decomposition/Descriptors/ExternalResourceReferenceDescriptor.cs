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

public sealed class ExternalResourceReferenceDescriptor(ExternalResourceReference externalResourceReference) : Descriptor, IDescriptorExtension
{
    public void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define(nameof(ExternalResourceServerUtils.ServerSupportsAssemblyCodeData)).Register(() => Variants.Value(ExternalResourceServerUtils.ServerSupportsAssemblyCodeData(externalResourceReference)));
        manager.Define(nameof(ExternalResourceServerUtils.ServerSupportsCADLinks)).Register(() => Variants.Value(ExternalResourceServerUtils.ServerSupportsCADLinks(externalResourceReference)));
        manager.Define(nameof(ExternalResourceServerUtils.ServerSupportsIFCLinks)).Register(() => Variants.Value(ExternalResourceServerUtils.ServerSupportsIFCLinks(externalResourceReference)));
        manager.Define(nameof(ExternalResourceServerUtils.ServerSupportsKeynotes)).Register(() => Variants.Value(ExternalResourceServerUtils.ServerSupportsKeynotes(externalResourceReference)));
        manager.Define(nameof(ExternalResourceServerUtils.ServerSupportsRevitLinks)).Register(() => Variants.Value(ExternalResourceServerUtils.ServerSupportsRevitLinks(externalResourceReference)));
    }
}
