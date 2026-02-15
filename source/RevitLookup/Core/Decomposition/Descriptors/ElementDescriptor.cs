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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Autodesk.Revit.DB.ExtensibleStorage;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Configuration;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.ViewModels.Decomposition;
using RevitLookup.Services.Application;
using RevitLookup.UI.Framework.Extensions;

namespace RevitLookup.Core.Decomposition.Descriptors;

public class ElementDescriptor : Descriptor, IDescriptorResolver, IDescriptorExtension, IContextMenuConnector
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
            nameof(Element.CanBeHidden) => ResolveCanBeHidden,
            nameof(Element.IsHidden) => ResolveIsHidden,
            nameof(Element.GetDependentElements) => ResolveGetDependentElements,
            nameof(Element.GetMaterialIds) => ResolveGetMaterialIds,
            nameof(Element.GetMaterialArea) => ResolveGetMaterialArea,
            nameof(Element.GetMaterialVolume) => ResolveGetMaterialVolume,
            nameof(Element.GetEntity) => ResolveGetEntity,
            nameof(Element.GetPhaseStatus) => ResolvePhaseStatus,
            nameof(Element.IsPhaseCreatedValid) => ResolveIsPhaseCreatedValid,
            nameof(Element.IsPhaseDemolishedValid) => ResolveIsPhaseDemolishedValid,
