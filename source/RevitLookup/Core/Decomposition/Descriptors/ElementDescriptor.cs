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
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Autodesk.Revit.DB.DirectContext3D;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nice3point.Revit.Toolkit.External;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.ViewModels.Decomposition;
using RevitLookup.Core.Decomposition.Extensions;
using RevitLookup.UI.Framework.Extensions;
using ContextMenu = System.Windows.Controls.ContextMenu;
#if REVIT2024_OR_GREATER
using Autodesk.Revit.DB.Structure;
#endif

#if REVIT2026_OR_GREATER
using Autodesk.Revit.DB.ExternalData;
#endif

namespace RevitLookup.Core.Decomposition.Descriptors;

public partial class ElementDescriptor : Descriptor, IDescriptorResolver, IDescriptorExtension, IContextMenuConnector
{
    private readonly Element _element;

    public ElementDescriptor(Element element)
    {
        _element = element;
        Name = element.Name == string.Empty ? $"ID{element.Id}" : $"{element.Name}, ID{element.Id}";
    }

    public virtual Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(Element.CanBeHidden) => () => Variants.Value(_element.CanBeHidden(RevitContext.ActiveView), "Active view"),
            nameof(Element.IsHidden) => () => Variants.Value(_element.IsHidden(RevitContext.ActiveView), "Active view"),
            nameof(Element.GetDependentElements) => () => Variants.Value(_element.GetDependentElements(null)),
            nameof(Element.GetMaterialIds) => ResolveGetMaterialIds,
            nameof(Element.GetMaterialArea) => ResolveGetMaterialArea,
            nameof(Element.GetMaterialVolume) => ResolveGetMaterialVolume,
            nameof(Element.GetEntity) => ResolveGetEntity,
            nameof(Element.GetPhaseStatus) => () => VariantsResolver.ResolvePhases(_element.Document.Phases, _element.GetPhaseStatus),
            nameof(Element.IsPhaseCreatedValid) => () => VariantsResolver.ResolvePhases(_element.Document.Phases, _element.IsPhaseCreatedValid),
            nameof(Element.IsPhaseDemolishedValid) => () => VariantsResolver.ResolvePhases(_element.Document.Phases, _element.IsPhaseDemolishedValid),
#if REVIT2022_OR_GREATER
            nameof(Element.IsDemolishedPhaseOrderValid) => () => VariantsResolver.ResolvePhases(_element.Document.Phases, _element.IsDemolishedPhaseOrderValid),
            nameof(Element.IsCreatedPhaseOrderValid) => () => VariantsResolver.ResolvePhases(_element.Document.Phases, _element.IsCreatedPhaseOrderValid),
#endif
            "BoundingBox" => ResolveBoundingBox,
            "Geometry" => ResolveGeometry,
            _ => null
        };

        IVariant ResolveGetMaterialArea()
        {
            var geometryMaterials = _element.GetMaterialIds(false);
            var paintMaterials = _element.GetMaterialIds(true);

            var capacity = geometryMaterials.Count + paintMaterials.Count;
            if (capacity == 0) return Variants.Empty<KeyValuePair<ElementId, double>>();

            var variants = Variants.Values<KeyValuePair<ElementId, double>>(capacity);
            foreach (var materialId in geometryMaterials)
            {
                var area = _element.GetMaterialArea(materialId, false);
                variants.Add(new KeyValuePair<ElementId, double>(materialId, area));
            }

            foreach (var materialId in paintMaterials)
            {
                var area = _element.GetMaterialArea(materialId, true);
                variants.Add(new KeyValuePair<ElementId, double>(materialId, area));
            }

            return variants.Consume();
        }

        IVariant ResolveGetMaterialVolume()
        {
            var geometryMaterials = _element.GetMaterialIds(false);

            if (geometryMaterials.Count == 0) return Variants.Empty<KeyValuePair<ElementId, double>>();

            var variants = Variants.Values<KeyValuePair<ElementId, double>>(geometryMaterials.Count);
            foreach (var materialId in geometryMaterials)
            {
                var area = _element.GetMaterialVolume(materialId);
                variants.Add(new KeyValuePair<ElementId, double>(materialId, area));
            }

            return variants.Consume();
        }

        IVariant ResolveGetEntity()
        {
            var schemas = Schema.ListSchemas();
            var variants = Variants.Values<Entity>(schemas.Count);
            foreach (var schema in schemas)
            {
                if (!schema.ReadAccessGranted()) continue;

                var entity = _element.GetEntity(schema);
                if (!entity.IsValid()) continue;

                variants.Add(entity, schema.SchemaName);
            }

            return variants.Consume();
        }

        IVariant ResolveGeometry()
        {
            return Variants.Values<GeometryElement>(10)
                .Add(_element.get_Geometry(new Options
                {
                    View = RevitContext.ActiveView,
                    ComputeReferences = true
                }), "Active view")
                .Add(_element.get_Geometry(new Options
                {
                    View = RevitContext.ActiveView,
                    IncludeNonVisibleObjects = true,
                    ComputeReferences = true
                }), "Active view, including non-visible objects")
                .Add(_element.get_Geometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Coarse,
                    ComputeReferences = true
                }), "Model, coarse detail level")
                .Add(_element.get_Geometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Fine,
                    ComputeReferences = true
                }), "Model, fine detail level")
                .Add(_element.get_Geometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Medium,
                    ComputeReferences = true
                }), "Model, medium detail level")
                .Add(_element.get_Geometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Undefined,
                    ComputeReferences = true
                }), "Model, undefined detail level")
                .Add(_element.get_Geometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Coarse,
                    IncludeNonVisibleObjects = true,
                    ComputeReferences = true
                }), "Model, coarse detail level, including non-visible objects")
                .Add(_element.get_Geometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Fine,
                    IncludeNonVisibleObjects = true,
                    ComputeReferences = true
                }), "Model, fine detail level, including non-visible objects")
                .Add(_element.get_Geometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Medium,
                    IncludeNonVisibleObjects = true,
                    ComputeReferences = true
                }), "Model, medium detail level, including non-visible objects")
                .Add(_element.get_Geometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Undefined,
                    IncludeNonVisibleObjects = true,
                    ComputeReferences = true
                }), "Model, undefined detail level, including non-visible objects")
                .Consume();
        }

        IVariant ResolveGetMaterialIds()
        {
            return Variants.Values<ICollection<ElementId>>(2)
                .Add(_element.GetMaterialIds(true), "Paint materials")
                .Add(_element.GetMaterialIds(false), "Geometry and compound structure materials")
                .Consume();
        }

        IVariant ResolveBoundingBox()
        {
            return Variants.Values<BoundingBoxXYZ>(2)
                .Add(_element.get_BoundingBox(null), "Model")
                .Add(_element.get_BoundingBox(RevitContext.ActiveView), "Active view")
                .Consume();
        }
    }

    public virtual void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define(nameof(JoinGeometryUtils.GetJoinedElements)).Register(() => Variants.Value(JoinGeometryUtils.GetJoinedElements(_element.Document, _element)));
        manager.Define(nameof(SolidSolidCutUtils.GetCuttingSolids)).Register(() => Variants.Value(SolidSolidCutUtils.GetCuttingSolids(_element)));
        manager.Define(nameof(SolidSolidCutUtils.GetSolidsBeingCut)).Register(() => Variants.Value(SolidSolidCutUtils.GetSolidsBeingCut(_element)));
        manager.Define(nameof(SolidSolidCutUtils.IsAllowedForSolidCut)).Register(() => Variants.Value(SolidSolidCutUtils.IsAllowedForSolidCut(_element)));
        manager.Define(nameof(SolidSolidCutUtils.IsElementFromAppropriateContext)).Register(() => Variants.Value(SolidSolidCutUtils.IsElementFromAppropriateContext(_element)));
        manager.Define(nameof(WorksharingUtils.GetCheckoutStatus)).Register(() => Variants.Value(WorksharingUtils.GetCheckoutStatus(_element.Document, _element.Id)));
        manager.Define(nameof(WorksharingUtils.GetWorksharingTooltipInfo)).Register(() => Variants.Value(WorksharingUtils.GetWorksharingTooltipInfo(_element.Document, _element.Id)));
        manager.Define(nameof(WorksharingUtils.GetModelUpdatesStatus)).Register(() => Variants.Value(WorksharingUtils.GetModelUpdatesStatus(_element.Document, _element.Id)));
        manager.Define("IsValidForCreateParts").Register(() => Variants.Value(PartUtils.AreElementsValidForCreateParts(_element.Document, [_element.Id])));
        manager.Define(nameof(DirectContext3DDocumentUtils.IsADirectContext3DHandleInstance)).Register(() => Variants.Value(DirectContext3DDocumentUtils.IsADirectContext3DHandleInstance(_element.Document, _element.Id)));
        manager.Define(nameof(DirectContext3DDocumentUtils.IsADirectContext3DHandleType)).Register(() => Variants.Value(DirectContext3DDocumentUtils.IsADirectContext3DHandleType(_element.Document, _element.Id)));
        manager.Define("IsCategorySupportedByElementIntersectsFilter").Register(() => Variants.Value(ElementIntersectsFilter.IsCategorySupported(_element)));
        manager.Define("IsSupportedByElementIntersectsFilter").Register(() => Variants.Value(ElementIntersectsFilter.IsElementSupported(_element)));
        manager.Define("ExportId").Register(() => Variants.Value(ExportUtils.GetExportId(_element.Document, _element.Id)));
        manager.Define(nameof(ExternalFileUtils.GetExternalFileReference)).Register(() => Variants.Value(ExternalFileUtils.GetExternalFileReference(_element.Document, _element.Id)));
        manager.Define(nameof(ExternalFileUtils.IsExternalFileReference)).Register(() => Variants.Value(ExternalFileUtils.IsExternalFileReference(_element.Document, _element.Id)));
        manager.Define(nameof(InstanceVoidCutUtils.CanBeCutWithVoid)).Register(() => Variants.Value(InstanceVoidCutUtils.CanBeCutWithVoid(_element)));
        manager.Define(nameof(InstanceVoidCutUtils.GetCuttingVoidInstances)).Register(() => Variants.Value(InstanceVoidCutUtils.GetCuttingVoidInstances(_element)));
        manager.Define(nameof(PartUtils.GetAssociatedParts)).Register(() => Variants.Value(PartUtils.GetAssociatedParts(_element.Document, _element.Id, true, true)));
        manager.Define(nameof(PartUtils.HasAssociatedParts)).Register(() => Variants.Value(PartUtils.HasAssociatedParts(_element.Document, _element.Id)));
        manager.Define("CanBeMirrored").Register(() => Variants.Value(ElementTransformUtils.CanMirrorElement(_element.Document, _element.Id)));
        manager.Define("CanBeDeleted").Register(() => Variants.Value(DocumentValidation.CanDeleteElement(_element.Document, _element.Id)));
        manager.Define(nameof(InstanceVoidCutUtils.AddInstanceVoidCut)).AsNotSupported();
        manager.Define(nameof(InstanceVoidCutUtils.InstanceVoidCutExists)).AsNotSupported();
        manager.Define(nameof(InstanceVoidCutUtils.RemoveInstanceVoidCut)).AsNotSupported();
        manager.Define(nameof(ParameterFilterUtilities.IsParameterApplicable)).AsNotSupported();
        manager.Define(nameof(SolidSolidCutUtils.AddCutBetweenSolids)).AsNotSupported();
        manager.Define(nameof(SolidSolidCutUtils.RemoveCutBetweenSolids)).AsNotSupported();
        manager.Define(nameof(SolidSolidCutUtils.CanElementCutElement)).AsNotSupported();
        manager.Define(nameof(SolidSolidCutUtils.CutExistsBetweenElements)).AsNotSupported();
        manager.Define(nameof(SolidSolidCutUtils.SplitFacesOfCuttingSolid)).AsNotSupported();
        manager.Define(nameof(JoinGeometryUtils.AreElementsJoined)).AsNotSupported();
        manager.Define(nameof(JoinGeometryUtils.IsCuttingElementInJoin)).AsNotSupported();
        manager.Define(nameof(JoinGeometryUtils.JoinGeometry)).AsNotSupported();
        manager.Define(nameof(JoinGeometryUtils.SwitchJoinOrder)).AsNotSupported();
        manager.Define(nameof(JoinGeometryUtils.UnjoinGeometry)).AsNotSupported();
        manager.Define("Copy").Map(nameof(ElementTransformUtils.CopyElement)).AsNotSupported();
        manager.Define("Mirror").Map(nameof(ElementTransformUtils.MirrorElement)).AsNotSupported();
        manager.Define("Move").Map(nameof(ElementTransformUtils.MoveElement)).AsNotSupported();
        manager.Define("Rotate").Map(nameof(ElementTransformUtils.RotateElement)).AsNotSupported();

        if (manager.Define(nameof(DetailElementOrderUtils.IsDetailElement)).TryRegister(() => Variants.Value(DetailElementOrderUtils.IsDetailElement(_element.Document, _element.Document.ActiveView, _element.Id))))
        {
            manager.Define(nameof(DetailElementOrderUtils.BringForward)).AsNotSupported();
            manager.Define(nameof(DetailElementOrderUtils.BringToFront)).AsNotSupported();
            manager.Define(nameof(DetailElementOrderUtils.SendBackward)).AsNotSupported();
            manager.Define(nameof(DetailElementOrderUtils.SendToBack)).AsNotSupported();
        }

