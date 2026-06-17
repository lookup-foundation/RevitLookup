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

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class ScheduleDefinitionDescriptor(ScheduleDefinition scheduleDefinition) : Descriptor, IDescriptorConfigurator, IDescriptorConfigurator<Document>
{
    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(ScheduleDefinition.CanFilterByGlobalParameters)).Resolve(() => ResolveScheduleFields(scheduleDefinition.GetFieldOrder(), scheduleDefinition.CanFilterByGlobalParameters, (field, result) => $"{scheduleDefinition.GetField(field).GetName()}: {result}"));
        configuration.Member(nameof(ScheduleDefinition.CanFilterByParameterExistence)).Resolve(() => ResolveScheduleFields(scheduleDefinition.GetFieldOrder(), scheduleDefinition.CanFilterByParameterExistence, (field, result) => $"{scheduleDefinition.GetField(field).GetName()}: {result}"));
        configuration.Member(nameof(ScheduleDefinition.CanFilterBySubstring)).Resolve(() => ResolveScheduleFields(scheduleDefinition.GetFieldOrder(), scheduleDefinition.CanFilterBySubstring, (field, result) => $"{scheduleDefinition.GetField(field).GetName()}: {result}"));
        configuration.Member(nameof(ScheduleDefinition.CanFilterByValue)).Resolve(() => ResolveScheduleFields(scheduleDefinition.GetFieldOrder(), scheduleDefinition.CanFilterByValue, (field, result) => $"{scheduleDefinition.GetField(field).GetName()}: {result}"));
        configuration.Member(nameof(ScheduleDefinition.CanFilterByValuePresence)).Resolve(() => ResolveScheduleFields(scheduleDefinition.GetFieldOrder(), scheduleDefinition.CanFilterByValuePresence, (field, result) => $"{scheduleDefinition.GetField(field).GetName()}: {result}"));
        configuration.Member(nameof(ScheduleDefinition.CanSortByField)).Resolve(() => ResolveScheduleFields(scheduleDefinition.GetFieldOrder(), scheduleDefinition.CanSortByField, (field, result) => $"{scheduleDefinition.GetField(field).GetName()}: {result}"));
        configuration.Member(nameof(ScheduleDefinition.GetField)).Resolve(() => ResolveScheduleFields(scheduleDefinition.GetFieldOrder(), scheduleDefinition.GetField, (_, result) => result.GetName()));
        configuration.Member(nameof(ScheduleDefinition.GetFieldId)).Resolve(() => ResolveScheduleFields(scheduleDefinition.GetFieldOrder(), field => scheduleDefinition.GetFieldId(field.IntegerValue), (field, result) => $"{scheduleDefinition.GetField(field).GetName()}: {result}"));
        configuration.Member(nameof(ScheduleDefinition.GetFieldIndex)).Resolve(() => ResolveScheduleFields(scheduleDefinition.GetFieldOrder(), scheduleDefinition.GetFieldIndex, (field, result) => $"{scheduleDefinition.GetField(field).GetName()}: {result}"));
        configuration.Member(nameof(ScheduleDefinition.GetFilter)).Resolve(ResolveGetFilter);
        configuration.Member(nameof(ScheduleDefinition.GetSortGroupField)).Resolve(ResolveGetSortGroupField);
        return;

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

    void IDescriptorConfigurator<Document>.Configure(IMemberConfigurator<Document> configuration)
    {
        configuration.Member(nameof(ScheduleDefinition.IsValidCategoryForEmbeddedSchedule)).Resolve(ResolveValidCategoryForEmbeddedSchedule);
        return;

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
    
    private static IVariant ResolveScheduleFields<TResult>(
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
}