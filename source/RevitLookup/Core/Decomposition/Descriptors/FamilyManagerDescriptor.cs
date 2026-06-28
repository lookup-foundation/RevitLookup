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

public sealed class FamilyManagerDescriptor(FamilyManager familyManager) : Descriptor, IDescriptorConfigurator, IDescriptorConfigurator<Document>
{
    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(FamilyManager.Dispose)).Disable();
        configuration.Member(nameof(FamilyManager.IsParameterLockable)).Resolve(() => ResolveFamilyParameters(familyManager.Parameters, familyManager.IsParameterLockable));
        configuration.Member(nameof(FamilyManager.IsParameterLocked)).Resolve(() => ResolveFamilyParameters(familyManager.Parameters, familyManager.IsParameterLocked));
    }

    void IDescriptorConfigurator<Document>.Configure(IMemberConfigurator<Document> configuration)
    {
        configuration.Member(nameof(FamilyManager.GetAssociatedFamilyParameter)).Resolve(ResolveGetAssociatedFamilyParameter);
        return;

        IVariant ResolveGetAssociatedFamilyParameter(Document context)
        {
            var elements = context.CollectElements().Types()
                .UnionWith(context.CollectElements().Instances())
                .ToElements();

            var variants = Variants.Values<KeyValuePair<Parameter, FamilyParameter>>(elements.Count);
            foreach (var element in elements)
            {
                foreach (Parameter parameter in element.Parameters)
                {
                    var familyParameter = familyManager.GetAssociatedFamilyParameter(parameter);
                    if (familyParameter is not null)
                    {
                        variants.Add(new KeyValuePair<Parameter, FamilyParameter>(parameter, familyParameter));
                    }
                }
            }

            return variants.Consume();
        }
    }
    
    private static IVariant ResolveFamilyParameters<TResult>(FamilyParameterSet parameters, Func<FamilyParameter, TResult> selector)
    {
        var variants = Variants.Values<TResult>(parameters.Size);
        foreach (FamilyParameter parameter in parameters)
        {
            var result = selector(parameter);
            variants.Add(result, $"{parameter.Definition.Name}: {result}");
        }

        return variants.Consume();
    }
}