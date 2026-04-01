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

using Autodesk.Revit.DB.Fabrication;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class ConnectorDescriptor : Descriptor, IDescriptorExtension
{
    public ConnectorDescriptor(Connector connector)
    {
        Name = connector.Id.ToString();
    }

    public void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define(nameof(MechanicalUtils.ConnectDuctPlaceholdersAtElbow)).AsNotSupported();
        manager.Define(nameof(MechanicalUtils.ConnectDuctPlaceholdersAtTee)).AsNotSupported();
        manager.Define(nameof(MechanicalUtils.ConnectDuctPlaceholdersAtCross)).AsNotSupported();
        manager.Define(nameof(PlumbingUtils.ConnectPipePlaceholdersAtElbow)).AsNotSupported();
        manager.Define(nameof(PlumbingUtils.ConnectPipePlaceholdersAtTee)).AsNotSupported();
        manager.Define(nameof(PlumbingUtils.ConnectPipePlaceholdersAtCross)).AsNotSupported();
        manager.Define("ValidateFabricationConnectivity").Map(nameof(FabricationUtils.ValidateConnectivity)).AsNotSupported();
        manager.Define("CreateFabricationPartRouteEnd").Map(nameof(FabricationPartRouteEnd.CreateFromConnector)).AsNotSupported();
    }

}