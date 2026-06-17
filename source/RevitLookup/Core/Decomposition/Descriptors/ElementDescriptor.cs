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
using RevitLookup.Common.Extensions;
using RevitLookup.Core.Decomposition.Extensions;
using RevitLookup.UI.Framework.Extensions;
using RevitLookup.Utils;
using ContextMenu = System.Windows.Controls.ContextMenu;
#if REVIT2024_OR_GREATER
using Autodesk.Revit.DB.Structure;
#endif
#if REVIT2026_OR_GREATER
using Autodesk.Revit.DB.ExternalData;
#endif

namespace RevitLookup.Core.Decomposition.Descriptors;

public partial class ElementDescriptor : ResolvingDescriptor, IDescriptorConfigurator, IContextMenuConnector
{
    private readonly Element _element;

    public ElementDescriptor(Element element)
    {
        _element = element;
        Name = element.Name == string.Empty ? $"ID{element.Id}" : $"{element.Name}, ID{element.Id}";
    }

    public virtual void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(Element.CanBeHidden)).Resolve(() => Variants.Value(_element.CanBeHidden(RevitContext.ActiveView), "Active view"));
        configuration.Member(nameof(Element.IsHidden)).Resolve(() => Variants.Value(_element.IsHidden(RevitContext.ActiveView), "Active view"));
        configuration.Member(nameof(Element.GetDependentElements)).Resolve(() => _element.GetDependentElements(null));
        configuration.Member(nameof(Element.GetMaterialIds)).Resolve(ResolveGetMaterialIds);
        configuration.Member(nameof(Element.GetMaterialArea)).Resolve(ResolveGetMaterialArea);
        configuration.Member(nameof(Element.GetMaterialVolume)).Resolve(ResolveGetMaterialVolume);
        configuration.Member(nameof(Element.GetEntity)).Resolve(ResolveGetEntity);
        configuration.Member(nameof(Element.GetPhaseStatus)).Resolve(() => ResolvePhases(_element.Document.Phases, _element.GetPhaseStatus));
        configuration.Member(nameof(Element.IsPhaseCreatedValid)).Resolve(() => ResolvePhases(_element.Document.Phases, _element.IsPhaseCreatedValid));
        configuration.Member(nameof(Element.IsPhaseDemolishedValid)).Resolve(() => ResolvePhases(_element.Document.Phases, _element.IsPhaseDemolishedValid));
#if REVIT2022_OR_GREATER
        configuration.Member(nameof(Element.IsDemolishedPhaseOrderValid)).Resolve(() => ResolvePhases(_element.Document.Phases, _element.IsDemolishedPhaseOrderValid));
        configuration.Member(nameof(Element.IsCreatedPhaseOrderValid)).Resolve(() => ResolvePhases(_element.Document.Phases, _element.IsCreatedPhaseOrderValid));
