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

public sealed class FailureDefinitionAccessorDescriptor : Descriptor, IDescriptorResolver
{
    private readonly FailureDefinitionAccessor _definitionAccessor;

    public FailureDefinitionAccessorDescriptor(FailureDefinitionAccessor definitionAccessor)
    {
        _definitionAccessor = definitionAccessor;

        try
        {
            Name = $"{definitionAccessor.GetSeverity()}: {definitionAccessor.GetDescriptionText()}";
        }
        catch
        {
            // ignored
            // FailureDefinitionAccessor has not been properly initialized
        }
    }

    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(FailureDefinitionAccessor.IsResolutionApplicable) => () => VariantsResolver.ResolveEnum<FailureResolutionType, bool>(_definitionAccessor.IsResolutionApplicable),
            _ => null
        };
    }
}