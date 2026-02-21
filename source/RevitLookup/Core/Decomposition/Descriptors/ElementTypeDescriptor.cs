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

using Autodesk.Revit.DB.Structure;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
#if REVIT2026_OR_GREATER
using Autodesk.Revit.DB.ExternalData;
#endif

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class ElementTypeDescriptor(ElementType elementType) : ElementDescriptor(elementType)
{
    public override void RegisterExtensions(IExtensionManager manager)
    {
#if REVIT2025_OR_GREATER
        manager.Register("GetRebarSpliceLapLengthMultiplier", () => Variants.Value(RebarSpliceTypeUtils.GetLapLengthMultiplier(elementType.Document, elementType.Id)));
        manager.Register("GetRebarSpliceShiftOption", () => Variants.Value(RebarSpliceTypeUtils.GetShiftOption(elementType.Document, elementType.Id)));
        manager.Register("GetRebarSpliceStaggerLengthMultiplier", () => Variants.Value(RebarSpliceTypeUtils.GetStaggerLengthMultiplier(elementType.Document, elementType.Id)));
        
        _ = nameof(RebarSpliceTypeUtils.SetLapLengthMultiplier); //Api compile-time compability check
        manager.Register("SetRebarSpliceLapLengthMultiplier", Variants.NotSupported);
        
        _ = nameof(RebarSpliceTypeUtils.SetShiftOption); //Api compile-time compability check
        manager.Register("SetRebarSpliceShiftOption", Variants.NotSupported);
        
        _ = nameof(RebarSpliceTypeUtils.SetStaggerLengthMultiplier); //Api compile-time compability check
        manager.Register("SetRebarSpliceStaggerLengthMultiplier", Variants.NotSupported);
#endif
#if REVIT2026_OR_GREATER
        manager.Register(nameof(CoordinationModelLinkUtils.GetCoordinationModelTypeData), () => Variants.Value(CoordinationModelLinkUtils.GetCoordinationModelTypeData(elementType.Document, elementType)));
        manager.Register("GetRebarCrankLengthMultiplier", () => Variants.Value(RebarCrankTypeUtils.GetCrankLengthMultiplier(elementType.Document, elementType.Id)));
        manager.Register("GetRebarCrankOffsetMultiplier", () => Variants.Value(RebarCrankTypeUtils.GetCrankOffsetMultiplier(elementType.Document, elementType.Id)));
        manager.Register("GetRebarCrankRatio", () => Variants.Value(RebarCrankTypeUtils.GetCrankRatio(elementType.Document, elementType.Id)));
        
        _ = nameof(RebarCrankTypeUtils.SetCrankLengthMultiplier); //Api compile-time compability check
        manager.Register("SetRebarCrankLengthMultiplier", Variants.NotSupported);
        
        _ = nameof(RebarCrankTypeUtils.SetCrankOffsetMultiplier); //Api compile-time compability check
        manager.Register("SetRebarCrankOffsetMultiplier", Variants.NotSupported);
        
        _ = nameof(RebarCrankTypeUtils.SetCrankRatio); //Api compile-time compability check
        manager.Register("SetRebarCrankRatio", Variants.NotSupported);
#endif
    }
}