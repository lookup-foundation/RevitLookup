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

public sealed class PerformanceAdviserDescriptor(PerformanceAdviser adviser) : Descriptor, IDescriptorConfigurator, IDescriptorConfigurator<Document>
{
    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(PerformanceAdviser.GetRuleDescription))
            .When(parameters => parameters.Length == 1 && parameters[0].ParameterType == typeof(int))
            .Resolve(() => ResolveIndexedPairs(adviser.GetNumberOfRules(), adviser.GetRuleDescription));
        configuration.Member(nameof(PerformanceAdviser.GetRuleId))
            .When(parameters => parameters.Length == 1 && parameters[0].ParameterType == typeof(int))
            .Resolve(() => ResolveIndexedPairs(adviser.GetNumberOfRules(), adviser.GetRuleId));
        configuration.Member(nameof(PerformanceAdviser.GetRuleName))
            .When(parameters => parameters.Length == 1 && parameters[0].ParameterType == typeof(int))
            .Resolve(() => ResolveIndexedPairs(adviser.GetNumberOfRules(), adviser.GetRuleName));
        configuration.Member(nameof(PerformanceAdviser.IsRuleEnabled))
            .When(parameters => parameters.Length == 1 && parameters[0].ParameterType == typeof(int))
            .Resolve(() => ResolveIndexedPairs(adviser.GetNumberOfRules(), adviser.IsRuleEnabled));
        configuration.Member(nameof(PerformanceAdviser.WillRuleCheckElements))
            .When(parameters => parameters.Length == 1 && parameters[0].ParameterType == typeof(int))
            .Resolve(() => ResolveIndexedPairs(adviser.GetNumberOfRules(), adviser.WillRuleCheckElements));
    }

    void IDescriptorConfigurator<Document>.Configure(IMemberConfigurator<Document> configuration)
    {
        configuration.Member(nameof(PerformanceAdviser.GetElementFilterFromRule))
            .When(parameters => parameters.Length == 2 && parameters[0].ParameterType == typeof(int))
            .Resolve(context => ResolveIndexedPairs(adviser.GetNumberOfRules(), i => adviser.GetElementFilterFromRule(i, context)));
        configuration.Member(nameof(PerformanceAdviser.ExecuteAllRules)).Defer(adviser.ExecuteAllRules);
    }
    
    private static IVariant ResolveIndexedPairs<TResult>(int capacity, Func<int, TResult> selector)
    {
        var variants = Variants.Values<KeyValuePair<int, TResult>>(capacity);
        for (var i = 0; i < capacity; i++)
        {
            variants.Add(new KeyValuePair<int, TResult>(i, selector(i)));
        }

        return variants.Consume();
    }
}