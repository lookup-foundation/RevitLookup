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

using System.Collections;
using System.Numerics;
using System.Windows.Media;
using LookupEngine.Abstractions.Decomposition;
using LookupEngine.Descriptors;
using RevitLookup.UI.Playground.Mocks.Decomposition.Descriptors;
using RevitLookup.UI.Playground.Mocks.Decomposition.Samples;

namespace RevitLookup.UI.Playground.Mocks.Decomposition;

/// <summary>
///     Provides the descriptor lookup that redirects Playground sample types to their LookupEngine descriptors.
/// </summary>
public static class DescriptorsMap
{
    /// <summary>
    ///     Finds the descriptor that resolves the members of <paramref name="obj" />.
    /// </summary>
    /// <param name="obj">The object to resolve a descriptor for.</param>
    /// <param name="type">The exact type to match, or <see langword="null" /> to match approximately.</param>
    /// <returns>The <see cref="Descriptor" /> that resolves the members of <paramref name="obj" />.</returns>
    /// <remarks>
    ///     <para>
    ///         An exact match is required by the reflection engine to add extensions and resolve conflicts when invoking methods and properties; <paramref name="type" /> is not <see langword="null" /> in that case.
    ///     </para>
    ///     <para>
    ///         An approximate match is used to describe the object shown to the user; <paramref name="type" /> is <see langword="null" /> in that case.
    ///     </para>
    /// </remarks>
    public static Descriptor FindDescriptor(object? obj, Type? type)
    {
        return obj switch
        {
            bool value when type is null || type == typeof(bool) => new BooleanDescriptor(value),
            string value when type is null || type == typeof(string) => new StringDescriptor(value),
            Exception value when type is null || type == typeof(Exception) => new ExceptionDescriptor(value),
            Color color when type is null || type == typeof(Color) => new ColorMediaDescriptor(color),
            Vector3 value when type is null || type == typeof(Vector3) => new Vector3Descriptor(value),
            DeferredSample value when type is null || type == typeof(DeferredSample) => new DeferredSampleDescriptor(value),
            DisabledSample value when type is null || type == typeof(DisabledSample) => new DisabledSampleDescriptor(value),
            UnsupportedSample value when type is null || type == typeof(UnsupportedSample) => new UnsupportedSampleDescriptor(value),
            ExceptionSample value when type is null || type == typeof(ExceptionSample) => new ExceptionSampleDescriptor(value),
            MixedSample value when type is null || type == typeof(MixedSample) => new MixedSampleDescriptor(value),
            IEnumerable value => new EnumerableDescriptor(value),
            _ => new ObjectDescriptor(obj)
        };
    }
}
