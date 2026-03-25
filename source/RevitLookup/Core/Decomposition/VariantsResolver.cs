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

namespace RevitLookup.Core.Decomposition;

public static class VariantsResolver
{
    public static IVariant ResolveEnum<TEnum, TResult>(Func<TEnum, TResult> selector)
        where TEnum : struct, Enum
    {
        var values = Enum.GetValues<TEnum>();
        var variants = Variants.Values<TResult>(values.Length);
        var isPrimitiveType = IsPrimitiveType(typeof(TResult));

        foreach (var value in values)
        {
            var result = selector(value);
            variants.Add(result, isPrimitiveType ? $"{value}: {result}" : value.ToString());
        }

        return variants.Consume();
    }

    public static IVariant ResolveIndex<TResult>(int capacity, Func<int, TResult> selector)
    {
        var variants = Variants.Values<TResult>(capacity);
        var isPrimitiveType = IsPrimitiveType(typeof(TResult));

        for (var i = 0; i < capacity; i++)
        {
            var result = selector(i);
            variants.Add(result, isPrimitiveType ? $"Index {i}: {result}" : $"Index {i}");
        }

        return variants.Consume();
    }

    public static IVariant ResolveIndexedPairs<TResult>(int capacity, Func<int, TResult> selector)
    {
        var variants = Variants.Values<KeyValuePair<int, TResult>>(capacity);
        for (var i = 0; i < capacity; i++)
        {
            variants.Add(new KeyValuePair<int, TResult>(i, selector(i)));
        }

        return variants.Consume();
    }

    public static IVariant ResolveElementIds<TResult>(ICollection<ElementId> ids, Func<ElementId, TResult> selector)
    {
        var variants = Variants.Values<TResult>(ids.Count);
        foreach (var id in ids)
        {
            var result = selector(id);
            variants.Add(result, $"ID{id}");
        }

        return variants.Consume();
    }

    public static IVariant ResolvePhases<TResult>(PhaseArray phases, Func<ElementId, TResult> selector)
    {
        var variants = Variants.Values<TResult>(phases.Size);
        foreach (Phase phase in phases)
        {
            var result = selector(phase.Id);
            variants.Add(result, $"{phase.Name}: {result}");
        }

        return variants.Consume();
    }

    public static IVariant ResolveFamilyParameters<TResult>(FamilyParameterSet parameters, Func<FamilyParameter, TResult> selector)
    {
        var variants = Variants.Values<TResult>(parameters.Size);
        foreach (FamilyParameter parameter in parameters)
        {
            var result = selector(parameter);
            variants.Add(result, $"{parameter.Definition.Name}: {result}");
        }

        return variants.Consume();
    }

    public static IVariant ResolveScheduleFields<TResult>(
        IList<ScheduleFieldId> fields,
        Func<ScheduleFieldId, TResult> selector,
        Func<ScheduleFieldId, TResult, string> label)
    {
        var variants = Variants.Values<TResult>(fields.Count);
        foreach (var field in fields)
        {
            var result = selector(field);
            variants.Add(result, label(field, result));
        }

        return variants.Consume();
    }

    public static IVariant ResolveTableCells<TResult>(int rows, int columns, Func<int, int, TResult> selector)
    {
        var variants = Variants.Values<TResult>(rows * columns);
        var simple = IsPrimitiveType(typeof(TResult));
        for (var i = 0; i < columns; i++)
        {
            for (var j = 0; j < rows; j++)
            {
                var result = selector(j, i);
                variants.Add(result, simple ? $"Row {j}, Column {i}: {result}" : $"Row {j}, Column {i}");
            }
        }

        return variants.Consume();
    }

    public static IVariant ResolveCategories<TResult>(CategoryNameMap categories, Func<ElementId, TResult> selector)
    {
        var variants = Variants.Values<TResult>(categories.Size);
        var simple = IsPrimitiveType(typeof(TResult));
        foreach (Category category in categories)
        {
            var result = selector(category.Id);
            variants.Add(result, simple ? $"{category.Name}: {result}" : category.Name);
        }

        return variants.Consume();
    }

    public static IVariant ResolveFilters<TResult>(ICollection<ElementId> filters, Document document, Func<ElementId, TResult> selector)
    {
        var variants = Variants.Values<TResult>(filters.Count);
        var simple = IsPrimitiveType(typeof(TResult));
        foreach (var filterId in filters)
        {
            var filter = filterId.ToElement(document)!;
            var result = selector(filterId);
            variants.Add(result, simple ? $"{filter.Name}: {result}" : filter.Name);
        }

        return variants.Consume();
    }

    public static IVariant ResolveWorksets<TResult>(IList<Workset> worksets, Func<WorksetId, TResult> selector)
    {
        var variants = Variants.Values<TResult>(worksets.Count);
        var simple = IsPrimitiveType(typeof(TResult));
        foreach (var workset in worksets)
        {
            var result = selector(workset.Id);
            variants.Add(result, simple ? $"{workset.Name}: {result}" : workset.Name);
        }

        return variants.Consume();
    }

    private static bool IsPrimitiveType(Type type) => type.IsPrimitive || type.IsEnum || type == typeof(string);
}