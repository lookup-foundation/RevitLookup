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
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class WorksetTableDescriptor : Descriptor, IDescriptorConfigurator
{
    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(WorksetTable.GetWorkset)).When(parameters => parameters?.Length == 1 && parameters[0].ParameterType == typeof(WorksetId)).Resolve(ResolveGetWorkset);
        return;

        IVariant ResolveGetWorkset()
        {
            var worksets = new FilteredWorksetCollector(RevitContext.ActiveDocument).ToWorksets();
            var variants = Variants.Values<Workset>(worksets.Count);

            foreach (var workset in worksets)
            {
                variants.Add(workset);
            }

            return variants.Consume();
        }
    }
}