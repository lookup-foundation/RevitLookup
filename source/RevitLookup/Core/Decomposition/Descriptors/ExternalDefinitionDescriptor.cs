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

using Autodesk.Revit.DB.Structure;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class ExternalDefinitionDescriptor(ExternalDefinition externalDefinition) : Descriptor, IDescriptorExtension, IDescriptorExtension<Document>
{
    public void RegisterExtensions(IExtensionManager manager)
    {
        manager.Register(nameof(RebarShapeParameters.IsValidExternalDefinition), () => Variants.Value(RebarShapeParameters.IsValidExternalDefinition(externalDefinition)));
        manager.Register(nameof(RebarShapeParameters.GetExternalDefinitionForElementId), Variants.NotSupported);
    }

    public void RegisterExtensions(IExtensionManager<Document> manager)
    {        
        manager.Register(nameof(RebarShapeParameters.GetElementIdForExternalDefinition), context => Variants.Value(RebarShapeParameters.GetElementIdForExternalDefinition(context, externalDefinition)));
        manager.Register(nameof(RebarShapeParameters.GetOrCreateElementIdForExternalDefinition), context => Variants.Value(RebarShapeParameters.GetOrCreateElementIdForExternalDefinition(context, externalDefinition)));
    }
}
