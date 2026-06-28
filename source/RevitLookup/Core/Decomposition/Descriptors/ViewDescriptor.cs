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

using Autodesk.Revit.DB.Analysis;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using RevitLookup.Common.Extensions;
#if REVIT2024_OR_GREATER
#endif

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class ViewDescriptor(View view) : ElementDescriptor(view)
{
    public override void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(View.CanCategoryBeHidden)).Defer(() => ResolveCategories(view.Document.Settings.Categories, view.CanCategoryBeHidden));
        configuration.Member(nameof(View.CanCategoryBeHiddenTemporary)).Defer(() => ResolveCategories(view.Document.Settings.Categories, view.CanCategoryBeHiddenTemporary));
        configuration.Member(nameof(View.CanViewBeDuplicated)).Resolve(() => ResolveEnum<ViewDuplicateOption, bool>(view.CanViewBeDuplicated));
        configuration.Member(nameof(View.GetCategoryHidden)).Resolve(() => ResolveCategories(view.Document.Settings.Categories, view.GetCategoryHidden));
        configuration.Member(nameof(View.GetCategoryOverrides)).Resolve(() => ResolveCategories(view.Document.Settings.Categories, view.GetCategoryOverrides));
        configuration.Member(nameof(View.GetIsFilterEnabled)).Resolve(() => ResolveFilters(view.GetFilters(), view.Document, view.GetIsFilterEnabled));
        configuration.Member(nameof(View.GetFilterOverrides)).Resolve(() => ResolveFilters(view.GetFilters(), view.Document, view.GetFilterOverrides));
        configuration.Member(nameof(View.GetFilterVisibility)).Resolve(() => ResolveFilters(view.GetFilters(), view.Document, view.GetFilterVisibility));
        configuration.Member(nameof(View.GetWorksetVisibility)).Resolve(() => ResolveWorksets(new FilteredWorksetCollector(view.Document).OfKind(WorksetKind.UserWorkset).ToWorksets(), view.GetWorksetVisibility));
        configuration.Member(nameof(View.IsCategoryOverridable)).Defer(() => ResolveCategories(view.Document.Settings.Categories, view.IsCategoryOverridable));
        configuration.Member(nameof(View.IsFilterApplied)).Resolve(() => ResolveFilters(view.GetFilters(), view.Document, view.IsFilterApplied));
        configuration.Member(nameof(View.IsInTemporaryViewMode)).Resolve(() => ResolveEnum<TemporaryViewMode, bool>(view.IsInTemporaryViewMode));
        configuration.Member(nameof(View.IsValidViewTemplate)).Resolve(ResolveIsValidViewTemplate);
        configuration.Member(nameof(View.IsWorksetVisible)).Resolve(() => ResolveWorksets(new FilteredWorksetCollector(view.Document).OfKind(WorksetKind.UserWorkset).ToWorksets(), view.IsWorksetVisible));
        configuration.Member(nameof(View.SupportsWorksharingDisplayMode)).Resolve(() => ResolveEnum<WorksharingDisplayMode, bool>(view.SupportsWorksharingDisplayMode));
#if REVIT2022_OR_GREATER
        configuration.Member(nameof(View.GetColorFillSchemeId)).Resolve(() => ResolveCategories(view.Document.Settings.Categories, view.GetColorFillSchemeId));
#endif

        configuration.Extension("GetInstances").Register(() => view.Document.CollectElements(view.Id).Instances().ToElements());
        configuration.Extension(nameof(SpatialFieldManager.GetSpatialFieldManager)).Register(() => SpatialFieldManager.GetSpatialFieldManager(view));
        configuration.Extension(nameof(ReferenceableViewUtils.GetReferencedViewId)).Register(() => ReferenceableViewUtils.GetReferencedViewId(view.Document, view.Id));
        configuration.Extension(nameof(ReferenceableViewUtils.ChangeReferencedView)).NotSupported();
        configuration.Extension(nameof(ElementTransformUtils.GetTransformFromViewToView)).NotSupported();
#if REVIT2024_OR_GREATER
        configuration.Extension(nameof(DetailElementOrderUtils.GetDrawOrderForDetails)).NotSupported();
#endif
        return;

        IVariant ResolveIsValidViewTemplate()
        {
            var templates = view.Document.CollectElements()
                .Instances()
                .OfClass<View>()
                .Cast<View>()
                .Where(static element => element.IsTemplate)
                .ToArray();

            var variants = Variants.Values<bool>(templates.Length);
            foreach (var template in templates)
            {
                var result = view.IsValidViewTemplate(template.Id);
                variants.Add(result, $"{template.Name}: {result}");
            }

            return variants.Consume();
        }
    }
    
    private static IVariant ResolveFilters<TResult>(ICollection<ElementId> filters, Document document, Func<ElementId, TResult> selector)
    {
        var variants = Variants.Values<TResult>(filters.Count);
        var simple = typeof(TResult).IsPrimitiveType();
        foreach (var filterId in filters)
        {
            var filter = filterId.ToElement(document)!;
            var result = selector(filterId);
            variants.Add(result, simple ? $"{filter.Name}: {result}" : filter.Name);
        }

        return variants.Consume();
    }
    
    private static IVariant ResolveWorksets<TResult>(IList<Workset> worksets, Func<WorksetId, TResult> selector)
    {
        var variants = Variants.Values<TResult>(worksets.Count);
        var simple = typeof(TResult).IsPrimitiveType();
        foreach (var workset in worksets)
        {
            var result = selector(workset.Id);
            variants.Add(result, simple ? $"{workset.Name}: {result}" : workset.Name);
        }

        return variants.Consume();
    }
}