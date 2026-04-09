// Copyright 2003-2024 by Autodesk, Inc.
// 
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
// 
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
// 
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.

using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Autodesk.Revit.DB.ExtensibleStorage;
using Microsoft.Extensions.DependencyInjection;
using RevitLookup.Services;
using RevitLookup.ViewModels.Contracts;
using RevitLookup.Views.Extensions;

namespace RevitLookup.Core.ComponentModel.Descriptors;

public class ElementDescriptor : Descriptor, IDescriptorResolver, IDescriptorConnector, IDescriptorExtension
{
    private readonly Element _element;

    public ElementDescriptor(Element element)
    {
        _element = element;
        Name = element.Name == string.Empty ? $"ID{element.Id}" : $"{element.Name}, ID{element.Id}";
    }

    public virtual Func<IVariants> Resolve(Document context, string target, ParameterInfo[] parameters)
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

        IVariants ResolveGetMaterialArea()
        {
            var geometryMaterials = _element.GetMaterialIds(false);
            var paintMaterials = _element.GetMaterialIds(true);

            var capacity = geometryMaterials.Count + paintMaterials.Count;
            var variants = new Variants<KeyValuePair<ElementId, double>>(capacity);
            if (capacity == 0) return variants;

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

            return variants;
        }

        IVariants ResolveGetMaterialVolume()
        {
            var geometryMaterials = _element.GetMaterialIds(false);

            var variants = new Variants<KeyValuePair<ElementId, double>>(geometryMaterials.Count);
            if (geometryMaterials.Count == 0) return variants;

            foreach (var materialId in geometryMaterials)
            {
                var area = _element.GetMaterialVolume(materialId);
                variants.Add(new KeyValuePair<ElementId, double>(materialId, area));
            }

            return variants;
        }

        IVariants ResolveGetEntity()
        {
            var schemas = Schema.ListSchemas();
            var variants = new Variants<Entity>(schemas.Count);
            foreach (var schema in schemas)
            {
                if (!schema.ReadAccessGranted()) continue;
                var entity = _element.GetEntity(schema);
                if (!entity.IsValid()) continue;

                variants.Add(entity, schema.SchemaName);
            }

            return variants;
        }

        IVariants ResolveGeometry()
        {
            return new Variants<GeometryElement>(10)
                .Add(_element.get_Geometry(new Options
                {
                    View = Context.ActiveView,
                    ComputeReferences = true
                }), "Active view")
                .Add(_element.get_Geometry(new Options
                {
                    View = Context.ActiveView,
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
                }), "Model, undefined detail level, including non-visible objects");
        }

        IVariants ResolveGetMaterialIds()
        {
            return new Variants<ICollection<ElementId>>(2)
                .Add(_element.GetMaterialIds(true), "Paint materials")
                .Add(_element.GetMaterialIds(false), "Geometry and compound structure materials");
        }

        IVariants ResolveBoundingBox()
        {
            return new Variants<BoundingBoxXYZ>(2)
                .Add(_element.get_BoundingBox(null), "Model")
                .Add(_element.get_BoundingBox(Context.ActiveView), "Active view");
        }

        IVariants ResolveCanBeHidden()
        {
            return Variants.Single(_element.CanBeHidden(Context.ActiveView), "Active view");
        }

        IVariants ResolveIsHidden()
        {
            return Variants.Single(_element.IsHidden(Context.ActiveView), "Active view");
        }

        IVariants ResolveGetDependentElements()
        {
            return Variants.Single(_element.GetDependentElements(null));
        }

        IVariants ResolvePhaseStatus()
        {
            var phases = context.Phases;
            var variants = new Variants<ElementOnPhaseStatus>(phases.Size);
            foreach (Phase phase in phases)
            {
                var result = _element.GetPhaseStatus(phase.Id);
                variants.Add(result, $"{phase.Name}: {result}");
            }

            return variants;
        }

        IVariants ResolveIsPhaseCreatedValid()
        {
            var phases = context.Phases;
            var variants = new Variants<bool>(phases.Size);
            foreach (Phase phase in phases)
            {
                var result = _element.IsPhaseCreatedValid(phase.Id);
                variants.Add(result, $"{phase.Name}: {result}");
            }

            return variants;
        }

        IVariants ResolveIsPhaseDemolishedValid()
        {
            var phases = context.Phases;
            var variants = new Variants<bool>(phases.Size);
            foreach (Phase phase in phases)
            {
                var result = _element.IsPhaseDemolishedValid(phase.Id);
                variants.Add(result, $"{phase.Name}: {result}");
            }

            return variants;
        }

#if REVIT2022_OR_GREATER
        IVariants ResolveIsCreatedPhaseOrderValid()
        {
            var phases = context.Phases;
            var variants = new Variants<bool>(phases.Size);
            foreach (Phase phase in phases)
            {
                var result = _element.IsCreatedPhaseOrderValid(phase.Id);
                variants.Add(result, $"{phase.Name}: {result}");
            }

            return variants;
        }