#if REVIT2022_OR_GREATER
            nameof(Element.IsDemolishedPhaseOrderValid) => ResolveIsDemolishedPhaseOrderValid,
            nameof(Element.IsCreatedPhaseOrderValid) => ResolveIsCreatedPhaseOrderValid,
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

        IVariant ResolveCanBeHidden()
        {
            return Variants.Value(_element.CanBeHidden(RevitContext.ActiveView), "Active view");
        }

        IVariant ResolveIsHidden()
        {
            return Variants.Value(_element.IsHidden(RevitContext.ActiveView), "Active view");
        }

        IVariant ResolveGetDependentElements()
        {
            return Variants.Value(_element.GetDependentElements(null));
        }

        IVariant ResolvePhaseStatus()
        {
            var phases = _element.Document.Phases;
            var variants = Variants.Values<ElementOnPhaseStatus>(phases.Size);
            foreach (Phase phase in phases)
            {
                var result = _element.GetPhaseStatus(phase.Id);
                variants.Add(result, $"{phase.Name}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolveIsPhaseCreatedValid()
        {
            var phases = _element.Document.Phases;
            var variants = Variants.Values<bool>(phases.Size);
            foreach (Phase phase in phases)
            {
                var result = _element.IsPhaseCreatedValid(phase.Id);
                variants.Add(result, $"{phase.Name}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolveIsPhaseDemolishedValid()
        {
            var phases = _element.Document.Phases;
            var variants = Variants.Values<bool>(phases.Size);
            foreach (Phase phase in phases)
            {
                var result = _element.IsPhaseDemolishedValid(phase.Id);
                variants.Add(result, $"{phase.Name}: {result}");
            }

            return variants.Consume();
        }

#if REVIT2022_OR_GREATER
        IVariant ResolveIsCreatedPhaseOrderValid()
        {
            var phases = _element.Document.Phases;
            var variants = Variants.Values<bool>(phases.Size);
            foreach (Phase phase in phases)
            {
                var result = _element.IsCreatedPhaseOrderValid(phase.Id);
                variants.Add(result, $"{phase.Name}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolveIsDemolishedPhaseOrderValid()
        {
            var phases = _element.Document.Phases;
            var variants = Variants.Values<bool>(phases.Size);
            foreach (Phase phase in phases)
            {
                var result = _element.IsDemolishedPhaseOrderValid(phase.Id);
                variants.Add(result, $"{phase.Name}: {result}");
            }

            return variants.Consume();
        }

#endif
    }

    public virtual void RegisterExtensions(IExtensionManager manager)
    {
        manager.Register(nameof(ElementTransformUtilsExtensions.CanBeMirrored), () => Variants.Value(ElementTransformUtils.CanMirrorElement(_element.Document, _element.Id)));
        manager.Register(nameof(JoinGeometryUtils.GetJoinedElements), () => Variants.Value(JoinGeometryUtils.GetJoinedElements(_element.Document, _element)));
        manager.Register(nameof(SolidSolidCutUtils.GetCuttingSolids), () => Variants.Value(SolidSolidCutUtils.GetCuttingSolids(_element)));
        manager.Register(nameof(SolidSolidCutUtils.GetSolidsBeingCut), () => Variants.Value(SolidSolidCutUtils.GetSolidsBeingCut(_element)));
        manager.Register(nameof(SolidSolidCutUtils.IsAllowedForSolidCut), () => Variants.Value(SolidSolidCutUtils.IsAllowedForSolidCut(_element)));
        manager.Register(nameof(SolidSolidCutUtils.IsElementFromAppropriateContext), () => Variants.Value(SolidSolidCutUtils.IsElementFromAppropriateContext(_element)));
        manager.Register(nameof(WorksharingUtils.GetCheckoutStatus), () => Variants.Value(WorksharingUtils.GetCheckoutStatus(_element.Document, _element.Id)));
        manager.Register(nameof(WorksharingUtils.GetWorksharingTooltipInfo), () => Variants.Value(WorksharingUtils.GetWorksharingTooltipInfo(_element.Document, _element.Id)));
        manager.Register(nameof(WorksharingUtils.GetModelUpdatesStatus), () => Variants.Value(WorksharingUtils.GetModelUpdatesStatus(_element.Document, _element.Id)));
        manager.Register(nameof(PartUtils.AreElementsValidForCreateParts), () => Variants.Value(PartUtils.AreElementsValidForCreateParts(_element.Document, [_element.Id])));
        manager.Register(nameof(DocumentValidation.CanDeleteElement), () => Variants.Value(DocumentValidation.CanDeleteElement(_element.Document, _element.Id)));
        manager.Register(nameof(AnnotationMultipleAlignmentUtils.ElementSupportsMultiAlign), () => Variants.Value(AnnotationMultipleAlignmentUtils.ElementSupportsMultiAlign(_element)));
        manager.Register(nameof(AnnotationMultipleAlignmentUtils.GetAnnotationOutlineWithoutLeaders), () => Variants.Value(AnnotationMultipleAlignmentUtils.GetAnnotationOutlineWithoutLeaders(_element)));
        manager.Register(nameof(AnnotationMultipleAlignmentUtils.MoveWithAnchoredLeaders), Variants.NotSupported);
    }

    public virtual void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
    {
        contextMenu.AddMenuItem("SelectMenuItem")
            .SetCommand(_element, SelectFace)
            .SetShortcut(Key.F6);

        if (_element is not ElementType && _element is not Family)
        {
            contextMenu.AddMenuItem("ShowMenuItem")
                .SetCommand(_element, ShowFace)
                .SetShortcut(Key.F7);
        }

        contextMenu.AddMenuItem("DeleteMenuItem")
            .SetCommand(_element, DeleteElement)
            .SetAvailability(DocumentValidation.CanDeleteElement(_element.Document, _element.Id))
            .SetShortcut(Key.Delete);

        void SelectFace(Element element)
        {
            if (RevitContext.ActiveUiDocument is null) return;
            if (!element.IsValidObject) return;

            EventHandlers.ActionEventHandler.Raise(_ => RevitContext.ActiveUiDocument.Selection.SetElementIds([element.Id]));
        }

        void ShowFace(Element element)
        {
            if (RevitContext.ActiveUiDocument is null) return;
            if (!element.IsValidObject) return;

            EventHandlers.ActionEventHandler.Raise(_ =>
            {
                RevitContext.ActiveUiDocument.ShowElements(element);
                RevitContext.ActiveUiDocument.Selection.SetElementIds([element.Id]);
            });
        }

        async Task DeleteElement(Element element)
        {
            if (RevitContext.ActiveUiDocument is null) return;

            ICollection<ElementId>? removedIds = [];
            var notificationService = serviceProvider.GetRequiredService<INotificationService>();
            try
            {
                await EventHandlers.AsyncEventHandler.RaiseAsync(_ =>
                {
                    using var transaction = new Transaction(element.Document);
                    transaction.Start($"Delete {element.Name}");

                    try
                    {
                        removedIds = element.Document.Delete(element.Id);
                        transaction.Commit();

                        if (transaction.GetStatus() == TransactionStatus.RolledBack) throw new OperationCanceledException("Element deletion cancelled by user");
                    }
                    catch
                    {
                        if (!transaction.HasEnded()) transaction.RollBack();
                        throw;
                    }
                });

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
}