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
            nameof(ViewSchedule.GetStripedRowsColor) => ResolveStripedRowsColor,
            nameof(ViewSchedule.IsValidTextTypeId) => ResolveValidTextTypeId,
            nameof(ViewSchedule.GetDefaultNameForKeySchedule) => ResolveDefaultNameForKeySchedule,
            nameof(ViewSchedule.GetDefaultNameForMaterialTakeoff) => ResolveDefaultNameForMaterialTakeoff,
            nameof(ViewSchedule.GetDefaultNameForSchedule) => ResolveDefaultNameForSchedule,
            nameof(ViewSchedule.GetDefaultParameterNameForKeySchedule) => ResolveDefaultParameterNameForKeySchedule,
            nameof(ViewSchedule.IsValidCategoryForKeySchedule) => ResolveIsValidCategoryForKeySchedule,
            nameof(ViewSchedule.IsValidCategoryForMaterialTakeoff) => ResolveIsValidCategoryForMaterialTakeoff,
            nameof(ViewSchedule.IsValidCategoryForSchedule) => ResolveIsValidCategoryForSchedule,
            nameof(ViewSchedule.GetDefaultNameForKeynoteLegend) => ResolveGetDefaultNameForKeynoteLegend,
            nameof(ViewSchedule.GetDefaultNameForNoteBlock) => ResolveGetDefaultNameForNoteBlock,
            nameof(ViewSchedule.GetDefaultNameForRevisionSchedule) => ResolveGetDefaultNameForRevisionSchedule,
            nameof(ViewSchedule.GetDefaultNameForSheetList) => ResolveGetDefaultNameForSheetList,
            nameof(ViewSchedule.GetDefaultNameForViewList) => ResolveGetDefaultNameForViewList,
            nameof(ViewSchedule.GetValidFamiliesForNoteBlock) => ResolveGetValidFamiliesForNoteBlock,
            nameof(ViewSchedule.RefreshData) => Variants.Disabled,
#if REVIT2022_OR_GREATER
            nameof(ViewSchedule.GetScheduleInstances) => ResolveScheduleInstances,
            nameof(ViewSchedule.GetSegmentHeight) => ResolveSegmentHeight,
#endif
            _ => null
        };

        IVariant ResolveStripedRowsColor()
        {
            var patterns = Enum.GetValues(typeof(StripedRowPattern));
            var variants = Variants.Values<Color>(patterns.Length);

            foreach (StripedRowPattern pattern in patterns)
            {
                variants.Add(viewSchedule.GetStripedRowsColor(pattern), pattern.ToString());
            }

            return variants.Consume();
        }

        IVariant ResolveValidTextTypeId()
        {
            var types = viewSchedule.Document.CollectElements()
                .OfClass<TextNoteType>()
                .Types()
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

        IVariant ResolveIsValidCategoryForKeySchedule()
        {
            var categories = viewSchedule.Document.Settings.Categories;
            var variants = Variants.Values<bool>(categories.Size);
            foreach (Category category in categories)
            {
                var result = ViewSchedule.IsValidCategoryForKeySchedule(category.Id);
                variants.Add(result, $"{category.Name}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolveIsValidCategoryForMaterialTakeoff()
        {
            var categories = viewSchedule.Document.Settings.Categories;
            var variants = Variants.Values<bool>(categories.Size);
            foreach (Category category in categories)
            {
                var result = ViewSchedule.IsValidCategoryForMaterialTakeoff(category.Id);
                variants.Add(result, $"{category.Name}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolveIsValidCategoryForSchedule()
        {
            var categories = viewSchedule.Document.Settings.Categories;
            var variants = Variants.Values<bool>(categories.Size);
            foreach (Category category in categories)
            {
                var result = ViewSchedule.IsValidCategoryForSchedule(category.Id);
                variants.Add(result, $"{category.Name}: {result}");
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

        IVariant ResolveSegmentHeight()
        {
            var count = viewSchedule.GetSegmentCount();
            var variants = Variants.Values<double>(count);

            for (var i = 0; i < count; i++)
            {
                variants.Add(viewSchedule.GetSegmentHeight(i));
            }

            return variants.Consume();
        }
#endif
        IVariant ResolveGetDefaultNameForKeynoteLegend()
        {
            return Variants.Value(ViewSchedule.GetDefaultNameForKeynoteLegend(viewSchedule.Document));
        }

        IVariant ResolveGetDefaultNameForNoteBlock()
        {
            return Variants.Value(ViewSchedule.GetDefaultNameForNoteBlock(viewSchedule.Document));
        }

        IVariant ResolveGetDefaultNameForRevisionSchedule()
        {
            return Variants.Value(ViewSchedule.GetDefaultNameForRevisionSchedule(viewSchedule.Document));
        }

        IVariant ResolveGetDefaultNameForSheetList()
        {
            return Variants.Value(ViewSchedule.GetDefaultNameForSheetList(viewSchedule.Document));
        }

        IVariant ResolveGetDefaultNameForViewList()
        {
            return Variants.Value(ViewSchedule.GetDefaultNameForViewList(viewSchedule.Document));
        }

        IVariant ResolveGetValidFamiliesForNoteBlock()
        {
            return Variants.Value(ViewSchedule.GetValidFamiliesForNoteBlock(viewSchedule.Document));
        }
    }
}