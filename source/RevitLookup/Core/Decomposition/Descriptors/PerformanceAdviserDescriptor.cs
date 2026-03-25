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

public sealed class PerformanceAdviserDescriptor(PerformanceAdviser adviser) : Descriptor, IDescriptorResolver, IDescriptorResolver<Document>
{
    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(PerformanceAdviser.GetRuleDescription)
                when parameters.Length == 1 && parameters[0].ParameterType == typeof(int) => () => VariantsResolver.ResolveIndexedPairs(adviser.GetNumberOfRules(), adviser.GetRuleDescription),
            nameof(PerformanceAdviser.GetRuleId)
                when parameters.Length == 1 && parameters[0].ParameterType == typeof(int) => () => VariantsResolver.ResolveIndexedPairs(adviser.GetNumberOfRules(), adviser.GetRuleId),
            nameof(PerformanceAdviser.GetRuleName)
                when parameters.Length == 1 && parameters[0].ParameterType == typeof(int) => () => VariantsResolver.ResolveIndexedPairs(adviser.GetNumberOfRules(), adviser.GetRuleName),
            nameof(PerformanceAdviser.IsRuleEnabled)
                when parameters.Length == 1 && parameters[0].ParameterType == typeof(int) => () => VariantsResolver.ResolveIndexedPairs(adviser.GetNumberOfRules(), adviser.IsRuleEnabled),
            nameof(PerformanceAdviser.WillRuleCheckElements)
                when parameters.Length == 1 && parameters[0].ParameterType == typeof(int) => () => VariantsResolver.ResolveIndexedPairs(adviser.GetNumberOfRules(), adviser.WillRuleCheckElements),
            _ => null
        };
    }

    Func<Document, IVariant>? IDescriptorResolver<Document>.Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(PerformanceAdviser.GetElementFilterFromRule)
                when parameters.Length == 2 && parameters[0].ParameterType == typeof(int) => context => VariantsResolver.ResolveIndexedPairs(adviser.GetNumberOfRules(), i => adviser.GetElementFilterFromRule(i, context)),
            nameof(PerformanceAdviser.ExecuteAllRules)
                when parameters.Length == 1 && parameters[0].ParameterType == typeof(Document) => context => Variants.Value(adviser.ExecuteAllRules(context)),
            _ => null
        };
    }
}