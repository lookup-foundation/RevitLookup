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
using Autodesk.Revit.DB.Architecture;
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
                if (familyInstance.GetPhaseStatus(phase.Id) == ElementOnPhaseStatus.Future) continue;
                variants.Add(familyInstance.get_Room(phase), phase.Name);
            }

            return variants.Consume();
        }

        IVariant ResolveFromRoom()
        {
            var variants = Variants.Values<Room>(familyInstance.Document.Phases.Size);
            foreach (Phase phase in familyInstance.Document.Phases)
            {
                if (familyInstance.GetPhaseStatus(phase.Id) == ElementOnPhaseStatus.Future) continue;
                variants.Add(familyInstance.get_FromRoom(phase), phase.Name);
            }

            return variants.Consume();
        }

        IVariant ResolveToRoom()
        {
            var variants = Variants.Values<Room>(familyInstance.Document.Phases.Size);
            foreach (Phase phase in familyInstance.Document.Phases)
            {
                if (familyInstance.GetPhaseStatus(phase.Id) == ElementOnPhaseStatus.Future) continue;
                variants.Add(familyInstance.get_ToRoom(phase), phase.Name);
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
        manager.Register(nameof(AdaptiveComponentInstanceUtils.IsAdaptiveComponentInstance), () => Variants.Value(AdaptiveComponentInstanceUtils.IsAdaptiveComponentInstance(familyInstance)));
    }
}