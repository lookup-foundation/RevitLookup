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
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Structure.StructuralSections;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class FamilyInstanceDescriptor(FamilyInstance familyInstance) : ElementDescriptor(familyInstance)
{
    public override Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            "Room" when parameters.Length == 1 => ResolveGetRoom,
            "FromRoom" when parameters.Length == 1 => ResolveFromRoom,
            "ToRoom" when parameters.Length == 1 => ResolveToRoom,
            nameof(FamilyInstance.GetOriginalGeometry) => ResolveOriginalGeometry,
            nameof(FamilyInstance.GetReferences) => ResolveGetReferences,
            _ => null
        };

        IVariant ResolveGetRoom()
        {
            var variants = Variants.Values<Room>(familyInstance.Document.Phases.Size);
            foreach (Phase phase in familyInstance.Document.Phases)
            {
                try
                {
                    variants.Add(familyInstance.get_Room(phase), phase.Name);
                }
                catch
                {
                    // ignored
                }
            }

            return variants.Consume();
        }

        IVariant ResolveFromRoom()
        {
            var variants = Variants.Values<Room>(familyInstance.Document.Phases.Size);
            foreach (Phase phase in familyInstance.Document.Phases)
            {
                try
                {
                    variants.Add(familyInstance.get_FromRoom(phase), phase.Name);
                }
                catch
                {
                    // ignored
                }
            }

            return variants.Consume();
        }

        IVariant ResolveToRoom()
        {
            var variants = Variants.Values<Room>(familyInstance.Document.Phases.Size);
            foreach (Phase phase in familyInstance.Document.Phases)
            {
                try
                {
                    variants.Add(familyInstance.get_ToRoom(phase), phase.Name);
                }
                catch
                {
                    // ignored
                }
            }

            return variants.Consume();
        }

        IVariant ResolveOriginalGeometry()
        {
            return Variants.Values<GeometryElement>(10)
                .Add(familyInstance.GetOriginalGeometry(new Options
                {
                    View = RevitContext.ActiveView,
                }), "Active view")
                .Add(familyInstance.GetOriginalGeometry(new Options
                {
                    View = RevitContext.ActiveView,
                    IncludeNonVisibleObjects = true,
                }), "Active view, including non-visible objects")
                .Add(familyInstance.GetOriginalGeometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Coarse,
                }), "Model, coarse detail level")
                .Add(familyInstance.GetOriginalGeometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Fine,
                }), "Model, fine detail level")
                .Add(familyInstance.GetOriginalGeometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Medium,
                }), "Model, medium detail level")
                .Add(familyInstance.GetOriginalGeometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Undefined,
                }), "Model, undefined detail level")
                .Add(familyInstance.GetOriginalGeometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Coarse,
                    IncludeNonVisibleObjects = true,
                }), "Model, coarse detail level, including non-visible objects")
                .Add(familyInstance.GetOriginalGeometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Fine,
                    IncludeNonVisibleObjects = true,
                }), "Model, fine detail level, including non-visible objects")
                .Add(familyInstance.GetOriginalGeometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Medium,
                    IncludeNonVisibleObjects = true,
                }), "Model, medium detail level, including non-visible objects")
                .Add(familyInstance.GetOriginalGeometry(new Options
                {
                    DetailLevel = ViewDetailLevel.Undefined,
                    IncludeNonVisibleObjects = true,
                }), "Model, undefined detail level, including non-visible objects")
                .Consume();
        }

        IVariant ResolveGetReferences()
        {
            return Variants.Values<IList<Reference>>(11)
                .Add(familyInstance.GetReferences(FamilyInstanceReferenceType.Back), "Back")
                .Add(familyInstance.GetReferences(FamilyInstanceReferenceType.Bottom), "Bottom")
                .Add(familyInstance.GetReferences(FamilyInstanceReferenceType.StrongReference), "Strong reference")
                .Add(familyInstance.GetReferences(FamilyInstanceReferenceType.WeakReference), "Weak reference")
                .Add(familyInstance.GetReferences(FamilyInstanceReferenceType.Front), "Front")
                .Add(familyInstance.GetReferences(FamilyInstanceReferenceType.Left), "Left")
                .Add(familyInstance.GetReferences(FamilyInstanceReferenceType.Right), "Right")
                .Add(familyInstance.GetReferences(FamilyInstanceReferenceType.Top), "Top")
                .Add(familyInstance.GetReferences(FamilyInstanceReferenceType.CenterElevation), "Center elevation")
                .Add(familyInstance.GetReferences(FamilyInstanceReferenceType.CenterFrontBack), "Center front back")
                .Add(familyInstance.GetReferences(FamilyInstanceReferenceType.CenterLeftRight), "Center left right")
                .Add(familyInstance.GetReferences(FamilyInstanceReferenceType.NotAReference), "Not a reference")
                .Consume();
        }
    }

    public override void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define("CanFlipFramingEnds").Register(() => Variants.Value(StructuralFramingUtils.CanFlipEnds(familyInstance)));
        manager.Define("IsFramingJoinAllowedAtEnd").Register(ResolveIsJoinAllowedAtEnd);
        manager.Define("CanSetFramingEndReference").Register(ResolveCanSetEndReference);
        manager.Define("GetFramingEndReference").Register(ResolveGetEndReference);
        manager.Define("IsVoidInstanceCuttingElement").Map(nameof(InstanceVoidCutUtils.IsVoidInstanceCuttingElement)).AsNotSupported();
        manager.Define("GetElementsBeingCut").Map(nameof(InstanceVoidCutUtils.GetElementsBeingCut)).AsNotSupported();
        manager.Define("AllowFramingJoinAtEnd").Map(nameof(StructuralFramingUtils.AllowJoinAtEnd)).AsNotSupported();
        manager.Define("DisallowFramingJoinAtEnd").Map(nameof(StructuralFramingUtils.DisallowJoinAtEnd)).AsNotSupported();
        manager.Define("FlipFramingEnds").Map(nameof(StructuralFramingUtils.FlipEnds)).AsNotSupported();
        manager.Define("IsFramingEndReferenceValid").Map(nameof(StructuralFramingUtils.IsEndReferenceValid)).AsNotSupported();
        manager.Define("RemoveFramingEndReference").Map(nameof(StructuralFramingUtils.RemoveEndReference)).AsNotSupported();
        manager.Define("SetFramingEndReference").Map(nameof(StructuralFramingUtils.SetEndReference)).AsNotSupported();
        manager.Define(nameof(StructuralSectionUtils.GetStructuralSection)).Register(() => Variants.Value(StructuralSectionUtils.GetStructuralSection(familyInstance.Document, familyInstance.Id)));
        manager.Define(nameof(StructuralSectionUtils.GetStructuralElementDefinitionData)).Register(GetStructuralElementDefinitionData);

        if (manager.Define(nameof(AdaptiveComponentInstanceUtils.IsAdaptiveComponentInstance)).TryRegister(() => Variants.Value(AdaptiveComponentInstanceUtils.IsAdaptiveComponentInstance(familyInstance))))
        {
            manager.Define(nameof(AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds)).Register(() => Variants.Value(AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(familyInstance)));
            manager.Define(nameof(AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds)).Register(() => Variants.Value(AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(familyInstance)));
            manager.Define(nameof(AdaptiveComponentInstanceUtils.GetInstanceShapeHandlePointElementRefIds)).Register(() => Variants.Value(AdaptiveComponentInstanceUtils.GetInstanceShapeHandlePointElementRefIds(familyInstance)));
            manager.Define(nameof(AdaptiveComponentInstanceUtils.HasAdaptiveFamilySymbol)).Register(() => Variants.Value(AdaptiveComponentInstanceUtils.HasAdaptiveFamilySymbol(familyInstance)));
            manager.Define(nameof(AdaptiveComponentInstanceUtils.IsInstanceFlipped)).Register(() => Variants.Value(AdaptiveComponentInstanceUtils.IsInstanceFlipped(familyInstance)));
            manager.Define("MoveAdaptiveComponentInstance").Map(nameof(AdaptiveComponentInstanceUtils.MoveAdaptiveComponentInstance)).AsNotSupported();
            manager.Define("SetAdaptiveInstanceFlipped").Map(nameof(AdaptiveComponentInstanceUtils.SetInstanceFlipped)).AsNotSupported();
        }

        if (manager.Define(nameof(MassLevelData.IsMassFamilyInstance)).TryRegister(() => Variants.Value(MassLevelData.IsMassFamilyInstance(familyInstance.Document, familyInstance.Id))))
        {
            manager.Define("GetMassGrossFloorArea").Register(() => Variants.Value(MassInstanceUtils.GetGrossFloorArea(familyInstance.Document, familyInstance.Id)));
            manager.Define("GetMassGrossSurfaceArea").Register(() => Variants.Value(MassInstanceUtils.GetGrossSurfaceArea(familyInstance.Document, familyInstance.Id)));
            manager.Define("GetMassGrossVolume").Register(() => Variants.Value(MassInstanceUtils.GetGrossVolume(familyInstance.Document, familyInstance.Id)));
            manager.Define("GetMassJoinedElementIds").Register(() => Variants.Value(MassInstanceUtils.GetJoinedElementIds(familyInstance.Document, familyInstance.Id)));
            manager.Define(nameof(MassInstanceUtils.GetMassLevelDataIds)).Register(() => Variants.Value(MassInstanceUtils.GetMassLevelDataIds(familyInstance.Document, familyInstance.Id)));
            manager.Define(nameof(MassInstanceUtils.GetMassLevelIds)).Register(() => Variants.Value(MassInstanceUtils.GetMassLevelIds(familyInstance.Document, familyInstance.Id)));
            manager.Define("AddMassLevelData").Map(nameof(MassInstanceUtils.AddMassLevelDataToMassInstance)).AsNotSupported();
            manager.Define("RemoveMassLevelData").Map(nameof(MassInstanceUtils.RemoveMassLevelDataFromMassInstance)).AsNotSupported();
        }

        return;

        IVariant ResolveIsJoinAllowedAtEnd()
        {
            var isJoinAllowedAtEnd0 = StructuralFramingUtils.IsJoinAllowedAtEnd(familyInstance, 0);
            var isJoinAllowedAtEnd1 = StructuralFramingUtils.IsJoinAllowedAtEnd(familyInstance, 1);

            return Variants.Values<bool>(2)
                .Add(isJoinAllowedAtEnd0, $"End 0: {isJoinAllowedAtEnd0}")
                .Add(isJoinAllowedAtEnd1, $"End 1: {isJoinAllowedAtEnd1}")
                .Consume();
        }

        IVariant ResolveCanSetEndReference()
        {
            var canSetEndReference0 = StructuralFramingUtils.CanSetEndReference(familyInstance, 0);
            var canSetEndReference1 = StructuralFramingUtils.CanSetEndReference(familyInstance, 1);

            return Variants.Values<bool>(2)
                .Add(canSetEndReference0, $"End 0: {canSetEndReference0}")
                .Add(canSetEndReference1, $"End 1: {canSetEndReference1}")
                .Consume();
        }

        IVariant ResolveGetEndReference()
        {
            return Variants.Values<Reference>(2)
                .Add(StructuralFramingUtils.GetEndReference(familyInstance, 0), "End 0")
                .Add(StructuralFramingUtils.GetEndReference(familyInstance, 1), "End 1")
                .Consume();
        }

        IVariant GetStructuralElementDefinitionData()
        {
            StructuralSectionUtils.GetStructuralElementDefinitionData(familyInstance.Document, familyInstance.Id, out var data);
            return Variants.Value(data);
        }
    }
}