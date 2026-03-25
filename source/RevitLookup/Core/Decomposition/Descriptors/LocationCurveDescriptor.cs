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

public sealed class LocationCurveDescriptor(LocationCurve locationCurve) : Descriptor, IDescriptorResolver
{
    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            "ElementsAtJoin" => ResolveElementsAtJoin,
            "JoinType" => ResolveJoinType,
            _ => null
        };

        IVariant ResolveElementsAtJoin()
        {
            return Variants.Values<ElementArray>(2)
                .Add(locationCurve.get_ElementsAtJoin(0), "Point 0")
                .Add(locationCurve.get_ElementsAtJoin(1), "Point 1")
                .Consume();
        }

        IVariant ResolveJoinType()
        {
            return Variants.Values<JoinType>(2)
                .Add(locationCurve.get_JoinType(0), "Point 0")
                .Add(locationCurve.get_JoinType(1), "Point 1")
                .Consume();
        }
    }
}