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

#if REVIT2024_OR_GREATER
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class EvaluatedParameterDescriptor : Descriptor, IDescriptorConfigurator, IDescriptorConfigurator<Document>
{
    private readonly EvaluatedParameter _parameter;

    public EvaluatedParameterDescriptor(EvaluatedParameter parameter)
    {
        _parameter = parameter;
        Name = parameter.Definition.Name;
    }

    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(EvaluatedParameter.Dispose)).Disable();
    }

    public void Configure(IMemberConfigurator<Document> configuration)
    {
        configuration.Member(nameof(EvaluatedParameter.AsValueString)).When(parameters => parameters.Length == 1).Resolve(context => _parameter.AsValueString(context));
        configuration.Member(nameof(EvaluatedParameter.AsValueString)).When(parameters => parameters.Length == 2).Resolve(context => _parameter.AsValueString(context, new FormatOptions()));
    }
}
#endif