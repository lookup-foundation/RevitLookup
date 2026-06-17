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
using RevitLookup.UI.Playground.Mocks.Core.Decomposition.Descriptors;
using RevitLookup.UI.Playground.Mocks.Models;

namespace RevitLookup.UI.Playground.Mocks.Core.Decomposition;

public static class DescriptorsMap
{
    /// <summary>
    ///     Search for a descriptor by approximate or exact match
    /// </summary>
    /// <remarks>
    ///     Exact search is necessary for the reflection engine, to add extensions and resolve conflicts when calling methods and properties. Type is not null <p />
    ///     An approximate search is needed to describe the object, which is displayed to the user. Type is null
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