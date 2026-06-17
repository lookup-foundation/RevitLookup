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

using System.Collections;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI.Selection;
using Autodesk.Windows;
using RevitLookup.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition;

public static class RevitObjectsCollector
{
    public static IEnumerable GetObjects(KnownDecompositionObject decompositionObject)
    {
        return decompositionObject switch
        {
            KnownDecompositionObject.View => FindView(),
            KnownDecompositionObject.Document => FindDocument(),
            KnownDecompositionObject.Application => FindApplication(),
            KnownDecompositionObject.UiApplication => FindUiApplication(),
            KnownDecompositionObject.UiControlledApplication => FindUiControlledApplication(),
            KnownDecompositionObject.Database => FindDatabase(),
            KnownDecompositionObject.DependentElements => FindDependentElements(),
            KnownDecompositionObject.Selection => FindSelection(),
            KnownDecompositionObject.Face => FindFace(),
            KnownDecompositionObject.Edge => FindEdge(),
            KnownDecompositionObject.SubElement => FindSubElement(),
            KnownDecompositionObject.Point => FindPoint(),
            KnownDecompositionObject.LinkedElement => FindLinkedElement(),
            KnownDecompositionObject.ComponentManager => FindComponentManager(),
            KnownDecompositionObject.PerformanceAdviser => FindPerformanceAdviser(),
            KnownDecompositionObject.UpdaterRegistry => FindUpdaterRegistry(),
            KnownDecompositionObject.Services => FindServices(),
            KnownDecompositionObject.Schemas => FindSchemas(),
            _ => throw new ArgumentOutOfRangeException(nameof(decompositionObject), decompositionObject, null)
        };
    }

    private static IEnumerable FindView()
    {
        return new object?[] {RevitContext.ActiveView};
    }

    private static IEnumerable FindDocument()
    {
        return new object?[] {RevitContext.ActiveDocument};
    }

    private static IEnumerable FindApplication()
    {
        return new object?[] {RevitApiContext.Application};
    }

    private static IEnumerable FindUiApplication()
    {
        return new object?[] {RevitContext.UiApplication};
    }

    private static IEnumerable FindUiControlledApplication()
    {
        return new object[] {RevitContext.UiApplication.AsControlledApplication()};
    }

    private static IEnumerable FindEdge()
    {
        return FindObject(ObjectType.Edge);
    }

    private static IEnumerable FindFace()
    {
        return FindObject(ObjectType.Face);
    }

    private static IEnumerable FindSubElement()
    {
        return FindObject(ObjectType.Subelement);
    }

    private static IEnumerable FindPoint()
    {
        return FindObject(ObjectType.PointOnElement);
    }

    private static IEnumerable FindLinkedElement()
    {
        return FindObject(ObjectType.LinkedElement);
    }

    private static IEnumerable FindSelection()
    {
        var activeUiDocument = RevitContext.ActiveUiDocument;
        if (activeUiDocument is null)
        {
            return Array.Empty<object>();
        }

        var selectedIds = activeUiDocument.Selection.GetElementIds();
        if (selectedIds.Count > 0)
        {
            return activeUiDocument.Document.CollectElements()
                .OfElements(selectedIds)
                .ToElements();
        }

        return activeUiDocument.Document.CollectElements(activeUiDocument.ActiveView.Id).ToElements();
    }

    private static IEnumerable FindDatabase()
    {
        var activeDocument = RevitContext.ActiveDocument!;
        return activeDocument
            .CollectElements().Types()
            .UnionWith(activeDocument.CollectElements().Instances())
            .ToElements();
    }

    private static IEnumerable FindDependentElements()
    {
        var selectedIds = RevitContext.ActiveUiDocument!.Selection.GetElementIds();
        if (selectedIds.Count == 0) return Array.Empty<object>();

        var elements = new List<ElementId>();
        var activeDocument = RevitContext.ActiveDocument!;
        var selectedElements = activeDocument.CollectElements(selectedIds).Instances();

        foreach (var selectedElement in selectedElements)
        {
            var dependentElements = selectedElement.GetDependentElements(null);
            foreach (var dependentElement in dependentElements) elements.Add(dependentElement);
        }

        return activeDocument.CollectElements().OfElements(elements).ToElements();
    }

    private static IEnumerable FindComponentManager()
    {
        return new object?[] {typeof(ComponentManager)};
    }

    private static IEnumerable FindPerformanceAdviser()
    {
        return new object?[] {PerformanceAdviser.GetPerformanceAdviser()};
    }

    private static IEnumerable FindUpdaterRegistry()
    {
        return UpdaterRegistry.GetRegisteredUpdaterInfos();
    }

    private static IEnumerable FindSchemas()
    {
        return Schema.ListSchemas();
    }

    private static IEnumerable FindServices()
    {
        return ExternalServiceRegistry.GetServices();
    }

    private static IEnumerable FindObject(ObjectType objectType)
    {
        var activeUiDocument = RevitContext.ActiveUiDocument;
        if (activeUiDocument is null)
        {
            return Array.Empty<object>();
        }

        var reference = activeUiDocument.Selection.PickObject(objectType);

        object element;
        switch (objectType)
        {
            case ObjectType.Edge:
            case ObjectType.Face:
                element = activeUiDocument.Document.GetElement(reference).GetGeometryObjectFromReference(reference);
                break;
            case ObjectType.Element:
            case ObjectType.Subelement:
                element = activeUiDocument.Document.GetElement(reference);
                break;
            case ObjectType.PointOnElement:
                element = reference.GlobalPoint;
                break;
            case ObjectType.LinkedElement:
                var revitLinkInstance = reference.ElementId.ToElement<RevitLinkInstance>(activeUiDocument.Document)!;
                element = revitLinkInstance.GetLinkDocument().GetElement(reference.LinkedElementId);
                break;
            case ObjectType.Nothing:
            default:
                throw new NotSupportedException();
        }

        return new[] {element};
    }
}