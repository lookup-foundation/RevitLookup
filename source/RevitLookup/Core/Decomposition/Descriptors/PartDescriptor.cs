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
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class PartDescriptor(Part part) : ElementDescriptor(part), IDescriptorExtension<Document>
{
    public override Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return null;
    }

    public override void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define(nameof(PartUtils.IsMergedPart)).Register(() => Variants.Value(PartUtils.IsMergedPart(part)));
        manager.Define(nameof(PartUtils.IsPartDerivedFromLink)).Register(() => Variants.Value(PartUtils.IsPartDerivedFromLink(part)));
        manager.Define(nameof(PartUtils.GetChainLengthToOriginal)).Register(() => Variants.Value(PartUtils.GetChainLengthToOriginal(part)));
        manager.Define(nameof(PartUtils.GetMergedParts)).Register(() => Variants.Value(PartUtils.GetMergedParts(part)));
    }

    public void RegisterExtensions(IExtensionManager<Document> manager)
    {
        manager.Define(nameof(PartUtils.ArePartsValidForDivide)).Register(context => Variants.Value(PartUtils.ArePartsValidForDivide(context, [part.Id])));
        manager.Define(nameof(PartUtils.FindMergeableClusters)).Register(context => Variants.Value(PartUtils.FindMergeableClusters(context, [part.Id])));
        manager.Define(nameof(PartUtils.ArePartsValidForMerge)).Register(context => Variants.Value(PartUtils.ArePartsValidForMerge(context, [part.Id])));
        manager.Define(nameof(PartUtils.GetAssociatedPartMaker)).Register(context => Variants.Value(PartUtils.GetAssociatedPartMaker(context, part.Id)));
        manager.Define(nameof(PartUtils.GetSplittingCurves)).Register(context => Variants.Value(PartUtils.GetSplittingCurves(context, part.Id)));
        manager.Define(nameof(PartUtils.GetSplittingElements)).Register(context => Variants.Value(PartUtils.GetSplittingElements(context, part.Id)));
        manager.Define(nameof(PartUtils.HasAssociatedParts)).Register(context => Variants.Value(PartUtils.HasAssociatedParts(context, part.Id)));
    }
}