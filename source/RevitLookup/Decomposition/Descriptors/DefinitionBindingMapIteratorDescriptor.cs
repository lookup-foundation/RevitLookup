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

namespace RevitLookup.Decomposition.Descriptors;

/// <summary>
///     Represents the <see cref="DefinitionBindingMapIterator" /> exposed to LookupEngine.
/// </summary>
/// <param name="iterator">The definition binding map iterator positioned at the entry to expose.</param>
public sealed class DefinitionBindingMapIteratorDescriptor(DefinitionBindingMapIterator iterator) : Descriptor, IDescriptorRedirector
{
    private readonly object _object = new KeyValuePair<Definition, object?>(iterator.Key, iterator.Current);

    /// <inheritdoc />
    public bool TryRedirect(string target, out object result)
    {
        result = _object;
        return true;
    }
}
