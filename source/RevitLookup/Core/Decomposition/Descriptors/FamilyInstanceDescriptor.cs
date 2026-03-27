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
        manager.Register(nameof(AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds), () => Variants.Value(AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(familyInstance)));
        manager.Register(nameof(AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds), () => Variants.Value(AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(familyInstance)));
        manager.Register(nameof(AdaptiveComponentInstanceUtils.GetInstanceShapeHandlePointElementRefIds), () => Variants.Value(AdaptiveComponentInstanceUtils.GetInstanceShapeHandlePointElementRefIds(familyInstance)));
        manager.Register(nameof(AdaptiveComponentInstanceUtils.HasAdaptiveFamilySymbol), () => Variants.Value(AdaptiveComponentInstanceUtils.HasAdaptiveFamilySymbol(familyInstance)));
        manager.Register(nameof(AdaptiveComponentInstanceUtils.IsAdaptiveComponentInstance), () => Variants.Value(AdaptiveComponentInstanceUtils.IsAdaptiveComponentInstance(familyInstance)));
        manager.Register(nameof(AdaptiveComponentInstanceUtils.IsInstanceFlipped), () => Variants.Value(AdaptiveComponentInstanceUtils.IsInstanceFlipped(familyInstance)));
        manager.Register(nameof(MassLevelData.IsMassFamilyInstance), () => Variants.Value(MassLevelData.IsMassFamilyInstance(familyInstance.Document, familyInstance.Id)));
        manager.Register("GetMassGrossFloorArea", () => Variants.Value(MassInstanceUtils.GetGrossFloorArea(familyInstance.Document, familyInstance.Id)));
        manager.Register("GetMassGrossSurfaceArea", () => Variants.Value(MassInstanceUtils.GetGrossSurfaceArea(familyInstance.Document, familyInstance.Id)));
        manager.Register("GetMassGrossVolume", () => Variants.Value(MassInstanceUtils.GetGrossVolume(familyInstance.Document, familyInstance.Id)));
        manager.Register("GetMassJoinedElementIds", () => Variants.Value(MassInstanceUtils.GetJoinedElementIds(familyInstance.Document, familyInstance.Id)));
        manager.Register(nameof(MassInstanceUtils.GetMassLevelDataIds), () => Variants.Value(MassInstanceUtils.GetMassLevelDataIds(familyInstance.Document, familyInstance.Id)));
        manager.Register(nameof(MassInstanceUtils.GetMassLevelIds), () => Variants.Value(MassInstanceUtils.GetMassLevelIds(familyInstance.Document, familyInstance.Id)));
        manager.Register(nameof(StructuralFramingUtils.CanFlipEnds), () => Variants.Value(StructuralFramingUtils.CanFlipEnds(familyInstance)));
        manager.Register(nameof(StructuralFramingUtils.IsJoinAllowedAtEnd), ResolveIsJoinAllowedAtEnd);
        manager.Register(nameof(StructuralFramingUtils.CanSetEndReference), ResolveCanSetEndReference);
        manager.Register(nameof(StructuralFramingUtils.GetEndReference), ResolveGetEndReference);
        manager.Register(nameof(StructuralSectionUtils.GetStructuralSection), () => Variants.Value(StructuralSectionUtils.GetStructuralSection(familyInstance.Document, familyInstance.Id)));
        manager.Register(nameof(StructuralSectionUtils.GetStructuralElementDefinitionData), GetStructuralElementDefinitionData);

        RegisterNotSupportedExtensions();
        return;

        // Indicates API methods that exist but cannot produce a read-only value in RevitLookup
        void RegisterNotSupportedExtensions()
        {
            _ = nameof(AdaptiveComponentInstanceUtils.MoveAdaptiveComponentInstance);
            manager.Register("MoveAdaptiveComponentInstance", Variants.NotSupported);
            
            _ = nameof(AdaptiveComponentInstanceUtils.SetInstanceFlipped);
            manager.Register("SetAdaptiveInstanceFlipped", Variants.NotSupported);
            
            _ = nameof(InstanceVoidCutUtils.IsVoidInstanceCuttingElement);
            manager.Register("IsVoidInstanceCuttingElement", Variants.NotSupported);
            
            _ = nameof(MassInstanceUtils.AddMassLevelDataToMassInstance);
            manager.Register("AddMassLevelData", Variants.NotSupported);
            
            _ = nameof(MassInstanceUtils.RemoveMassLevelDataFromMassInstance);
            manager.Register("RemoveMassLevelData", Variants.NotSupported);
            
            _ = nameof(StructuralFramingUtils.AllowJoinAtEnd);
            manager.Register("AllowFramingJoinAtEnd", Variants.NotSupported);
            
            _ = nameof(StructuralFramingUtils.DisallowJoinAtEnd);
            manager.Register("DisallowFramingJoinAtEnd", Variants.NotSupported);
            
            _ = nameof(StructuralFramingUtils.FlipEnds);
            manager.Register("FlipFramingEnds", Variants.NotSupported);
            
            _ = nameof(StructuralFramingUtils.IsEndReferenceValid);
            manager.Register("IsFramingEndReferenceValid", Variants.NotSupported);
            
            _ = nameof(StructuralFramingUtils.RemoveEndReference);
            manager.Register("RemoveFramingEndReference", Variants.NotSupported);
            
            _ = nameof(StructuralFramingUtils.SetEndReference);
            manager.Register("SetFramingEndReference", Variants.NotSupported);
        }

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