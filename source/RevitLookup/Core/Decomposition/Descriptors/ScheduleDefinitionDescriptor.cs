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

public sealed class ScheduleDefinitionDescriptor(ScheduleDefinition scheduleDefinition) : Descriptor, IDescriptorResolver, IDescriptorResolver<Document>
{
    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(ScheduleDefinition.CanFilterByGlobalParameters) => () => VariantsResolver.ResolveScheduleFields(scheduleDefinition.GetFieldOrder(), scheduleDefinition.CanFilterByGlobalParameters, (field, result) => $"{scheduleDefinition.GetField(field).GetName()}: {result}"),
            nameof(ScheduleDefinition.CanFilterByParameterExistence) => () => VariantsResolver.ResolveScheduleFields(scheduleDefinition.GetFieldOrder(), scheduleDefinition.CanFilterByParameterExistence, (field, result) => $"{scheduleDefinition.GetField(field).GetName()}: {result}"),
            nameof(ScheduleDefinition.CanFilterBySubstring) => () => VariantsResolver.ResolveScheduleFields(scheduleDefinition.GetFieldOrder(), scheduleDefinition.CanFilterBySubstring, (field, result) => $"{scheduleDefinition.GetField(field).GetName()}: {result}"),
            nameof(ScheduleDefinition.CanFilterByValue) => () => VariantsResolver.ResolveScheduleFields(scheduleDefinition.GetFieldOrder(), scheduleDefinition.CanFilterByValue, (field, result) => $"{scheduleDefinition.GetField(field).GetName()}: {result}"),
            nameof(ScheduleDefinition.CanFilterByValuePresence) => () => VariantsResolver.ResolveScheduleFields(scheduleDefinition.GetFieldOrder(), scheduleDefinition.CanFilterByValuePresence, (field, result) => $"{scheduleDefinition.GetField(field).GetName()}: {result}"),
            nameof(ScheduleDefinition.CanSortByField) => () => VariantsResolver.ResolveScheduleFields(scheduleDefinition.GetFieldOrder(), scheduleDefinition.CanSortByField, (field, result) => $"{scheduleDefinition.GetField(field).GetName()}: {result}"),
            nameof(ScheduleDefinition.GetField) => () => VariantsResolver.ResolveScheduleFields(scheduleDefinition.GetFieldOrder(), scheduleDefinition.GetField, (_, result) => result.GetName()),
            nameof(ScheduleDefinition.GetFieldId) => () => VariantsResolver.ResolveScheduleFields(scheduleDefinition.GetFieldOrder(), field => scheduleDefinition.GetFieldId(field.IntegerValue), (field, result) => $"{scheduleDefinition.GetField(field).GetName()}: {result}"),
            nameof(ScheduleDefinition.GetFieldIndex) => () => VariantsResolver.ResolveScheduleFields(scheduleDefinition.GetFieldOrder(), scheduleDefinition.GetFieldIndex, (field, result) => $"{scheduleDefinition.GetField(field).GetName()}: {result}"),
            nameof(ScheduleDefinition.GetFilter) => ResolveGetFilter,
            nameof(ScheduleDefinition.GetSortGroupField) => ResolveGetSortGroupField,
            _ => null
        };

        IVariant ResolveGetFilter()
        {
            var count = scheduleDefinition.GetFilterCount();
            var variants = Variants.Values<ScheduleFilter>(count);
            for (var i = 0; i < count; i++)
            {
                variants.Add(scheduleDefinition.GetFilter(i));
            }

            return variants.Consume();
        }

        IVariant ResolveGetSortGroupField()
        {
            var count = scheduleDefinition.GetSortGroupFieldCount();
            var variants = Variants.Values<ScheduleSortGroupField>(count);
            for (var i = 0; i < count; i++)
            {
                variants.Add(scheduleDefinition.GetSortGroupField(i));
            }

            return variants.Consume();
        }
    }

    Func<Document, IVariant>? IDescriptorResolver<Document>.Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(ScheduleDefinition.IsValidCategoryForEmbeddedSchedule) => ResolveValidCategoryForEmbeddedSchedule,
            _ => null
        };

        IVariant ResolveValidCategoryForEmbeddedSchedule(Document context)
        {
            var categories = context.Settings.Categories;
            var variants = Variants.Values<bool>(categories.Size);
            foreach (Category category in categories)
            {
                if (scheduleDefinition.IsValidCategoryForEmbeddedSchedule(category.Id))
                {
                    variants.Add(true, category.Name);
                }
            }

            return variants.Consume();
        }
    }
}