#if REVIT2024_OR_GREATER
        if (manager.Define(nameof(RebarBendingDetail.IsBendingDetail)).TryRegister(() => Variants.Value(RebarBendingDetail.IsBendingDetail(_element))))
        {
            manager.Define("GetBendingDetailHost").Register(() => Variants.Value(RebarBendingDetail.GetHost(_element)));
            manager.Define("GetBendingDetailPosition").Register(() => Variants.Value(RebarBendingDetail.GetPosition(_element)));
            manager.Define("GetBendingDetailRotation").Register(() => Variants.Value(RebarBendingDetail.GetRotation(_element)));
#if REVIT2025_OR_GREATER
            manager.Define(nameof(RebarBendingDetail.IsRealisticBendingDetail)).Register(() => Variants.Value(RebarBendingDetail.IsRealisticBendingDetail(_element)));
            manager.Define(nameof(RebarBendingDetail.IsSchematicBendingDetail)).Register(() => Variants.Value(RebarBendingDetail.IsSchematicBendingDetail(_element)));
            manager.Define("GetBendingDetailHosts").Register(() => Variants.Value(RebarBendingDetail.GetHosts(_element)));
            manager.Define("GetBendingDetailTagRelativePosition").Register(() => Variants.Value(RebarBendingDetail.GetTagRelativePosition(_element)));
            manager.Define("GetBendingDetailTagRelativeRotation").Register(() => Variants.Value(RebarBendingDetail.GetTagRelativeRotation(_element)));
#endif
        }
