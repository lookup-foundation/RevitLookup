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

public sealed class ViewScheduleDescriptor(ViewSchedule viewSchedule) : ElementDescriptor(viewSchedule)
{
    public override void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(ViewSchedule.Dispose)).Disable();
        configuration.Member(nameof(ViewSchedule.GetStripedRowsColor)).Resolve(() => ResolveEnum<StripedRowPattern, Color>(viewSchedule.GetStripedRowsColor));
        configuration.Member(nameof(ViewSchedule.IsValidTextTypeId)).Resolve(ResolveValidTextTypeId);
        configuration.Member(nameof(ViewSchedule.GetDefaultNameForKeySchedule)).Resolve(ResolveDefaultNameForKeySchedule);
        configuration.Member(nameof(ViewSchedule.GetDefaultNameForMaterialTakeoff)).Resolve(ResolveDefaultNameForMaterialTakeoff);
        configuration.Member(nameof(ViewSchedule.GetDefaultNameForSchedule)).Resolve(ResolveDefaultNameForSchedule);
        configuration.Member(nameof(ViewSchedule.GetDefaultParameterNameForKeySchedule)).Resolve(ResolveDefaultParameterNameForKeySchedule);
        configuration.Member(nameof(ViewSchedule.IsValidCategoryForKeySchedule)).Resolve(() => ResolveCategories(viewSchedule.Document.Settings.Categories, ViewSchedule.IsValidCategoryForKeySchedule));
        configuration.Member(nameof(ViewSchedule.IsValidCategoryForMaterialTakeoff)).Resolve(() => ResolveCategories(viewSchedule.Document.Settings.Categories, ViewSchedule.IsValidCategoryForMaterialTakeoff));
        configuration.Member(nameof(ViewSchedule.IsValidCategoryForSchedule)).Resolve(() => ResolveCategories(viewSchedule.Document.Settings.Categories, ViewSchedule.IsValidCategoryForSchedule));
        configuration.Member(nameof(ViewSchedule.GetDefaultNameForKeynoteLegend)).Resolve(() => ViewSchedule.GetDefaultNameForKeynoteLegend(viewSchedule.Document));
        configuration.Member(nameof(ViewSchedule.GetDefaultNameForNoteBlock)).Resolve(() => ViewSchedule.GetDefaultNameForNoteBlock(viewSchedule.Document));
        configuration.Member(nameof(ViewSchedule.GetDefaultNameForRevisionSchedule)).Resolve(() => ViewSchedule.GetDefaultNameForRevisionSchedule(viewSchedule.Document));
        configuration.Member(nameof(ViewSchedule.GetDefaultNameForSheetList)).Resolve(() => ViewSchedule.GetDefaultNameForSheetList(viewSchedule.Document));
        configuration.Member(nameof(ViewSchedule.GetDefaultNameForViewList)).Resolve(() => ViewSchedule.GetDefaultNameForViewList(viewSchedule.Document));
        configuration.Member(nameof(ViewSchedule.GetValidFamiliesForNoteBlock)).Resolve(() => ViewSchedule.GetValidFamiliesForNoteBlock(viewSchedule.Document));
        configuration.Member(nameof(ViewSchedule.RefreshData)).Disable();
#if REVIT2022_OR_GREATER
        configuration.Member(nameof(ViewSchedule.GetScheduleInstances)).Resolve(ResolveScheduleInstances);
        configuration.Member(nameof(ViewSchedule.GetSegmentHeight)).Resolve(() => ResolveRange(viewSchedule.GetSegmentCount(), viewSchedule.GetSegmentHeight));
#endif
        return;

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