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
using Autodesk.Revit.DB.Structure;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using RevitLookup.Core.Decomposition.Extensions;
#if REVIT2026_OR_GREATER
using Autodesk.Revit.DB.ExternalData;
#endif

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class ElementTypeDescriptor(ElementType elementType) : ElementDescriptor(elementType)
{
    public override void RegisterExtensions(IExtensionManager manager)
    {
#if REVIT2025_OR_GREATER
        if (elementType.Category?.Id.IsCategory(BuiltInCategory.OST_RebarSpliceType) == true)
        {
            manager.Define("GetRebarSpliceLapLengthMultiplier").Register(() => Variants.Value(RebarSpliceTypeUtils.GetLapLengthMultiplier(elementType.Document, elementType.Id)));
            manager.Define("GetRebarSpliceShiftOption").Register(() => Variants.Value(RebarSpliceTypeUtils.GetShiftOption(elementType.Document, elementType.Id)));
            manager.Define("GetRebarSpliceStaggerLengthMultiplier").Register(() => Variants.Value(RebarSpliceTypeUtils.GetStaggerLengthMultiplier(elementType.Document, elementType.Id)));
            manager.Define("SetRebarSpliceLapLengthMultiplier").Map(nameof(RebarSpliceTypeUtils.SetLapLengthMultiplier)).AsNotSupported();
            manager.Define("SetRebarSpliceShiftOption").Map(nameof(RebarSpliceTypeUtils.SetShiftOption)).AsNotSupported();
            manager.Define("SetRebarSpliceStaggerLengthMultiplier").Map(nameof(RebarSpliceTypeUtils.SetStaggerLengthMultiplier)).AsNotSupported();
        }
#endif
#if REVIT2026_OR_GREATER

        if (elementType.Category?.Id.IsCategory(BuiltInCategory.OST_RebarCrankType) == true)
        {
            manager.Define("GetRebarCrankLengthMultiplier").Register(() => Variants.Value(RebarCrankTypeUtils.GetCrankLengthMultiplier(elementType.Document, elementType.Id)));
            manager.Define("GetRebarCrankOffsetMultiplier").Register(() => Variants.Value(RebarCrankTypeUtils.GetCrankOffsetMultiplier(elementType.Document, elementType.Id)));
            manager.Define("GetRebarCrankRatio").Register(() => Variants.Value(RebarCrankTypeUtils.GetCrankRatio(elementType.Document, elementType.Id)));
            manager.Define("SetRebarCrankLengthMultiplier").Map(nameof(RebarCrankTypeUtils.SetCrankLengthMultiplier)).AsNotSupported();
            manager.Define("SetRebarCrankOffsetMultiplier").Map(nameof(RebarCrankTypeUtils.SetCrankOffsetMultiplier)).AsNotSupported();
            manager.Define("SetRebarCrankRatio").Map(nameof(RebarCrankTypeUtils.SetCrankRatio)).AsNotSupported();
        }

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
        if (manager.Define(nameof(CoordinationModelLinkUtils.IsCoordinationModelType)).TryRegister(() => Variants.Value(CoordinationModelLinkUtils.IsCoordinationModelType(elementType.Document, elementType))))
        {
            manager.Define("GetCoordinationModelTransparencyOverride").Register(() => Variants.Value(CoordinationModelLinkUtils.GetTransparencyOverride(elementType.Document, elementType.Document.ActiveView, elementType)));
            manager.Define(nameof(CoordinationModelLinkUtils.GetCoordinationModelTypeData)).Register(() => Variants.Value(CoordinationModelLinkUtils.GetCoordinationModelTypeData(elementType.Document, elementType)));
            manager.Define(nameof(CoordinationModelLinkUtils.ReloadLocalCoordinationModelFrom)).AsNotSupported();
            manager.Define(nameof(CoordinationModelLinkUtils.ReloadAutodeskDocsCoordinationModelFrom)).AsNotSupported();
            manager.Define("ContainsCoordinationModelCategory").Map(nameof(CoordinationModelLinkUtils.ContainsCategory)).AsNotSupported();
            manager.Define("GetCoordinationModelColorOverride").Map(nameof(CoordinationModelLinkUtils.GetColorOverride)).AsNotSupported();
            manager.Define("GetCoordinationModelColorOverrideForCategory").Map(nameof(CoordinationModelLinkUtils.GetColorOverrideForCategory)).AsNotSupported();
            manager.Define("GetCoordinationModelVisibilityOverride").Map(nameof(CoordinationModelLinkUtils.GetVisibilityOverride)).AsNotSupported();
            manager.Define("GetCoordinationModelVisibilityOverrideForCategory").Map(nameof(CoordinationModelLinkUtils.GetVisibilityOverrideForCategory)).AsNotSupported();
            manager.Define("ReloadCoordinationModel").Map(nameof(CoordinationModelLinkUtils.Reload)).AsNotSupported();
            manager.Define("UnloadCoordinationModel").Map(nameof(CoordinationModelLinkUtils.Unload)).AsNotSupported();
            manager.Define("SetCoordinationModelColorOverride").Map(nameof(CoordinationModelLinkUtils.SetColorOverride)).AsNotSupported();
            manager.Define("SetCoordinationModelColorOverrideForCategory").Map(nameof(CoordinationModelLinkUtils.SetColorOverrideForCategory)).AsNotSupported();
            manager.Define("SetCoordinationModelTransparencyOverride").Map(nameof(CoordinationModelLinkUtils.SetTransparencyOverride)).AsNotSupported();
            manager.Define("SetCoordinationModelVisibilityOverride").Map(nameof(CoordinationModelLinkUtils.SetVisibilityOverride)).AsNotSupported();
            manager.Define("SetCoordinationModelVisibilityOverrideForCategory").Map(nameof(CoordinationModelLinkUtils.SetVisibilityOverrideForCategory)).AsNotSupported();
        }
    }
#endif
}