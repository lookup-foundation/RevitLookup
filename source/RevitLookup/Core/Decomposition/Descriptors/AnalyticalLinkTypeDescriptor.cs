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
using Autodesk.Revit.DB.Structure;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class AnalyticalLinkTypeDescriptor(AnalyticalLinkType analyticalLinkType) : ElementDescriptor(analyticalLinkType)
{
    public override Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(AnalyticalLinkType.IsValidAnalyticalFixityState) => ResolveIsValidAnalyticalFixityState,
            _ => null
        };

        IVariant ResolveIsValidAnalyticalFixityState()
        {
            var values = Enum.GetValues(typeof(AnalyticalFixityState));
            var variants = Variants.Values<bool>(values.Length);
            foreach (AnalyticalFixityState state in values)
            {
                var result = AnalyticalLinkType.IsValidAnalyticalFixityState(state);
                variants.Add(result, $"{state}: {result}");
            }

            return variants.Consume();
        }
    }
}