#endif
        configuration.Member("BoundingBox").Resolve(ResolveBoundingBox);
        configuration.Member("Geometry").Resolve(ResolveGeometry);

        ConfigureExtensions(configuration);
        return;

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
                using (schema.GrantAccess())
                {
                    var entity = _element.GetEntity(schema);
                    if (!entity.IsValid()) continue;

                    variants.Add(entity, schema.SchemaName);
                }
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

    private void ConfigureExtensions(IMemberConfigurator configuration)
    {
        configuration.Extension(nameof(JoinGeometryUtils.GetJoinedElements)).Register(() => JoinGeometryUtils.GetJoinedElements(_element.Document, _element));
        configuration.Extension(nameof(SolidSolidCutUtils.GetCuttingSolids)).Register(() => SolidSolidCutUtils.GetCuttingSolids(_element));
        configuration.Extension(nameof(SolidSolidCutUtils.GetSolidsBeingCut)).Register(() => SolidSolidCutUtils.GetSolidsBeingCut(_element));
        configuration.Extension(nameof(SolidSolidCutUtils.IsAllowedForSolidCut)).Register(() => SolidSolidCutUtils.IsAllowedForSolidCut(_element));
        configuration.Extension(nameof(SolidSolidCutUtils.IsElementFromAppropriateContext)).Register(() => SolidSolidCutUtils.IsElementFromAppropriateContext(_element));
        configuration.Extension(nameof(WorksharingUtils.GetCheckoutStatus)).Register(() => WorksharingUtils.GetCheckoutStatus(_element.Document, _element.Id));
        configuration.Extension(nameof(WorksharingUtils.GetWorksharingTooltipInfo)).Register(() => WorksharingUtils.GetWorksharingTooltipInfo(_element.Document, _element.Id));
        configuration.Extension(nameof(WorksharingUtils.GetModelUpdatesStatus)).Register(() => WorksharingUtils.GetModelUpdatesStatus(_element.Document, _element.Id));
        configuration.Extension("IsValidForCreateParts").Register(() => PartUtils.AreElementsValidForCreateParts(_element.Document, [_element.Id]));
        configuration.Extension(nameof(DirectContext3DDocumentUtils.IsADirectContext3DHandleInstance)).Register(() => DirectContext3DDocumentUtils.IsADirectContext3DHandleInstance(_element.Document, _element.Id));
        configuration.Extension(nameof(DirectContext3DDocumentUtils.IsADirectContext3DHandleType)).Register(() => DirectContext3DDocumentUtils.IsADirectContext3DHandleType(_element.Document, _element.Id));
        configuration.Extension("IsCategorySupportedByElementIntersectsFilter").Register(() => ElementIntersectsFilter.IsCategorySupported(_element));
        configuration.Extension("IsSupportedByElementIntersectsFilter").Register(() => ElementIntersectsFilter.IsElementSupported(_element));
        configuration.Extension("ExportId").Register(() => ExportUtils.GetExportId(_element.Document, _element.Id));
        configuration.Extension(nameof(ExternalFileUtils.GetExternalFileReference)).Register(() => ExternalFileUtils.GetExternalFileReference(_element.Document, _element.Id));
        configuration.Extension(nameof(ExternalFileUtils.IsExternalFileReference)).Register(() => ExternalFileUtils.IsExternalFileReference(_element.Document, _element.Id));
        configuration.Extension(nameof(InstanceVoidCutUtils.CanBeCutWithVoid)).Register(() => InstanceVoidCutUtils.CanBeCutWithVoid(_element));
        configuration.Extension(nameof(InstanceVoidCutUtils.GetCuttingVoidInstances)).Register(() => InstanceVoidCutUtils.GetCuttingVoidInstances(_element));
        configuration.Extension(nameof(PartUtils.GetAssociatedParts)).Register(() => PartUtils.GetAssociatedParts(_element.Document, _element.Id, true, true));
        configuration.Extension(nameof(PartUtils.HasAssociatedParts)).Register(() => PartUtils.HasAssociatedParts(_element.Document, _element.Id));
        configuration.Extension("CanBeMirrored").Register(() => ElementTransformUtils.CanMirrorElement(_element.Document, _element.Id));
        configuration.Extension("CanBeDeleted").Register(() => DocumentValidation.CanDeleteElement(_element.Document, _element.Id));
        configuration.Extension(nameof(InstanceVoidCutUtils.AddInstanceVoidCut)).NotSupported();
        configuration.Extension(nameof(InstanceVoidCutUtils.InstanceVoidCutExists)).NotSupported();
        configuration.Extension(nameof(InstanceVoidCutUtils.RemoveInstanceVoidCut)).NotSupported();
        configuration.Extension(nameof(ParameterFilterUtilities.IsParameterApplicable)).NotSupported();
        configuration.Extension(nameof(SolidSolidCutUtils.AddCutBetweenSolids)).NotSupported();
        configuration.Extension(nameof(SolidSolidCutUtils.RemoveCutBetweenSolids)).NotSupported();
        configuration.Extension(nameof(SolidSolidCutUtils.CanElementCutElement)).NotSupported();
        configuration.Extension(nameof(SolidSolidCutUtils.CutExistsBetweenElements)).NotSupported();
        configuration.Extension(nameof(SolidSolidCutUtils.SplitFacesOfCuttingSolid)).NotSupported();
        configuration.Extension(nameof(JoinGeometryUtils.AreElementsJoined)).NotSupported();
        configuration.Extension(nameof(JoinGeometryUtils.IsCuttingElementInJoin)).NotSupported();
        configuration.Extension(nameof(JoinGeometryUtils.JoinGeometry)).NotSupported();
        configuration.Extension(nameof(JoinGeometryUtils.SwitchJoinOrder)).NotSupported();
        configuration.Extension(nameof(JoinGeometryUtils.UnjoinGeometry)).NotSupported();
        configuration.Extension("Copy").Map(nameof(ElementTransformUtils.CopyElement)).NotSupported();
        configuration.Extension("Mirror").Map(nameof(ElementTransformUtils.MirrorElement)).NotSupported();
        configuration.Extension("Move").Map(nameof(ElementTransformUtils.MoveElement)).NotSupported();
        configuration.Extension("Rotate").Map(nameof(ElementTransformUtils.RotateElement)).NotSupported();

        var isDetailElement = SafeEvaluate(() => DetailElementOrderUtils.IsDetailElement(_element.Document, _element.Document.ActiveView, _element.Id));
        configuration.Extension(nameof(DetailElementOrderUtils.IsDetailElement)).Register(() => isDetailElement);

        if (isDetailElement)
        {
            configuration.Extension(nameof(DetailElementOrderUtils.BringForward)).NotSupported();
            configuration.Extension(nameof(DetailElementOrderUtils.BringToFront)).NotSupported();
            configuration.Extension(nameof(DetailElementOrderUtils.SendBackward)).NotSupported();
            configuration.Extension(nameof(DetailElementOrderUtils.SendToBack)).NotSupported();
        }

#if REVIT2024_OR_GREATER
        var isBendingDetail = SafeEvaluate(() => RebarBendingDetail.IsBendingDetail(_element));
        configuration.Extension(nameof(RebarBendingDetail.IsBendingDetail)).Register(() => isBendingDetail);

        if (isBendingDetail)
        {
            configuration.Extension("GetBendingDetailHost").Register(() => RebarBendingDetail.GetHost(_element));
            configuration.Extension("GetBendingDetailPosition").Register(() => RebarBendingDetail.GetPosition(_element));
            configuration.Extension("GetBendingDetailRotation").Register(() => RebarBendingDetail.GetRotation(_element));
#if REVIT2025_OR_GREATER
            configuration.Extension(nameof(RebarBendingDetail.IsRealisticBendingDetail)).Register(() => RebarBendingDetail.IsRealisticBendingDetail(_element));
            configuration.Extension(nameof(RebarBendingDetail.IsSchematicBendingDetail)).Register(() => RebarBendingDetail.IsSchematicBendingDetail(_element));
            configuration.Extension("GetBendingDetailHosts").Register(() => RebarBendingDetail.GetHosts(_element));
            configuration.Extension("GetBendingDetailTagRelativePosition").Register(() => RebarBendingDetail.GetTagRelativePosition(_element));
            configuration.Extension("GetBendingDetailTagRelativeRotation").Register(() => RebarBendingDetail.GetTagRelativeRotation(_element));
#endif
        }
#endif
#if REVIT2025_OR_GREATER
        var isMultiAlignSupported = SafeEvaluate(() => AnnotationMultipleAlignmentUtils.ElementSupportsMultiAlign(_element));
        configuration.Extension("IsMultiAlignSupported").Register(() => isMultiAlignSupported);

        if (isMultiAlignSupported)
        {
            configuration.Extension(nameof(AnnotationMultipleAlignmentUtils.GetAnnotationOutlineWithoutLeaders)).Register(() => AnnotationMultipleAlignmentUtils.GetAnnotationOutlineWithoutLeaders(_element));
            configuration.Extension(nameof(AnnotationMultipleAlignmentUtils.MoveWithAnchoredLeaders)).NotSupported();
        }
#endif
#if REVIT2026_OR_GREATER

        if (RevitApiContext.Application.Version.Minor >= 3)
        {
            ConfigureCoordinationModelExtensions(configuration);
        }
#endif
    }

