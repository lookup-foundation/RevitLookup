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

using Autodesk.Revit.DB.Mechanical;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class MepSectionDescriptor(MEPSection mepSection) : Descriptor, IDescriptorConfigurator
{
    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(MEPSection.GetElementIds)).Resolve(() => ResolveElementIds(mepSection.GetElementIds(), id => id));
        configuration.Member(nameof(MEPSection.GetCoefficient)).Resolve(() => ResolveElementIds(mepSection.GetElementIds(), mepSection.GetCoefficient));
        configuration.Member(nameof(MEPSection.GetPressureDrop)).Resolve(() => ResolveElementIds(mepSection.GetElementIds(), mepSection.GetPressureDrop));
        configuration.Member(nameof(MEPSection.GetSegmentLength)).Resolve(() => ResolveElementIds(mepSection.GetElementIds(), mepSection.GetSegmentLength));
        configuration.Member(nameof(MEPSection.IsMain)).Resolve(() => ResolveElementIds(mepSection.GetElementIds(), mepSection.IsMain));
    }
    
    private static IVariant ResolveElementIds<TResult>(ICollection<ElementId> ids, Func<ElementId, TResult> selector)
    {
        var variants = Variants.Values<TResult>(ids.Count);
        foreach (var id in ids)
        {
            var result = selector(id);
            variants.Add(result, $"ID{id}");
        }

        return variants.Consume();
    }
}