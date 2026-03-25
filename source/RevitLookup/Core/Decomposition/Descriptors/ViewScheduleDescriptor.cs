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

public sealed class ViewScheduleDescriptor(ViewSchedule viewSchedule) : ElementDescriptor(viewSchedule)
{
    public override Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(ViewSchedule.GetStripedRowsColor) => () => VariantsResolver.ResolveEnum<StripedRowPattern, Color>(viewSchedule.GetStripedRowsColor),
            nameof(ViewSchedule.IsValidTextTypeId) => ResolveValidTextTypeId,
            nameof(ViewSchedule.GetDefaultNameForKeySchedule) => ResolveDefaultNameForKeySchedule,
            nameof(ViewSchedule.GetDefaultNameForMaterialTakeoff) => ResolveDefaultNameForMaterialTakeoff,
            nameof(ViewSchedule.GetDefaultNameForSchedule) => ResolveDefaultNameForSchedule,
            nameof(ViewSchedule.GetDefaultParameterNameForKeySchedule) => ResolveDefaultParameterNameForKeySchedule,
            nameof(ViewSchedule.IsValidCategoryForKeySchedule) => () => VariantsResolver.ResolveCategories(viewSchedule.Document.Settings.Categories, ViewSchedule.IsValidCategoryForKeySchedule),
            nameof(ViewSchedule.IsValidCategoryForMaterialTakeoff) => () => VariantsResolver.ResolveCategories(viewSchedule.Document.Settings.Categories, ViewSchedule.IsValidCategoryForMaterialTakeoff),
            nameof(ViewSchedule.IsValidCategoryForSchedule) => () => VariantsResolver.ResolveCategories(viewSchedule.Document.Settings.Categories, ViewSchedule.IsValidCategoryForSchedule),
            nameof(ViewSchedule.GetDefaultNameForKeynoteLegend) => () => Variants.Value(ViewSchedule.GetDefaultNameForKeynoteLegend(viewSchedule.Document)),
            nameof(ViewSchedule.GetDefaultNameForNoteBlock) => () => Variants.Value(ViewSchedule.GetDefaultNameForNoteBlock(viewSchedule.Document)),
            nameof(ViewSchedule.GetDefaultNameForRevisionSchedule) => () => Variants.Value(ViewSchedule.GetDefaultNameForRevisionSchedule(viewSchedule.Document)),
            nameof(ViewSchedule.GetDefaultNameForSheetList) => () => Variants.Value(ViewSchedule.GetDefaultNameForSheetList(viewSchedule.Document)),
            nameof(ViewSchedule.GetDefaultNameForViewList) => () => Variants.Value(ViewSchedule.GetDefaultNameForViewList(viewSchedule.Document)),
            nameof(ViewSchedule.GetValidFamiliesForNoteBlock) => () => Variants.Value(ViewSchedule.GetValidFamiliesForNoteBlock(viewSchedule.Document)),
            nameof(ViewSchedule.RefreshData) => Variants.Disabled,
#if REVIT2022_OR_GREATER
            nameof(ViewSchedule.GetScheduleInstances) => ResolveScheduleInstances,
            nameof(ViewSchedule.GetSegmentHeight) => () => VariantsResolver.ResolveIndex(viewSchedule.GetSegmentCount(), viewSchedule.GetSegmentHeight),
#endif
            _ => null
        };

        IVariant ResolveValidTextTypeId()
        {
            var types = viewSchedule.Document.CollectElements()
                .Types()
                .OfClass<TextNoteType>()
                .ToElements();

            var variants = Variants.Values<bool>(types.Count);
            foreach (var type in types)
            {
                var result = viewSchedule.IsValidTextTypeId(type.Id);
                variants.Add(result, $"{type.Name}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolveDefaultNameForKeySchedule()
        {
            var categories = ViewSchedule.GetValidCategoriesForKeySchedule();
            var variants = Variants.Values<string>(categories.Count);
            foreach (var categoryId in categories)
            {
                variants.Add(ViewSchedule.GetDefaultNameForKeySchedule(viewSchedule.Document, categoryId));
            }

            return variants.Consume();
        }

        IVariant ResolveDefaultNameForMaterialTakeoff()
        {
            var categories = ViewSchedule.GetValidCategoriesForMaterialTakeoff();
            var variants = Variants.Values<string>(categories.Count);
            foreach (var categoryId in categories)
            {
                variants.Add(ViewSchedule.GetDefaultNameForMaterialTakeoff(viewSchedule.Document, categoryId));
            }

            return variants.Consume();
        }

        IVariant ResolveDefaultNameForSchedule()
        {
            var areas = viewSchedule.Document
                .CollectElements()
                .Instances()
                .OfClass<AreaScheme>()
                .ToArray();

            var categories = ViewSchedule.GetValidCategoriesForSchedule();
            var variants = Variants.Values<string>(categories.Count + areas.Length);
            var areaId = new ElementId(BuiltInCategory.OST_Areas);
            foreach (var categoryId in categories)
            {
                if (categoryId == areaId)
                {
                    foreach (var area in areas)
                    {
                        variants.Add(ViewSchedule.GetDefaultNameForSchedule(viewSchedule.Document, categoryId, area.Id));
                    }
                }
                else
                {
                    variants.Add(ViewSchedule.GetDefaultNameForSchedule(viewSchedule.Document, categoryId));
                }
            }

            return variants.Consume();
        }

        IVariant ResolveDefaultParameterNameForKeySchedule()
        {
            var categories = ViewSchedule.GetValidCategoriesForKeySchedule();
            var variants = Variants.Values<string>(categories.Count);
            var areaId = new ElementId(BuiltInCategory.OST_Areas);
            foreach (var categoryId in categories)
            {
                if (categoryId == areaId) continue;
                variants.Add(ViewSchedule.GetDefaultParameterNameForKeySchedule(viewSchedule.Document, categoryId));
            }

            return variants.Consume();
        }

#if REVIT2022_OR_GREATER
        IVariant ResolveScheduleInstances()
        {
            var count = viewSchedule.GetSegmentCount();
            var variants = Variants.Values<IList<ElementId>>(count);
            for (var i = -1; i < count; i++)
            {
                variants.Add(viewSchedule.GetScheduleInstances(i));
            }

            return variants.Consume();
        }
#endif
    }
}