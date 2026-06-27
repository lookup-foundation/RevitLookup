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

#if !REVIT2025_OR_GREATER
using Autodesk.Revit.DB.Macros;
#endif
using System.Windows.Documents;
using Autodesk.Revit.DB.Fabrication;
using Autodesk.Revit.DB.Lighting;
using Autodesk.Revit.DB.Structure;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
#if REVIT2026_OR_GREATER
using Autodesk.Revit.DB.ExternalData;
#endif

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class DocumentDescriptor : Descriptor, IDescriptorConfigurator
{
    private readonly Document _document;

    public DocumentDescriptor(Document document)
    {
        _document = document;
        Name = document.Title;
    }

    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(Document.Close)).Defer();
        configuration.Member(nameof(Document.PlanTopologies)).Defer(ResolvePlanTopologies);
        configuration.Member(nameof(Document.GetDefaultElementTypeId)).Resolve(ResolveDefaultElementTypeId);
        configuration.Member(nameof(Document.GetDocumentVersion)).Resolve(() => Document.GetDocumentVersion(_document));
#if REVIT2024_OR_GREATER
        configuration.Member(nameof(Document.GetUnusedElements)).Resolve(() => _document.GetUnusedElements(new HashSet<ElementId>()));
        configuration.Member(nameof(Document.GetAllUnusedElements)).Resolve(() => _document.GetAllUnusedElements(new HashSet<ElementId>()));
#endif

        configuration.Extension(nameof(AssemblyCodeTable.GetAssemblyCodeTable)).Register(() => AssemblyCodeTable.GetAssemblyCodeTable(_document));
        configuration.Extension(nameof(ExternalFileUtils.GetAllExternalFileReferences)).Register(() => ExternalFileUtils.GetAllExternalFileReferences(_document));
        configuration.Extension(nameof(ExternalResourceUtils.GetAllExternalResourceReferences)).Register(() => ExternalResourceUtils.GetAllExternalResourceReferences(_document));
        configuration.Extension(nameof(GlobalParametersManager.GetAllGlobalParameters)).Register(() => GlobalParametersManager.GetAllGlobalParameters(_document));
        configuration.Extension(nameof(GlobalParametersManager.AreGlobalParametersAllowed)).Register(() => GlobalParametersManager.AreGlobalParametersAllowed(_document));
        configuration.Extension(nameof(GlobalParametersManager.GetGlobalParametersOrdered)).Register(() => GlobalParametersManager.GetGlobalParametersOrdered(_document));
        configuration.Extension(nameof(KeynoteTable.GetKeynoteTable)).Register(() => KeynoteTable.GetKeynoteTable(_document));
        configuration.Extension(nameof(LightGroupManager.GetLightGroupManager)).Register(() => LightGroupManager.GetLightGroupManager(_document));
        configuration.Extension(nameof(UpdaterRegistry.GetRegisteredUpdaterInfos)).Register(() => UpdaterRegistry.GetRegisteredUpdaterInfos(_document));
        configuration.Extension(nameof(LightFamily.GetLightFamily)).Register(() => LightFamily.GetLightFamily(_document));
        configuration.Extension(nameof(FamilySizeTableManager.CreateFamilySizeTableManager)).Register(() => FamilySizeTableManager.GetFamilySizeTableManager(_document, new ElementId(BuiltInParameter.RBS_LOOKUP_TABLE_NAME)));
        configuration.Extension("GbXmlId").Map(nameof(ExportUtils.GetGBXMLDocumentId)).Register(() => ExportUtils.GetGBXMLDocumentId(_document));
        configuration.Extension(nameof(LoadedFamilyIntegrityCheck.CheckAllFamilies)).Defer(RegisterCorruptFamilyIds);
        configuration.Extension(nameof(LoadedFamilyIntegrityCheck.CheckAllFamiliesSlow)).Defer(RegisterGetUserWorksetInfo);
        configuration.Extension(nameof(WorksharingUtils.GetUserWorksetInfo)).Defer(() => WorksharingUtils.GetUserWorksetInfo(_document.GetWorksharingCentralModelPath()));
        configuration.Extension(nameof(WorksharingUtils.RelinquishOwnership)).NotSupported();
        configuration.Extension(nameof(FabricationUtils.ExportToPCF)).NotSupported();
#if !REVIT2025_OR_GREATER
        configuration.Extension(nameof(MacroManager.GetMacroManager)).Register(() => MacroManager.GetMacroManager(_document));
#endif
#if REVIT2022_OR_GREATER
        configuration.Extension(nameof(TemporaryGraphicsManager.GetTemporaryGraphicsManager)).Register(() => TemporaryGraphicsManager.GetTemporaryGraphicsManager(_document));
#endif
#if REVIT2023_OR_GREATER
        configuration.Extension(nameof(AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager)).Register(() => AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(_document));
#endif
#if REVIT2026_OR_GREATER
        configuration.Extension(nameof(CoordinationModelLinkUtils.GetAllCoordinationModelInstanceIds)).Register(() => CoordinationModelLinkUtils.GetAllCoordinationModelInstanceIds(_document));
        configuration.Extension(nameof(CoordinationModelLinkUtils.GetAllCoordinationModelTypeIds)).Register(() => CoordinationModelLinkUtils.GetAllCoordinationModelTypeIds(_document));
        configuration.Extension(nameof(CoordinationModelLinkUtils.Link3DViewFromAutodeskDocs)).NotSupported();
        configuration.Extension(nameof(CoordinationModelLinkUtils.LinkCoordinationModelFromLocalPath)).NotSupported();
#endif
        return;

        IVariant ResolvePlanTopologies()
        {
            if (_document.IsReadOnly) return Variants.Empty<PlanTopologySet>();

            using var transaction = new Transaction(_document);
            transaction.Start("Calculating plan topologies");
            var topologies = _document.PlanTopologies;
            transaction.Commit();

            return Variants.Value(topologies);
        }

        IVariant ResolveDefaultElementTypeId()
        {
            var values = Enum.GetValues<ElementTypeGroup>();
            var variants = Variants.Values<ElementId>(values.Length);

            foreach (var value in values)
            {
                var result = _document.GetDefaultElementTypeId(value);
                if (result is not null && result != ElementId.InvalidElementId)
                {
                    var element = result.ToElement(_document);
                    if (element is not null)
                    {
                        variants.Add(result, $"{value.ToString()}: {element.Name}");
                        continue;
                    }
                }

                variants.Add(result, $"{value.ToString()}: {result}");
            }

            return variants.Consume();
        }
        
        object? RegisterCorruptFamilyIds()
        {
            var corruptFamilyIds = new HashSet<ElementId>();
            LoadedFamilyIntegrityCheck.CheckAllFamilies(_document, corruptFamilyIds);
            return corruptFamilyIds;
        }

        object? RegisterCheckAllFamiliesSlow()
        {
            var corruptFamilyIds = new HashSet<ElementId>();
            LoadedFamilyIntegrityCheck.CheckAllFamiliesSlow(_document, corruptFamilyIds);
            return corruptFamilyIds;
        }

        object? RegisterGetUserWorksetInfo()
        {
            return WorksharingUtils.GetUserWorksetInfo(_document.GetWorksharingCentralModelPath());
        }
    }
}