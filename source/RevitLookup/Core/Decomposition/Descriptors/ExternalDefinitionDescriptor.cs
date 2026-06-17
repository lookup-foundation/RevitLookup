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

public sealed class ExternalDefinitionDescriptor(ExternalDefinition externalDefinition) : Descriptor, IDescriptorConfigurator, IDescriptorConfigurator<Document>
{
    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Extension(nameof(RebarShapeParameters.IsValidExternalDefinition)).Register(() => RebarShapeParameters.IsValidExternalDefinition(externalDefinition));
        configuration.Extension("SearchExternalDefinition").Map(nameof(RebarShapeParameters.GetExternalDefinitionForElementId)).NotSupported();
    }

    void IDescriptorConfigurator<Document>.Configure(IMemberConfigurator<Document> configuration)
    {
        configuration.Extension("GetRebarShapeParameterElementId").Map(nameof(RebarShapeParameters.GetElementIdForExternalDefinition)).Register(context => RebarShapeParameters.GetElementIdForExternalDefinition(context, externalDefinition));
        configuration.Extension("GetOrCreateRebarShapeParameterElementId").Map(nameof(RebarShapeParameters.GetOrCreateElementIdForExternalDefinition)).Register(context => RebarShapeParameters.GetOrCreateElementIdForExternalDefinition(context, externalDefinition));
    }
}