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
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class ElevationMarkerDescriptor(ElevationMarker elevationMarker) : ElementDescriptor(elevationMarker)
{
    public override Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(ElevationMarker.IsAvailableIndex) => () => VariantsResolver.ResolveIndex(elevationMarker.MaximumViewCount, elevationMarker.IsAvailableIndex),
            nameof(ElevationMarker.GetViewId) => ResolveViewId,
            _ => null
        };

        IVariant ResolveViewId()
        {
            var variants = Variants.Values<ElementId>(elevationMarker.MaximumViewCount);
            for (var i = 0; i < elevationMarker.MaximumViewCount; i++)
            {
                if (elevationMarker.IsAvailableIndex(i)) continue;

                var result = elevationMarker.GetViewId(i);
                var element = result.ToElement(elevationMarker.Document);
                var name = element!.Name == string.Empty ? $"ID{element.Id}" : $"{element.Name}, ID{element.Id}";
                variants.Add(result, $"Index {i}: {name}");
            }

            return variants.Consume();
        }
    }
}