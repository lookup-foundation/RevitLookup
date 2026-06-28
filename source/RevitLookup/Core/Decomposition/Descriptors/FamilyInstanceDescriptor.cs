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

using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Structure.StructuralSections;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class FamilyInstanceDescriptor(FamilyInstance familyInstance) : ElementDescriptor(familyInstance)
{
    public override void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(FamilyInstance.Dispose)).Disable();
        configuration.Member("Room").When(parameters => parameters.Length == 1).Resolve(ResolveGetRoom);
        configuration.Member("FromRoom").When(parameters => parameters.Length == 1).Resolve(ResolveFromRoom);
        configuration.Member("ToRoom").When(parameters => parameters.Length == 1).Resolve(ResolveToRoom);
        configuration.Member(nameof(FamilyInstance.GetOriginalGeometry)).Resolve(ResolveOriginalGeometry);
        configuration.Member(nameof(FamilyInstance.GetReferences)).Resolve(ResolveGetReferences);

        ConfigureExtensions(configuration);
        return;

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

    private void ConfigureExtensions(IMemberConfigurator configuration)
    {
        configuration.Extension("CanFlipFramingEnds").Register(() => StructuralFramingUtils.CanFlipEnds(familyInstance));
        configuration.Extension("IsFramingJoinAllowedAtEnd").Register(ResolveIsJoinAllowedAtEnd);
        configuration.Extension("CanSetFramingEndReference").Register(ResolveCanSetEndReference);
        configuration.Extension("GetFramingEndReference").Register(ResolveGetEndReference);
        configuration.Extension("IsVoidInstanceCuttingElement").Map(nameof(InstanceVoidCutUtils.IsVoidInstanceCuttingElement)).NotSupported();
        configuration.Extension("GetElementsBeingCut").Map(nameof(InstanceVoidCutUtils.GetElementsBeingCut)).NotSupported();
        configuration.Extension("AllowFramingJoinAtEnd").Map(nameof(StructuralFramingUtils.AllowJoinAtEnd)).NotSupported();
        configuration.Extension("DisallowFramingJoinAtEnd").Map(nameof(StructuralFramingUtils.DisallowJoinAtEnd)).NotSupported();
        configuration.Extension("FlipFramingEnds").Map(nameof(StructuralFramingUtils.FlipEnds)).NotSupported();
        configuration.Extension("IsFramingEndReferenceValid").Map(nameof(StructuralFramingUtils.IsEndReferenceValid)).NotSupported();
        configuration.Extension("RemoveFramingEndReference").Map(nameof(StructuralFramingUtils.RemoveEndReference)).NotSupported();
        configuration.Extension("SetFramingEndReference").Map(nameof(StructuralFramingUtils.SetEndReference)).NotSupported();
        configuration.Extension(nameof(StructuralSectionUtils.GetStructuralSection)).Register(() => StructuralSectionUtils.GetStructuralSection(familyInstance.Document, familyInstance.Id));
        configuration.Extension(nameof(StructuralSectionUtils.GetStructuralElementDefinitionData)).Register(GetStructuralElementDefinitionData);

        var isAdaptiveComponentInstance = SafeEvaluate(() => AdaptiveComponentInstanceUtils.IsAdaptiveComponentInstance(familyInstance));
        configuration.Extension(nameof(AdaptiveComponentInstanceUtils.IsAdaptiveComponentInstance)).Register(() => isAdaptiveComponentInstance);
        
        if (isAdaptiveComponentInstance)
        {
            configuration.Extension(nameof(AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds)).Register(() => AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(familyInstance));
            configuration.Extension(nameof(AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds)).Register(() => AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(familyInstance));
            configuration.Extension(nameof(AdaptiveComponentInstanceUtils.GetInstanceShapeHandlePointElementRefIds)).Register(() => AdaptiveComponentInstanceUtils.GetInstanceShapeHandlePointElementRefIds(familyInstance));
            configuration.Extension(nameof(AdaptiveComponentInstanceUtils.HasAdaptiveFamilySymbol)).Register(() => AdaptiveComponentInstanceUtils.HasAdaptiveFamilySymbol(familyInstance));
            configuration.Extension(nameof(AdaptiveComponentInstanceUtils.IsInstanceFlipped)).Register(() => AdaptiveComponentInstanceUtils.IsInstanceFlipped(familyInstance));
            configuration.Extension("MoveAdaptiveComponentInstance").Map(nameof(AdaptiveComponentInstanceUtils.MoveAdaptiveComponentInstance)).NotSupported();
            configuration.Extension("SetAdaptiveInstanceFlipped").Map(nameof(AdaptiveComponentInstanceUtils.SetInstanceFlipped)).NotSupported();
        }

        var isMassFamilyInstance = SafeEvaluate(() => MassLevelData.IsMassFamilyInstance(familyInstance.Document, familyInstance.Id));
        configuration.Extension(nameof(MassLevelData.IsMassFamilyInstance)).Register(() => isMassFamilyInstance);
        
        if (isMassFamilyInstance)
        {
            configuration.Extension("GetMassGrossFloorArea").Register(() => MassInstanceUtils.GetGrossFloorArea(familyInstance.Document, familyInstance.Id));
            configuration.Extension("GetMassGrossSurfaceArea").Register(() => MassInstanceUtils.GetGrossSurfaceArea(familyInstance.Document, familyInstance.Id));
            configuration.Extension("GetMassGrossVolume").Register(() => MassInstanceUtils.GetGrossVolume(familyInstance.Document, familyInstance.Id));
            configuration.Extension("GetMassJoinedElementIds").Register(() => MassInstanceUtils.GetJoinedElementIds(familyInstance.Document, familyInstance.Id));
            configuration.Extension(nameof(MassInstanceUtils.GetMassLevelDataIds)).Register(() => MassInstanceUtils.GetMassLevelDataIds(familyInstance.Document, familyInstance.Id));
            configuration.Extension(nameof(MassInstanceUtils.GetMassLevelIds)).Register(() => MassInstanceUtils.GetMassLevelIds(familyInstance.Document, familyInstance.Id));
            configuration.Extension("AddMassLevelData").Map(nameof(MassInstanceUtils.AddMassLevelDataToMassInstance)).NotSupported();
            configuration.Extension("RemoveMassLevelData").Map(nameof(MassInstanceUtils.RemoveMassLevelDataFromMassInstance)).NotSupported();
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

        object? GetStructuralElementDefinitionData()
        {
            StructuralSectionUtils.GetStructuralElementDefinitionData(familyInstance.Document, familyInstance.Id, out var data);
            return data;
        }
    }
}