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
using System.Reflection;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class EvaluatedParameterDescriptor : Descriptor, IDescriptorResolver<Document>
{
    private readonly EvaluatedParameter _parameter;

    public EvaluatedParameterDescriptor(EvaluatedParameter parameter)
    {
        _parameter = parameter;
        Name = parameter.Definition.Name;
    }

    public Func<Document, IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(EvaluatedParameter.AsValueString) when parameters.Length == 1 => context => Variants.Value(_parameter.AsValueString(context)),
            nameof(EvaluatedParameter.AsValueString) when parameters.Length == 2 => context => Variants.Value(_parameter.AsValueString(context, new FormatOptions())),
            _ => null
        };
    }
}
#endif