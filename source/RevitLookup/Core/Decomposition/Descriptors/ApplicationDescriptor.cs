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
        manager.Register("GetSupportedPointCloudEngines", () => Variants.Value(PointCloudEngineRegistry.GetSupportedEngines()));
        manager.Register(nameof(MacroManager.GetMacroManager), () => Variants.Value(MacroManager.GetMacroManager(_application)));
        manager.Register(nameof(OptionalFunctionalityUtils.IsDGNExportAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsDGNExportAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsDGNImportLinkAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsDGNImportLinkAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsDWFExportAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsDWFExportAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsDWGExportAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsDWGExportAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsDWGImportLinkAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsDWGImportLinkAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsDXFExportAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsDXFExportAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsFBXExportAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsFBXExportAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsGraphicsAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsGraphicsAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsIFCAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsIFCAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsNavisworksExporterAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsNavisworksExporterAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsSATImportLinkAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsSATImportLinkAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsShapeImporterAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsShapeImporterAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsSKPImportLinkAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsSKPImportLinkAvailable()));
        manager.Register(nameof(LabelUtils.GetStructuralSectionShapeName), ResolveGetStructuralSectionShapeName);
#if REVIT2022_OR_GREATER 
        manager.Register(nameof(OptionalFunctionalityUtils.IsAXMImportLinkAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsAXMImportLinkAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.Is3DMImportLinkAvailable), () => Variants.Value(OptionalFunctionalityUtils.Is3DMImportLinkAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsOBJImportLinkAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsOBJImportLinkAvailable()));
        manager.Register(nameof(OptionalFunctionalityUtils.IsSTLImportLinkAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsSTLImportLinkAvailable()));
#endif
#if REVIT2024_OR_GREATER
        manager.Register(nameof(OptionalFunctionalityUtils.IsSTEPImportLinkAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsSTEPImportLinkAvailable()));
#endif
#if REVIT2026_OR_GREATER
        manager.Register(nameof(OptionalFunctionalityUtils.IsMaterialLibraryAvailable), () => Variants.Value(OptionalFunctionalityUtils.IsMaterialLibraryAvailable()));
        manager.Register(nameof(LabelUtils.GetFailureSeverityName), ResolveGetFailureSeverityName);
#endif
        return;

#if REVIT2026_OR_GREATER
        IVariant ResolveGetFailureSeverityName()
        {
            var severities = Enum.GetValues<FailureSeverity>();
            var variants = Variants.Values<string>(severities.Length);
            foreach (var severity in severities)
            {
                var name = LabelUtils.GetFailureSeverityName(severity);
                variants.Add(name, $"{severity}: {name}");
            }

            return variants.Consume();
        }
#endif

        IVariant ResolveGetStructuralSectionShapeName()
        {
            var shapes = Enum.GetValues<StructuralSectionShape>();
            var variants = Variants.Values<string>(shapes.Length);
            foreach (var shape in shapes)
            {
                var name = LabelUtils.GetStructuralSectionShapeName(shape);
                variants.Add(name, $"{shape}: {name}");
            }

            return variants.Consume();
        }
    }
}