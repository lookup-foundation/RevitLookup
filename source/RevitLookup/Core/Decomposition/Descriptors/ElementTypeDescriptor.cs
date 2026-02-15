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

using Autodesk.Revit.DB.ExternalData;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class ElementTypeDescriptor(ElementType elementType) : ElementDescriptor(elementType)
{
    public override void RegisterExtensions(IExtensionManager manager)
    {
        manager.Register(nameof(CoordinationModelLinkUtils.GetCoordinationModelTypeData), () => Variants.Value(CoordinationModelLinkUtils.GetCoordinationModelTypeData(elementType.Document, elementType)));
        manager.Register("ContainsCoordinationModelCategory", Variants.NotSupported); //CoordinationModelLinkUtils.ContainsCategory
        manager.Register("GetCoordinationModelColorOverrideForCategory", Variants.NotSupported); //CoordinationModelLinkUtils.GetColorOverrideForCategory
        manager.Register("GetCoordinationModelVisibilityOverrideForCategory", Variants.NotSupported); //CoordinationModelLinkUtils.GetVisibilityOverrideForCategory
        manager.Register("ReloadCoordinationModel", Variants.NotSupported); //CoordinationModelLinkUtils.Reload
        manager.Register("ReloadCoordinationModelFromAutodeskDocs", Variants.NotSupported); //CoordinationModelLinkUtils.ReloadAutodeskDocsCoordinationModelFrom
        manager.Register("ReloadLocalCoordinationModel", Variants.NotSupported); //CoordinationModelLinkUtils.ReloadLocalCoordinationModelFrom
        manager.Register("SetCoordinationModelColorOverrideForCategory", Variants.NotSupported); //CoordinationModelLinkUtils.SetColorOverrideForCategory
        manager.Register("SetCoordinationModelVisibilityOverrideForCategory", Variants.NotSupported); //CoordinationModelLinkUtils.SetVisibilityOverrideForCategory
        manager.Register("UnloadCoordinationModel", Variants.NotSupported); //CoordinationModelLinkUtils.Unload
    }
}