#endif
#if REVIT2025_OR_GREATER
        if (manager.Define("IsMultiAlignSupported").TryRegister(() => Variants.Value(AnnotationMultipleAlignmentUtils.ElementSupportsMultiAlign(_element))))
        {
            manager.Define(nameof(AnnotationMultipleAlignmentUtils.GetAnnotationOutlineWithoutLeaders)).Register(() => Variants.Value(AnnotationMultipleAlignmentUtils.GetAnnotationOutlineWithoutLeaders(_element)));
            manager.Define(nameof(AnnotationMultipleAlignmentUtils.MoveWithAnchoredLeaders)).AsNotSupported();
        }
#endif
#if REVIT2026_OR_GREATER

        if (RevitApiContext.Application.Version.Minor >= 3)
        {
            RegisterCoordinationModelExtensions(manager);
        }
#endif
    }
#if REVIT2026_OR_GREATER

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void RegisterCoordinationModelExtensions(IExtensionManager manager)
    {
        if (manager.Define(nameof(CoordinationModelLinkUtils.IsCoordinationModelInstance)).TryRegister(() => Variants.Value(CoordinationModelLinkUtils.IsCoordinationModelInstance(_element.Document, _element))))
        {
            manager.Define(nameof(CoordinationModelLinkUtils.GetAllPropertiesForReferenceInsideCoordinationModel)).AsNotSupported();
            manager.Define(nameof(CoordinationModelLinkUtils.GetCategoryForReferenceInsideCoordinationModel)).AsNotSupported();
            manager.Define(nameof(CoordinationModelLinkUtils.GetVisibilityOverrideForReferenceInsideCoordinationModel)).AsNotSupported();
            manager.Define(nameof(CoordinationModelLinkUtils.SetVisibilityOverrideForReferenceInsideCoordinationModel)).AsNotSupported();
            manager.Define("GetCoordinationModelVisibilityOverride").Map(nameof(CoordinationModelLinkUtils.GetVisibilityOverride)).AsNotSupported();
            manager.Define("SetCoordinationModelVisibilityOverride").Map(nameof(CoordinationModelLinkUtils.SetVisibilityOverride)).AsNotSupported();
        }
    }
