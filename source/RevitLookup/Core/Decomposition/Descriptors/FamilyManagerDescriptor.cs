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

public sealed class FamilyManagerDescriptor(FamilyManager familyManager) : Descriptor, IDescriptorResolver, IDescriptorResolver<Document>
{
    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(FamilyManager.IsParameterLockable) => () => VariantsResolver.ResolveFamilyParameters(familyManager.Parameters, familyManager.IsParameterLockable),
            nameof(FamilyManager.IsParameterLocked) => () => VariantsResolver.ResolveFamilyParameters(familyManager.Parameters, familyManager.IsParameterLocked),
            _ => null
        };
    }

    Func<Document, IVariant>? IDescriptorResolver<Document>.Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(FamilyManager.GetAssociatedFamilyParameter) => ResolveGetAssociatedFamilyParameter,
            _ => null
        };

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
}