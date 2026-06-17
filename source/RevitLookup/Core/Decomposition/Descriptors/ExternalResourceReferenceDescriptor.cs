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

public sealed class ExternalResourceReferenceDescriptor(ExternalResourceReference externalResourceReference) : Descriptor, IDescriptorConfigurator
{
    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Extension(nameof(ExternalResourceServerUtils.ServerSupportsAssemblyCodeData)).Register(() => ExternalResourceServerUtils.ServerSupportsAssemblyCodeData(externalResourceReference));
        configuration.Extension(nameof(ExternalResourceServerUtils.ServerSupportsCADLinks)).Register(() => ExternalResourceServerUtils.ServerSupportsCADLinks(externalResourceReference));
        configuration.Extension(nameof(ExternalResourceServerUtils.ServerSupportsIFCLinks)).Register(() => ExternalResourceServerUtils.ServerSupportsIFCLinks(externalResourceReference));
        configuration.Extension(nameof(ExternalResourceServerUtils.ServerSupportsKeynotes)).Register(() => ExternalResourceServerUtils.ServerSupportsKeynotes(externalResourceReference));
        configuration.Extension(nameof(ExternalResourceServerUtils.ServerSupportsRevitLinks)).Register(() => ExternalResourceServerUtils.ServerSupportsRevitLinks(externalResourceReference));
    }
}
