// Copyright 2003-2023 by Autodesk, Inc.
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
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using RevitLookup.Core.Contracts;
using RevitLookup.Core.Objects;
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

    public void RegisterMenu(ContextMenu contextMenu, UIElement bindableElement)
    {
        if (_element is ElementType) return;

        contextMenu.AddMenuItem()
            .SetHeader("Show element")
            .SetCommand(_element, element =>
            {
                Application.ActionEventHandler.Raise(_ =>
                {
                    if (RevitApi.UiDocument is null) return;
                    RevitApi.UiDocument.ShowElements(element);
                    RevitApi.UiDocument.Selection.SetElementIds(new List<ElementId>(1) {element.Id});
                });
            })
            .SetShortcut(bindableElement, ModifierKeys.Alt, Key.F7);
    }

    public void RegisterExtensions(IExtensionManager manager)
    {
        manager.Register(_element, extension =>
        {
            extension.Name = nameof(ElementTransformUtilsExtensions.CanBeMirrored);
            extension.Result = extension.Value.CanBeMirrored;
        });
        manager.Register(_element, extension =>
        {
            extension.Name = nameof(JoinGeometryUtilsExtensions.GetJoinedElements);
            extension.Result = extension.Value.GetJoinedElements();
        });
    }

    public ResolveSet Resolve(Document context, string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(Element.CanBeHidden) => ResolveSet.Append(_element.CanBeHidden(RevitApi.ActiveView), "Active view"),
            nameof(Element.IsHidden) => ResolveSet.Append(_element.IsHidden(RevitApi.ActiveView), "Active view"),
            nameof(Element.GetDependentElements) => ResolveSet.Append(_element.GetDependentElements(null)),
            nameof(Element.GetMaterialIds) => ResolveSet
                .Append(_element.GetMaterialIds(true), "Paint materials")
                .AppendVariant(_element.GetMaterialIds(false), "Geometry and compound structure materials"),
            "BoundingBox" => ResolveSet
                .Append(_element.get_BoundingBox(null), "Model")
                .AppendVariant(_element.get_BoundingBox(RevitApi.ActiveView), "Active view"),
            "Geometry" => new ResolveSet(10)
                .AppendVariant(_element.get_Geometry(new Options
                {
                    View = RevitApi.ActiveView,
                    ComputeReferences = true
                }), "Active view")
                .AppendVariant(_element.get_Geometry(new Options
                {
                    View = RevitApi.ActiveView,
                    IncludeNonVisibleObjects = true,
                    ComputeReferences = true
                }), "Active view, including non-visible objects")
                .AppendVariant(_element.get_Geometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Coarse,
                    ComputeReferences = true
                }), "Model, coarse detail level")
                .AppendVariant(_element.get_Geometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Fine,
                    ComputeReferences = true
                }), "Model, fine detail level")
                .AppendVariant(_element.get_Geometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Medium,
                    ComputeReferences = true
                }), "Model, medium detail level")
                .AppendVariant(_element.get_Geometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Undefined,
                    ComputeReferences = true
                }), "Model, undefined detail level")
                .AppendVariant(_element.get_Geometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Coarse,
                    IncludeNonVisibleObjects = true,
                    ComputeReferences = true
                }), "Model, coarse detail level, including non-visible objects")
                .AppendVariant(_element.get_Geometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Fine,
                    IncludeNonVisibleObjects = true,
                    ComputeReferences = true
                }), "Model, fine detail level, including non-visible objects")
                .AppendVariant(_element.get_Geometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Medium,
                    IncludeNonVisibleObjects = true,
                    ComputeReferences = true
                }), "Model, medium detail level, including non-visible objects")
                .AppendVariant(_element.get_Geometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Undefined,
                    IncludeNonVisibleObjects = true,
                    ComputeReferences = true
                }), "Model, undefined detail level, including non-visible objects"),
            nameof(Element.GetMaterialArea) => ResolveGetMaterialArea(),
            nameof(Element.GetMaterialVolume) => ResolveGetMaterialVolume(),
            nameof(Element.GetEntity) => ResolveGetEntity(),
            _ => null
        };

        ResolveSet ResolveGetMaterialArea()
        {
            var geometryMaterials = _element.GetMaterialIds(false);
            var paintMaterials = _element.GetMaterialIds(true);

            var capacity = geometryMaterials.Count + paintMaterials.Count;
            var resolveSummary = new ResolveSet(capacity);
            if (capacity == 0) return resolveSummary;

            foreach (var materialId in geometryMaterials)
            {
                var area = _element.GetMaterialArea(materialId, false);
                resolveSummary.AppendVariant(new KeyValuePair<ElementId, double>(materialId, area));
            }

            foreach (var materialId in paintMaterials)
            {
                var area = _element.GetMaterialArea(materialId, true);
                resolveSummary.AppendVariant(new KeyValuePair<ElementId, double>(materialId, area));
            }

            return resolveSummary;
        }

        ResolveSet ResolveGetMaterialVolume()
        {
            var geometryMaterials = _element.GetMaterialIds(false);

            var resolveSummary = new ResolveSet(geometryMaterials.Count);
            if (geometryMaterials.Count == 0) return resolveSummary;

            foreach (var materialId in geometryMaterials)
            {
                var area = _element.GetMaterialVolume(materialId);
                resolveSummary.AppendVariant(new KeyValuePair<ElementId, double>(materialId, area));
            }

            return resolveSummary;
        }

        ResolveSet ResolveGetEntity()
        {
            var resolveSummary = new ResolveSet();
            var schemas = Schema.ListSchemas();
            foreach (var schema in schemas)
            {
                if (!schema.ReadAccessGranted()) continue;
                var entity = _element.GetEntity(schema);
                if (!entity.IsValid()) continue;

                resolveSummary.AppendVariant(entity, schema.SchemaName);
            }

            return resolveSummary;
        }
    }
}