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
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class ApplicationDescriptor : Descriptor, IDescriptorExtension
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
    //        TODO slow, ~8s
    //         nameof(Autodesk.Revit.ApplicationServices.Application.GetAssets) => ResolveGetAssets,
    //         _ => null
    //     };
    // }

    public void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define("GetFormulaFunctions").AsStatic().Register(() => Variants.Value(FormulaManager.GetFunctions()));
        manager.Define("GetFormulaOperators").AsStatic().Register(() => Variants.Value(FormulaManager.GetOperators()));
        manager.Define("GetSupportedPointCloudEngines").AsStatic().Register(() => Variants.Value(PointCloudEngineRegistry.GetSupportedEngines()));
        manager.Define(nameof(LabelUtils.GetStructuralSectionShapeName)).Register(() => VariantsResolver.ResolveEnum<StructuralSectionShape, string>(LabelUtils.GetStructuralSectionShapeName));
        manager.Define(nameof(MacroManager.GetMacroManager)).Register(() => Variants.Value(MacroManager.GetMacroManager(_application)));
        manager.Define(nameof(OptionalFunctionalityUtils.IsDGNExportAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.IsDGNExportAvailable()));
        manager.Define(nameof(OptionalFunctionalityUtils.IsDGNImportLinkAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.IsDGNImportLinkAvailable()));
        manager.Define(nameof(OptionalFunctionalityUtils.IsDWFExportAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.IsDWFExportAvailable()));
        manager.Define(nameof(OptionalFunctionalityUtils.IsDWGExportAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.IsDWGExportAvailable()));
        manager.Define(nameof(OptionalFunctionalityUtils.IsDWGImportLinkAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.IsDWGImportLinkAvailable()));
        manager.Define(nameof(OptionalFunctionalityUtils.IsDXFExportAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.IsDXFExportAvailable()));
        manager.Define(nameof(OptionalFunctionalityUtils.IsFBXExportAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.IsFBXExportAvailable()));
        manager.Define(nameof(OptionalFunctionalityUtils.IsGraphicsAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.IsGraphicsAvailable()));
        manager.Define(nameof(OptionalFunctionalityUtils.IsIFCAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.IsIFCAvailable()));
        manager.Define(nameof(OptionalFunctionalityUtils.IsNavisworksExporterAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.IsNavisworksExporterAvailable()));
        manager.Define(nameof(OptionalFunctionalityUtils.IsSATImportLinkAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.IsSATImportLinkAvailable()));
        manager.Define(nameof(OptionalFunctionalityUtils.IsShapeImporterAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.IsShapeImporterAvailable()));
        manager.Define(nameof(OptionalFunctionalityUtils.IsSKPImportLinkAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.IsSKPImportLinkAvailable()));
#if REVIT2022_OR_GREATER
        manager.Define(nameof(OptionalFunctionalityUtils.Is3DMImportLinkAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.Is3DMImportLinkAvailable()));
        manager.Define(nameof(OptionalFunctionalityUtils.IsOBJImportLinkAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.IsOBJImportLinkAvailable()));
        manager.Define(nameof(OptionalFunctionalityUtils.IsSTLImportLinkAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.IsSTLImportLinkAvailable()));
#if !REVIT2027_OR_GREATER
        manager.Define(nameof(OptionalFunctionalityUtils.IsAXMImportLinkAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.IsAXMImportLinkAvailable()));
#endif
#endif
#if REVIT2024_OR_GREATER
        manager.Define(nameof(OptionalFunctionalityUtils.IsSTEPImportLinkAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.IsSTEPImportLinkAvailable()));
#endif
#if REVIT2026_OR_GREATER
        manager.Define(nameof(OptionalFunctionalityUtils.IsMaterialLibraryAvailable)).Register(() => Variants.Value(OptionalFunctionalityUtils.IsMaterialLibraryAvailable()));
        manager.Define(nameof(ModelPathUtils.GetAllCloudRegions)).Register(() => Variants.Value(ModelPathUtils.GetAllCloudRegions()));
        manager.Define(nameof(LabelUtils.GetFailureSeverityName)).Register(() => VariantsResolver.ResolveEnum<FailureSeverity, string>(LabelUtils.GetFailureSeverityName));
#endif
    }
}