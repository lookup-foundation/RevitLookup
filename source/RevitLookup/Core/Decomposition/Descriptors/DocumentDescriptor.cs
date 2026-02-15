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

using System.Reflection;
using Autodesk.Revit.DB.ExternalData;
using Autodesk.Revit.DB.Lighting;
using Autodesk.Revit.DB.Structure;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
#if !REVIT2025_OR_GREATER
using Autodesk.Revit.DB.Macros;
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
#if REVIT2024_OR_GREATER
            nameof(Document.GetUnusedElements) => ResolveGetUnusedElements,
            nameof(Document.GetAllUnusedElements) => ResolveGetAllUnusedElements,
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
#if REVIT2024_OR_GREATER

        IVariant ResolveGetUnusedElements()
        {
            return Variants.Value(_document.GetUnusedElements(new HashSet<ElementId>()));
        }

        IVariant ResolveGetAllUnusedElements()
        {
            return Variants.Value(_document.GetAllUnusedElements(new HashSet<ElementId>()));
        }
#endif
    }

    public void RegisterExtensions(IExtensionManager manager)
    {
        manager.Register(nameof(GlobalParametersManager.GetAllGlobalParameters), () => Variants.Value(GlobalParametersManager.GetAllGlobalParameters(_document)));
        manager.Register(nameof(LightGroupManager.GetLightGroupManager), () => Variants.Value(LightGroupManager.GetLightGroupManager(_document)));
#if !REVIT2025_OR_GREATER
        manager.Register(nameof(MacroManager.GetMacroManager), () => Variants.Value(MacroManager.GetMacroManager(_document)));
#endif
#if REVIT2022_OR_GREATER
        manager.Register(nameof(TemporaryGraphicsManager.GetTemporaryGraphicsManager), () => Variants.Value(TemporaryGraphicsManager.GetTemporaryGraphicsManager(_document)));
#endif
#if REVIT2023_OR_GREATER
        manager.Register(nameof(AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager),
            () => Variants.Value(AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(_document)));
#endif
        if (_document.IsFamilyDocument)
        {
            manager.Register(nameof(FamilySizeTableManager.CreateFamilySizeTableManager), () =>
            {
                var familyTableId = new ElementId(BuiltInParameter.RBS_LOOKUP_TABLE_NAME);
                return Variants.Value(FamilySizeTableManager.GetFamilySizeTableManager(_document, familyTableId));
            });

            manager.Register(nameof(LightFamily.GetLightFamily), () => Variants.Value(LightFamily.GetLightFamily(_document)));
        }

        manager.Register(nameof(AssemblyCodeTable.GetAssemblyCodeTable), () => Variants.Value(AssemblyCodeTable.GetAssemblyCodeTable(_document)));
        manager.Register(nameof(CoordinationModelLinkUtils.GetAllCoordinationModelInstanceIds), () => Variants.Value(CoordinationModelLinkUtils.GetAllCoordinationModelInstanceIds(_document)));
        manager.Register(nameof(CoordinationModelLinkUtils.GetAllCoordinationModelTypeIds), () => Variants.Value(CoordinationModelLinkUtils.GetAllCoordinationModelTypeIds(_document)));
        manager.Register(nameof(CoordinationModelLinkUtils.Link3DViewFromAutodeskDocs), Variants.NotSupported);
        manager.Register(nameof(CoordinationModelLinkUtils.LinkCoordinationModelFromLocalPath), Variants.NotSupported);
        manager.Register(nameof(ExportUtils.GetGBXMLDocumentId), () => Variants.Value(ExportUtils.GetGBXMLDocumentId(_document)));
        manager.Register(nameof(ExternalFileUtils.GetAllExternalFileReferences), () => Variants.Value(ExternalFileUtils.GetAllExternalFileReferences(_document)));
        manager.Register(nameof(ExternalResourceUtils.GetAllExternalResourceReferences), () => Variants.Value(ExternalResourceUtils.GetAllExternalResourceReferences(_document)));
        manager.Register(nameof(GlobalParametersManager.AreGlobalParametersAllowed), () => Variants.Value(GlobalParametersManager.AreGlobalParametersAllowed(_document)));
        manager.Register(nameof(GlobalParametersManager.GetGlobalParametersOrdered), () => Variants.Value(GlobalParametersManager.GetGlobalParametersOrdered(_document)));
        manager.Register(nameof(GlobalParametersManager.FindByName), Variants.NotSupported);
        manager.Register(nameof(GlobalParametersManager.IsUniqueName), Variants.NotSupported);
        manager.Register(nameof(GlobalParametersManager.SortParameters), Variants.Disabled);
        manager.Register(nameof(ParameterFilterUtilities.RemoveUnfilterableCategories), Variants.NotSupported);

        // Disabled: slow performance.
        // manager.Register(nameof(WorksharingUtils.GetUserWorksetInfo), context =>
        // {
        //     var modelPath = context.IsModelInCloud ? context.GetCloudModelPath() : context.GetWorksharingCentralModelPath();
        //     return WorksharingUtils.GetUserWorksetInfo(modelPath);
        // });
    }
}