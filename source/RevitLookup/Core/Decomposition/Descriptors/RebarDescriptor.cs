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

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class RebarDescriptor(Rebar rebar) : ElementDescriptor(rebar)
{
    public override void RegisterExtensions(IExtensionManager manager)
    {
#if REVIT2025_OR_GREATER
        manager.Register(nameof(RebarSpliceUtils.GetSpliceChain), () => Variants.Value(RebarSpliceUtils.GetSpliceChain(rebar)));

        RegisterNotSupportedExtensions();
        return;

        // Indicates API methods that exist but cannot produce a read-only value in RevitLookup
        void RegisterNotSupportedExtensions()
        {
            _ = nameof(RebarSpliceUtils.CanRebarBeSpliced);
            manager.Register("CanBeSpliced", Variants.NotSupported);
            
            _ = nameof(RebarSpliceUtils.SpliceRebar);
            manager.Register("Splice", Variants.NotSupported);

            manager.Register(nameof(RebarSpliceUtils.GetLapDirectionForSpliceGeometryAndPosition), Variants.NotSupported);
            manager.Register(nameof(RebarSpliceUtils.UnifyRebarsIntoOne), Variants.NotSupported);
            manager.Register(nameof(RebarSpliceUtils.GetSpliceGeometries), Variants.NotSupported);
        }
#endif
    }
}