        IVariants ResolveIsDemolishedPhaseOrderValid()
        {
            var phases = context.Phases;
            var variants = new Variants<bool>(phases.Size);
            foreach (Phase phase in phases)
            {
                var result = _element.IsDemolishedPhaseOrderValid(phase.Id);
                variants.Add(result, $"{phase.Name}: {result}");
            }

            return variants;
        }

#endif
    }

    public virtual void RegisterExtensions(IExtensionManager manager)
    {
        manager.Register(nameof(ElementTransformUtilsExtensions.CanBeMirrored), _ => _element.CanBeMirrored);
        manager.Register(nameof(JoinGeometryUtilsExtensions.GetJoinedElements), _ => _element.GetJoinedElements());
        manager.Register(nameof(SolidSolidCutUtils.GetCuttingSolids), _ => SolidSolidCutUtils.GetCuttingSolids(_element));
        manager.Register(nameof(SolidSolidCutUtils.GetSolidsBeingCut), _ => SolidSolidCutUtils.GetSolidsBeingCut(_element));
        manager.Register(nameof(SolidSolidCutUtils.IsAllowedForSolidCut), _ => SolidSolidCutUtils.IsAllowedForSolidCut(_element));
        manager.Register(nameof(SolidSolidCutUtils.IsElementFromAppropriateContext), _ => SolidSolidCutUtils.IsElementFromAppropriateContext(_element));
        manager.Register(nameof(WorksharingUtils.GetCheckoutStatus), context => WorksharingUtils.GetCheckoutStatus(context, _element.Id));
        manager.Register(nameof(WorksharingUtils.GetWorksharingTooltipInfo), context => WorksharingUtils.GetWorksharingTooltipInfo(context, _element.Id));
        manager.Register(nameof(WorksharingUtils.GetModelUpdatesStatus), context => WorksharingUtils.GetModelUpdatesStatus(context, _element.Id));
        manager.Register(nameof(PartUtils.AreElementsValidForCreateParts), context => PartUtils.AreElementsValidForCreateParts(context, [_element.Id]));
    }

    public virtual void RegisterMenu(ContextMenu contextMenu)
    {
        contextMenu.AddMenuItem("SelectMenuItem")
            .SetCommand(_element, element =>
            {
                if (Context.ActiveUiDocument is null) return;
                if (!element.IsValidObject) return;

                RevitShell.ActionEventHandler.Raise(_ => { Context.ActiveUiDocument.Selection.SetElementIds([element.Id]); });
            })
            .SetShortcut(Key.F6);

        if (_element is not ElementType && _element is not Family)
        {
            contextMenu.AddMenuItem("ShowMenuItem")
                .SetCommand(_element, element =>
                {
                    if (Context.ActiveUiDocument is null) return;
                    if (!element.IsValidObject) return;

                    RevitShell.ActionEventHandler.Raise(_ =>
                    {
                        Context.ActiveUiDocument.ShowElements(element);
                        Context.ActiveUiDocument.Selection.SetElementIds([element.Id]);
                    });
                })
                .SetShortcut(Key.F7);
        }

        contextMenu.AddMenuItem("DeleteMenuItem")
            .SetCommand(_element, async element =>
            {
                if (Context.ActiveUiDocument is null) return;
                var context = (ISnoopViewModel) contextMenu.DataContext;

                try
                {
                    await RevitShell.AsyncEventHandler.RaiseAsync(_ =>
                    {
                        var transaction = new Transaction(element.Document);
                        transaction.Start($"Delete {element.Name}");

                        try
                        {
                            element.Document.Delete(element.Id);
                            transaction.Commit();

                            if (transaction.GetStatus() == TransactionStatus.RolledBack) throw new OperationCanceledException("Element deletion cancelled by user");
                        }
                        catch
                        {
                            if (!transaction.HasEnded()) transaction.RollBack();
                            throw;
                        }
                    });

                    var placementTarget = (FrameworkElement) contextMenu.PlacementTarget;
                    context.RemoveObject(placementTarget.DataContext);
                }
                catch (OperationCanceledException exception)
                {
                    var notificationService = context.ServiceProvider.GetRequiredService<NotificationService>();
                    notificationService.ShowWarning("Warning", exception.Message);
                }
                catch (Exception exception)
                {
                    var notificationService = context.ServiceProvider.GetRequiredService<NotificationService>();
                    notificationService.ShowError("Element deletion error", exception.Message);
                }
            })
            .SetShortcut(Key.Delete);
    }
}