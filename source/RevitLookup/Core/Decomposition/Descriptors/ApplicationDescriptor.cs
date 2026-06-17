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

using Autodesk.Revit.DB.Macros;
using Autodesk.Revit.DB.PointClouds;
using Autodesk.Revit.DB.Structure.StructuralSections;
using Autodesk.Revit.DB.Visual;
using LookupEngine.Abstractions.Configuration;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class ApplicationDescriptor : ResolvingDescriptor, IDescriptorConfigurator
{
    private readonly Autodesk.Revit.ApplicationServices.Application _application;

    public ApplicationDescriptor(Autodesk.Revit.ApplicationServices.Application application)
    {
        _application = application;
        Name = application.VersionName;
    }

    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(Autodesk.Revit.ApplicationServices.Application.GetAssets)).Defer(() => ResolveEnum<AssetType, IList<Asset>>(_application.GetAssets));

        configuration.Extension("GetFormulaFunctions").AsStatic().Register(FormulaManager.GetFunctions);
        configuration.Extension("GetFormulaOperators").AsStatic().Register(FormulaManager.GetOperators);
        configuration.Extension("GetSupportedPointCloudEngines").AsStatic().Register(PointCloudEngineRegistry.GetSupportedEngines);
        configuration.Extension(nameof(LabelUtils.GetStructuralSectionShapeName)).Register(() => ResolveEnum<StructuralSectionShape, string>(LabelUtils.GetStructuralSectionShapeName));
        configuration.Extension(nameof(MacroManager.GetMacroManager)).Register(() => MacroManager.GetMacroManager(_application));
        configuration.Extension(nameof(OptionalFunctionalityUtils.IsDGNExportAvailable)).Register(() => OptionalFunctionalityUtils.IsDGNExportAvailable());
        configuration.Extension(nameof(OptionalFunctionalityUtils.IsDGNImportLinkAvailable)).Register(() => OptionalFunctionalityUtils.IsDGNImportLinkAvailable());
        configuration.Extension(nameof(OptionalFunctionalityUtils.IsDWFExportAvailable)).Register(() => OptionalFunctionalityUtils.IsDWFExportAvailable());
        configuration.Extension(nameof(OptionalFunctionalityUtils.IsDWGExportAvailable)).Register(() => OptionalFunctionalityUtils.IsDWGExportAvailable());
        configuration.Extension(nameof(OptionalFunctionalityUtils.IsDWGImportLinkAvailable)).Register(() => OptionalFunctionalityUtils.IsDWGImportLinkAvailable());
        configuration.Extension(nameof(OptionalFunctionalityUtils.IsDXFExportAvailable)).Register(() => OptionalFunctionalityUtils.IsDXFExportAvailable());
        configuration.Extension(nameof(OptionalFunctionalityUtils.IsFBXExportAvailable)).Register(() => OptionalFunctionalityUtils.IsFBXExportAvailable());
        configuration.Extension(nameof(OptionalFunctionalityUtils.IsGraphicsAvailable)).Register(() => OptionalFunctionalityUtils.IsGraphicsAvailable());
        configuration.Extension(nameof(OptionalFunctionalityUtils.IsIFCAvailable)).Register(() => OptionalFunctionalityUtils.IsIFCAvailable());
        configuration.Extension(nameof(OptionalFunctionalityUtils.IsNavisworksExporterAvailable)).Register(() => OptionalFunctionalityUtils.IsNavisworksExporterAvailable());
        configuration.Extension(nameof(OptionalFunctionalityUtils.IsSATImportLinkAvailable)).Register(() => OptionalFunctionalityUtils.IsSATImportLinkAvailable());
        configuration.Extension(nameof(OptionalFunctionalityUtils.IsShapeImporterAvailable)).Register(() => OptionalFunctionalityUtils.IsShapeImporterAvailable());
        configuration.Extension(nameof(OptionalFunctionalityUtils.IsSKPImportLinkAvailable)).Register(() => OptionalFunctionalityUtils.IsSKPImportLinkAvailable());
#if REVIT2022_OR_GREATER
        configuration.Extension(nameof(OptionalFunctionalityUtils.Is3DMImportLinkAvailable)).Register(() => OptionalFunctionalityUtils.Is3DMImportLinkAvailable());
        configuration.Extension(nameof(OptionalFunctionalityUtils.IsOBJImportLinkAvailable)).Register(() => OptionalFunctionalityUtils.IsOBJImportLinkAvailable());
        configuration.Extension(nameof(OptionalFunctionalityUtils.IsSTLImportLinkAvailable)).Register(() => OptionalFunctionalityUtils.IsSTLImportLinkAvailable());
#if !REVIT2027_OR_GREATER
        configuration.Extension(nameof(OptionalFunctionalityUtils.IsAXMImportLinkAvailable)).Register(() => OptionalFunctionalityUtils.IsAXMImportLinkAvailable());
#endif
#endif
#if REVIT2024_OR_GREATER
        configuration.Extension(nameof(OptionalFunctionalityUtils.IsSTEPImportLinkAvailable)).Register(() => OptionalFunctionalityUtils.IsSTEPImportLinkAvailable());
#endif
#if REVIT2026_OR_GREATER
        configuration.Extension(nameof(OptionalFunctionalityUtils.IsMaterialLibraryAvailable)).Register(() => OptionalFunctionalityUtils.IsMaterialLibraryAvailable());
        configuration.Extension(nameof(ModelPathUtils.GetAllCloudRegions)).Register(ModelPathUtils.GetAllCloudRegions);
        configuration.Extension(nameof(LabelUtils.GetFailureSeverityName)).Register(() => ResolveEnum<FailureSeverity, string>(LabelUtils.GetFailureSeverityName));
#endif
    }
}