#if REVIT2026_OR_GREATER

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ConfigureCoordinationModelExtensions(IMemberConfigurator configuration)
    {
        var isCoordinationModelInstance = SafeEvaluate(() => CoordinationModelLinkUtils.IsCoordinationModelInstance(_element.Document, _element));
        configuration.Extension(nameof(CoordinationModelLinkUtils.IsCoordinationModelInstance)).Register(() => isCoordinationModelInstance);

        if (isCoordinationModelInstance)
        {
            configuration.Extension(nameof(CoordinationModelLinkUtils.GetAllPropertiesForReferenceInsideCoordinationModel)).NotSupported();
            configuration.Extension(nameof(CoordinationModelLinkUtils.GetCategoryForReferenceInsideCoordinationModel)).NotSupported();
            configuration.Extension(nameof(CoordinationModelLinkUtils.GetVisibilityOverrideForReferenceInsideCoordinationModel)).NotSupported();
            configuration.Extension(nameof(CoordinationModelLinkUtils.SetVisibilityOverrideForReferenceInsideCoordinationModel)).NotSupported();
            configuration.Extension("GetCoordinationModelVisibilityOverride").Map(nameof(CoordinationModelLinkUtils.GetVisibilityOverride)).NotSupported();
            configuration.Extension("SetCoordinationModelVisibilityOverride").Map(nameof(CoordinationModelLinkUtils.SetVisibilityOverride)).NotSupported();
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

    protected static IVariant ResolveCategories<TResult>(CategoryNameMap categories, Func<ElementId, TResult> selector)
    {
        var variants = Variants.Values<TResult>(categories.Size);
        var simple = typeof(TResult).IsPrimitiveType();
        foreach (Category category in categories)
        {
            var result = selector(category.Id);
            variants.Add(result, simple ? $"{category.Name}: {result}" : category.Name);
        }

        return variants.Consume();
    }

    private static IVariant ResolvePhases<TResult>(PhaseArray phases, Func<ElementId, TResult> selector)
    {
        var variants = Variants.Values<TResult>(phases.Size);
        foreach (Phase phase in phases)
        {
            var result = selector(phase.Id);
            variants.Add(result, $"{phase.Name}: {result}");
        }

        return variants.Consume();
    }
}