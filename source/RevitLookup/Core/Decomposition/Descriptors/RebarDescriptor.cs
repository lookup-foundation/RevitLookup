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

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class RebarDescriptor(Rebar rebar) : ElementDescriptor(rebar)
{
    public override void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(Rebar.GetFullGeometryForView)).Resolve(() => rebar.GetFullGeometryForView(RevitContext.ActiveView));

#if REVIT2025_OR_GREATER
        configuration.Extension("CanBeSpliced").Map(nameof(RebarSpliceUtils.CanRebarBeSpliced)).NotSupported();
        configuration.Extension("Splice").Map(nameof(RebarSpliceUtils.SpliceRebar)).NotSupported();
        configuration.Extension(nameof(RebarSpliceUtils.GetSpliceChain)).Register(() => RebarSpliceUtils.GetSpliceChain(rebar));
        configuration.Extension(nameof(RebarSpliceUtils.GetLapDirectionForSpliceGeometryAndPosition)).NotSupported();
        configuration.Extension(nameof(RebarSpliceUtils.UnifyRebarsIntoOne)).NotSupported();
        configuration.Extension(nameof(RebarSpliceUtils.GetSpliceGeometries)).NotSupported();
#endif
    }
}