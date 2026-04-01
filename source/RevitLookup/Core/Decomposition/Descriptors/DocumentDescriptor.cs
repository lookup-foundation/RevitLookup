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
using System.Reflection;
using Autodesk.Revit.DB.Fabrication;
using Autodesk.Revit.DB.Lighting;
using Autodesk.Revit.DB.Structure;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
#if REVIT2026_OR_GREATER
using Autodesk.Revit.DB.ExternalData;
#endif

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class DocumentDescriptor : Descriptor, IDescriptorResolver, IDescriptorExtension
{
    private readonly Document _document;

    public DocumentDescriptor(Document document)
    {
        _document = document;
        Name = document.Title;
    }

    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(Document.Close) => Variants.Disabled,
            nameof(Document.PlanTopologies) => ResolvePlanTopologies,
            nameof(Document.GetDefaultElementTypeId) => ResolveDefaultElementTypeId,
            nameof(Document.GetDocumentVersion) => () => Variants.Value(Document.GetDocumentVersion(_document)),
#if REVIT2024_OR_GREATER
            nameof(Document.GetUnusedElements) => () => Variants.Value(_document.GetUnusedElements(new HashSet<ElementId>())),
            nameof(Document.GetAllUnusedElements) => () => Variants.Value(_document.GetAllUnusedElements(new HashSet<ElementId>())),
#endif
            _ => null
        };

        IVariant ResolvePlanTopologies()
        {
            if (_document.IsReadOnly) return Variants.Empty<PlanTopologySet>();

            var transaction = new Transaction(_document);
            transaction.Start("Calculating plan topologies");
            var topologies = _document.PlanTopologies;
            transaction.Commit();

            return Variants.Value(topologies);
        }

        IVariant ResolveDefaultElementTypeId()
        {
            var values = Enum.GetValues(typeof(ElementTypeGroup));
            var variants = Variants.Values<ElementId>(values.Length);

            foreach (ElementTypeGroup value in values)
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
    }

    public void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define(nameof(AssemblyCodeTable.GetAssemblyCodeTable)).Register(() => Variants.Value(AssemblyCodeTable.GetAssemblyCodeTable(_document)));
        manager.Define(nameof(ExternalFileUtils.GetAllExternalFileReferences)).Register(() => Variants.Value(ExternalFileUtils.GetAllExternalFileReferences(_document)));
        manager.Define(nameof(ExternalResourceUtils.GetAllExternalResourceReferences)).Register(() => Variants.Value(ExternalResourceUtils.GetAllExternalResourceReferences(_document)));
        manager.Define(nameof(GlobalParametersManager.GetAllGlobalParameters)).Register(() => Variants.Value(GlobalParametersManager.GetAllGlobalParameters(_document)));
        manager.Define(nameof(GlobalParametersManager.AreGlobalParametersAllowed)).Register(() => Variants.Value(GlobalParametersManager.AreGlobalParametersAllowed(_document)));
        manager.Define(nameof(GlobalParametersManager.GetGlobalParametersOrdered)).Register(() => Variants.Value(GlobalParametersManager.GetGlobalParametersOrdered(_document)));
        manager.Define(nameof(KeynoteTable.GetKeynoteTable)).Register(() => Variants.Value(KeynoteTable.GetKeynoteTable(_document)));
        manager.Define(nameof(LightGroupManager.GetLightGroupManager)).Register(() => Variants.Value(LightGroupManager.GetLightGroupManager(_document)));
        manager.Define(nameof(UpdaterRegistry.GetRegisteredUpdaterInfos)).Register(() => Variants.Value(UpdaterRegistry.GetRegisteredUpdaterInfos(_document)));
        manager.Define(nameof(LightFamily.GetLightFamily)).Register(() => Variants.Value(LightFamily.GetLightFamily(_document)));
        manager.Define(nameof(FamilySizeTableManager.CreateFamilySizeTableManager)).Register(() => Variants.Value(FamilySizeTableManager.GetFamilySizeTableManager(_document, new ElementId(BuiltInParameter.RBS_LOOKUP_TABLE_NAME))));
        manager.Define("GbXmlId").Map(nameof(ExportUtils.GetGBXMLDocumentId)).Register(() => Variants.Value(ExportUtils.GetGBXMLDocumentId(_document)));
        manager.Define(nameof(LoadedFamilyIntegrityCheck.CheckAllFamilies)).AsNotSupported(); //TODO add impl after Lazy invocation feature
        manager.Define(nameof(LoadedFamilyIntegrityCheck.CheckAllFamiliesSlow)).AsNotSupported(); //TODO add impl after Lazy invocation feature
        manager.Define(nameof(WorksharingUtils.GetUserWorksetInfo)).AsNotSupported(); //TODO slow performance
        manager.Define(nameof(WorksharingUtils.RelinquishOwnership)).AsNotSupported();
        manager.Define(nameof(FabricationUtils.ExportToPCF)).AsNotSupported();
#if !REVIT2025_OR_GREATER
        manager.Define(nameof(MacroManager.GetMacroManager)).Register(() => Variants.Value(MacroManager.GetMacroManager(_document)));
#endif
#if REVIT2022_OR_GREATER
        manager.Define(nameof(TemporaryGraphicsManager.GetTemporaryGraphicsManager)).Register(() => Variants.Value(TemporaryGraphicsManager.GetTemporaryGraphicsManager(_document)));
#endif
#if REVIT2023_OR_GREATER
        manager.Define(nameof(AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager)).Register(() => Variants.Value(AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(_document)));
#endif
#if REVIT2026_OR_GREATER
        manager.Define(nameof(CoordinationModelLinkUtils.GetAllCoordinationModelInstanceIds)).Register(() => Variants.Value(CoordinationModelLinkUtils.GetAllCoordinationModelInstanceIds(_document)));
        manager.Define(nameof(CoordinationModelLinkUtils.GetAllCoordinationModelTypeIds)).Register(() => Variants.Value(CoordinationModelLinkUtils.GetAllCoordinationModelTypeIds(_document)));
        manager.Define(nameof(CoordinationModelLinkUtils.Link3DViewFromAutodeskDocs)).AsNotSupported();
        manager.Define(nameof(CoordinationModelLinkUtils.LinkCoordinationModelFromLocalPath)).AsNotSupported();
#endif
    }
}