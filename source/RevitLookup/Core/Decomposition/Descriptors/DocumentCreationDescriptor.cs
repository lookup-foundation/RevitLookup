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
using Document = Autodesk.Revit.Creation.Document;

namespace RevitLookup.Core.Decomposition.Descriptors;

#pragma warning disable CS9113 // Parameter is unread.
public sealed class DocumentCreationDescriptor(Document document) : Descriptor, IDescriptorConfigurator
#pragma warning restore CS9113 // Parameter is unread.
{
    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(Document.Dispose)).Disable();
#if REVIT2024_OR_GREATER
        configuration.Extension("NewDuctworkStiffener").Map(nameof(MEPSupportUtils.CreateDuctworkStiffener)).NotSupported();
#endif
    }
}