#endif

    public virtual void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
    {
        contextMenu.AddMenuItem("SelectMenuItem")
            .SetCommand(_element, element => SelectElementEvent.Raise(element))
            .SetShortcut(Key.F6);

        if (_element is not ElementType && _element is not Family)
        {
            contextMenu.AddMenuItem("ShowMenuItem")
                .SetCommand(_element, element => ShowElementEvent.Raise(element))
                .SetShortcut(Key.F7);
        }

        contextMenu.AddMenuItem("DeleteMenuItem")
            .SetCommand(_element, element => DeleteElementAsync(element, serviceProvider, contextMenu))
            .SetAvailability(DocumentValidation.CanDeleteElement(_element.Document, _element.Id))
            .SetShortcut(Key.Delete);
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private static void SelectElement(UIApplication application, Element element)
    {
        if (application.ActiveUIDocument is null) return;
        if (!element.IsValidObject) return;

        application.ActiveUIDocument.Selection.SetElementIds([element.Id]);
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private static void ShowElement(UIApplication application, Element element)
    {
        if (application.ActiveUIDocument is null) return;
        if (!element.IsValidObject) return;

        application.ActiveUIDocument.ShowElements(element);
        application.ActiveUIDocument.Selection.SetElementIds([element.Id]);
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private static ICollection<ElementId> DeleteElement(UIApplication application, Element element)
    {
        if (application.ActiveUIDocument is null) throw new InvalidOperationException("No active document");

        using var transaction = new Transaction(element.Document);
        transaction.Start($"Delete {element.Name}");

        try
        {
            var deletedIds = element.Document.Delete(element.Id);
            transaction.Commit();

            if (transaction.GetStatus() == TransactionStatus.RolledBack) throw new OperationCanceledException("Element deletion cancelled by user");

            return deletedIds;
        }
        catch
        {
            if (!transaction.HasEnded()) transaction.RollBack();
            throw;
        }
    }

    private static async Task DeleteElementAsync(Element element, IServiceProvider serviceProvider, ContextMenu contextMenu)
    {
        var notificationService = serviceProvider.GetRequiredService<INotificationService>();
        try
        {
            var removedIds = await DeleteElementAsyncEvent.RaiseAsync(element);

            var summaryViewModel = serviceProvider.GetRequiredService<IDecompositionSummaryViewModel>();
            var placementTarget = (FrameworkElement) contextMenu.PlacementTarget;
            summaryViewModel.RemoveItem(placementTarget.DataContext);

            notificationService.ShowSuccess("Success", $"{removedIds.Count} elements completely removed from the Revit database");
        }
        catch (OperationCanceledException exception)
        {
            notificationService.ShowWarning("Warning", exception.Message);
        }
        catch (Exception exception)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<ElementDescriptor>>();

            logger.LogError(exception, "Element deletion error");
            notificationService.ShowError("Element deletion error", exception.Message);
        }
    }
}