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

using LookupEngine.Abstractions.Decomposition;
using RevitLookup.UI.Framework.Extensions;

namespace RevitLookup.Decomposition.Descriptors;

/// <summary>
///     Provides shared helpers that resolve a member into a set of variants.
/// </summary>
public abstract class ResolvingDescriptor : Descriptor
{
    /// <summary>
    ///     Resolves a member across all values of an enumeration.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type to iterate.</typeparam>
    /// <typeparam name="TResult">The type of the resolved value.</typeparam>
    /// <param name="selector">A function that evaluates the member for a given enumeration value.</param>
    /// <returns>A variant collection with one entry per enumeration value.</returns>
    protected static IVariant ResolveEnum<TEnum, TResult>(Func<TEnum, TResult> selector)
        where TEnum : struct, Enum
    {
        var values = Enum.GetValues<TEnum>();
        var variants = Variants.Values<TResult>(values.Length);
        var isPrimitiveType = typeof(TResult).IsPrimitiveType();

        foreach (var value in values)
        {
            var result = selector(value);
            variants.Add(result, isPrimitiveType ? $"{value}: {result}" : value.ToString());
        }

        return variants.Consume();
    }

    /// <summary>
    ///     Resolves a member across a zero-based index range.
    /// </summary>
    /// <typeparam name="TResult">The type of the resolved value.</typeparam>
    /// <param name="capacity">The exclusive upper bound of the index range, starting at zero.</param>
    /// <param name="selector">A function that evaluates the member for a given index.</param>
    /// <returns>A variant collection with one entry per index in the range.</returns>
    protected static IVariant ResolveRange<TResult>(int capacity, Func<int, TResult> selector)
    {
        var variants = Variants.Values<TResult>(capacity);
        var isPrimitiveType = typeof(TResult).IsPrimitiveType();

        for (var i = 0; i < capacity; i++)
        {
            var result = selector(i);
            variants.Add(result, isPrimitiveType ? $"Index {i}: {result}" : $"Index {i}");
        }

        return variants.Consume();
    }

    /// <summary>
    ///     Safely evaluates a predicate and returns its result.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate.</param>
    /// <returns><see langword="true" /> if the predicate evaluates to <see langword="true" />; otherwise, <see langword="false" />, including when evaluation throws.</returns>
    protected static bool SafeEvaluate(Func<bool> predicate)
    {
        try
        {
            return predicate.Invoke();
        }
        catch
        {
            return false;
        }
    }
}
