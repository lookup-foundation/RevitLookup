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

using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.DB.Macros;
using Autodesk.Revit.DB.PointClouds;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class ApplicationDescriptor : Descriptor,IDescriptorExtension
{
    private readonly Autodesk.Revit.ApplicationServices.Application _application;

    public ApplicationDescriptor(Autodesk.Revit.ApplicationServices.Application application)
    {
        _application = application;
        Name = application.VersionName;
    }

    // public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    // {
    //     return target switch
    //     {
    //         nameof(Autodesk.Revit.ApplicationServices.Application.GetAssets) => ResolveGetAssets,
    //         _ => null
    //     };
    //
    //     TODO slow, ~8s
    //     IVariant ResolveGetAssets()
    //     {
    //         var assetTypes = Enum.GetValues<AssetType>();
    //         var capacity = assetTypes.Length;
    //         var variants = Variants.Values<IList<Asset>>(capacity);
    //
    //         foreach (var assetType in assetTypes)
    //         {
    //             variants.Add(Context.Application.GetAssets(assetType));
    //         }
    //
    //         return variants.Consume();
    //     }
    // }

    public void RegisterExtensions(IExtensionManager manager)
    {
        manager.Register("GetFormulaFunctions", () => Variants.Value(FormulaManager.GetFunctions()));
        manager.Register("GetFormulaOperators", () => Variants.Value(FormulaManager.GetOperators()));
        manager.Register(nameof(MacroManager.GetMacroManager), () => Variants.Value(MacroManager.GetMacroManager(_application)));
        manager.Register(nameof(ExternalServiceRegistry.GetServices), () => Variants.Value(ExternalServiceRegistry.GetServices()));
        manager.Register(nameof(ModelPathUtils.GetAllCloudRegions), () => Variants.Value(ModelPathUtils.GetAllCloudRegions()));
        manager.Register(nameof(OptionalFunctionalityUtils.Is3DMImportLinkAvailable), () => Variants.Value(OptionalFunctionalityUtils.Is3DMImportLinkAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsAXMImportLinkAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsAXMImportLinkAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsDGNExportAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsDGNExportAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsDGNImportLinkAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsDGNImportLinkAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsDWFExportAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsDWFExportAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsDWGExportAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsDWGExportAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsDWGImportLinkAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsDWGImportLinkAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsDXFExportAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsDXFExportAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsFBXExportAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsFBXExportAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsGraphicsAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsGraphicsAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsIFCAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsIFCAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsMaterialLibraryAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsMaterialLibraryAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsNavisworksExporterAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsNavisworksExporterAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsOBJImportLinkAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsOBJImportLinkAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsSATImportLinkAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsSATImportLinkAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsShapeImporterAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsShapeImporterAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsSKPImportLinkAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsSKPImportLinkAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsSTEPImportLinkAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsSTEPImportLinkAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsSTLImportLinkAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsSTLImportLinkAvailable()));
        manager.Register(nameof(ParameterFilterUtilities.GetAllFilterableCategories), () => Variants.Value(ParameterFilterUtilities.GetAllFilterableCategories()));
        manager.Register(nameof(ParameterUtils.GetAllBuiltInGroups), () => Variants.Value(ParameterUtils.GetAllBuiltInGroups()));
        manager.Register(nameof(ParameterUtils.GetAllBuiltInParameters), () => Variants.Value(ParameterUtils.GetAllBuiltInParameters()));
        manager.Register("GetSupportedPointCloudEngines", () => Variants.Value(PointCloudEngineRegistry.GetSupportedEngines()));
        manager.Register(nameof(SpecUtils.GetAllSpecs), () => Variants.Value(SpecUtils.GetAllSpecs()));
        manager.Register(nameof(UnitUtils.GetAllDisciplines), () => Variants.Value(UnitUtils.GetAllDisciplines()));
        manager.Register(nameof(UnitUtils.GetAllMeasurableSpecs), () => Variants.Value(UnitUtils.GetAllMeasurableSpecs()));
        manager.Register(nameof(UpdaterRegistry.GetRegisteredUpdaterInfos), () => Variants.Value(UpdaterRegistry.GetRegisteredUpdaterInfos()));
    }
}