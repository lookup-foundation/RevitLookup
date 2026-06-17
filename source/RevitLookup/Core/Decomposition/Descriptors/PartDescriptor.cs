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

using LookupEngine.Abstractions.Configuration;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class PartDescriptor(Part part) : ElementDescriptor(part), IDescriptorConfigurator<Document>
{
    public override void Configure(IMemberConfigurator configuration)
    {
        configuration.Extension(nameof(PartUtils.IsMergedPart)).Register(() => PartUtils.IsMergedPart(part));
        configuration.Extension(nameof(PartUtils.IsPartDerivedFromLink)).Register(() => PartUtils.IsPartDerivedFromLink(part));
        configuration.Extension(nameof(PartUtils.GetChainLengthToOriginal)).Register(() => PartUtils.GetChainLengthToOriginal(part));
        configuration.Extension(nameof(PartUtils.GetMergedParts)).Register(() => PartUtils.GetMergedParts(part));
    }

    void IDescriptorConfigurator<Document>.Configure(IMemberConfigurator<Document> configuration)
    {
        configuration.Extension(nameof(PartUtils.ArePartsValidForDivide)).Register(context => PartUtils.ArePartsValidForDivide(context, [part.Id]));
        configuration.Extension(nameof(PartUtils.FindMergeableClusters)).Register(context => PartUtils.FindMergeableClusters(context, [part.Id]));
        configuration.Extension(nameof(PartUtils.ArePartsValidForMerge)).Register(context => PartUtils.ArePartsValidForMerge(context, [part.Id]));
        configuration.Extension(nameof(PartUtils.GetAssociatedPartMaker)).Register(context => PartUtils.GetAssociatedPartMaker(context, part.Id));
        configuration.Extension(nameof(PartUtils.GetSplittingCurves)).Register(context => PartUtils.GetSplittingCurves(context, part.Id));
        configuration.Extension(nameof(PartUtils.GetSplittingElements)).Register(context => PartUtils.GetSplittingElements(context, part.Id));
        configuration.Extension(nameof(PartUtils.HasAssociatedParts)).Register(context => PartUtils.HasAssociatedParts(context, part.Id));
    }
}