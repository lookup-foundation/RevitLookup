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
using Autodesk.Revit.DB.Analysis;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
#if REVIT2024_OR_GREATER
using Autodesk.Revit.DB.Structure;
#endif

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class ViewDescriptor(View view) : ElementDescriptor(view)
{
    public override Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(View.CanCategoryBeHidden) => ResolveCanCategoryBeHidden,
            nameof(View.CanCategoryBeHiddenTemporary) => ResolveCanCategoryBeHiddenTemporary,
            nameof(View.CanViewBeDuplicated) => ResolveCanViewBeDuplicated,
            nameof(View.GetCategoryHidden) => ResolveCategoryHidden,
            nameof(View.GetCategoryOverrides) => ResolveCategoryOverrides,
            nameof(View.GetIsFilterEnabled) => ResolveFilterEnabled,
            nameof(View.GetFilterOverrides) => ResolveFilterOverrides,
            nameof(View.GetFilterVisibility) => ResolveFilterVisibility,
            nameof(View.GetWorksetVisibility) => ResolveWorksetVisibility,
            nameof(View.IsCategoryOverridable) => ResolveIsCategoryOverridable,
            nameof(View.IsFilterApplied) => ResolveIsFilterApplied,
            nameof(View.IsInTemporaryViewMode) => ResolveIsInTemporaryViewMode,
            nameof(View.IsValidViewTemplate) => ResolveIsValidViewTemplate,
            nameof(View.IsWorksetVisible) => ResolveIsWorksetVisible,
            nameof(View.SupportsWorksharingDisplayMode) => ResolveSupportsWorksharingDisplayMode,
#if REVIT2022_OR_GREATER
            nameof(View.GetColorFillSchemeId) => ResolveColorFillSchemeId,
#endif
            _ => null
        };

        IVariant ResolveCanCategoryBeHidden()
        {
            var categories = view.Document.Settings.Categories;
            var variants = Variants.Values<bool>(categories.Size);
            foreach (Category category in categories)
            {
                var result = view.CanCategoryBeHidden(category.Id);
                variants.Add(result, $"{category.Name}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolveCanCategoryBeHiddenTemporary()
        {
            var categories = view.Document.Settings.Categories;
            var variants = Variants.Values<bool>(categories.Size);
            foreach (Category category in categories)
            {
                var result = view.CanCategoryBeHiddenTemporary(category.Id);
                variants.Add(result, $"{category.Name}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolveCanViewBeDuplicated()
        {
            var values = Enum.GetValues(typeof(ViewDuplicateOption));
            var variants = Variants.Values<bool>(values.Length);

            foreach (ViewDuplicateOption option in values)
            {
                var result = view.CanViewBeDuplicated(option);
                variants.Add(result, $"{option.ToString()}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolveCategoryHidden()
        {
            var categories = view.Document.Settings.Categories;
            var variants = Variants.Values<bool>(categories.Size);
            foreach (Category category in categories)
            {
                var result = view.GetCategoryHidden(category.Id);
                variants.Add(result, $"{category.Name}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolveCategoryOverrides()
        {
            var categories = view.Document.Settings.Categories;
            var variants = Variants.Values<OverrideGraphicSettings>(categories.Size);
            foreach (Category category in categories)
            {
                var result = view.GetCategoryOverrides(category.Id);
                variants.Add(result, category.Name);
            }

            return variants.Consume();
        }

        IVariant ResolveIsCategoryOverridable()
        {
            var categories = view.Document.Settings.Categories;
            var variants = Variants.Values<bool>(categories.Size);
            foreach (Category category in categories)
            {
                var result = view.IsCategoryOverridable(category.Id);
                variants.Add(result, $"{category.Name}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolveFilterOverrides()
        {
            var filters = view.GetFilters();
            var variants = Variants.Values<OverrideGraphicSettings>(filters.Count);
            foreach (var filterId in filters)
            {
                var filter = filterId.ToElement(view.Document)!;
                var result = view.GetFilterOverrides(filterId);
                variants.Add(result, filter.Name);
            }

            return variants.Consume();
        }

        IVariant ResolveFilterVisibility()
        {
            var filters = view.GetFilters();
            var variants = Variants.Values<bool>(filters.Count);
            foreach (var filterId in filters)
            {
                var filter = filterId.ToElement(view.Document)!;
                var result = view.GetFilterVisibility(filterId);
                variants.Add(result, $"{filter.Name}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolveFilterEnabled()
        {
            var filters = view.GetFilters();
            var variants = Variants.Values<bool>(filters.Count);
            foreach (var filterId in filters)
            {
                var filter = filterId.ToElement(view.Document)!;
                var result = view.GetIsFilterEnabled(filterId);
                variants.Add(result, $"{filter.Name}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolveIsFilterApplied()
        {
            var filters = view.GetFilters();
            var variants = Variants.Values<bool>(filters.Count);
            foreach (var filterId in filters)
            {
                var filter = filterId.ToElement(view.Document)!;
                var result = view.IsFilterApplied(filterId);
                variants.Add(result, $"{filter.Name}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolveIsInTemporaryViewMode()
        {
            var values = Enum.GetValues(typeof(TemporaryViewMode));
            var variants = Variants.Values<bool>(values.Length);

            foreach (TemporaryViewMode mode in values)
            {
                var result = view.IsInTemporaryViewMode(mode);
                variants.Add(result, $"{mode.ToString()}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolveIsValidViewTemplate()
        {
            var templates = view.Document.CollectElements()
                .Instances()
                .OfClass<View>()
                .Cast<View>()
                .Where(element => element.IsTemplate)
                .ToArray();
            
            var variants = Variants.Values<bool>(templates.Length);
            foreach (var template in templates)
            {
                var result = view.IsValidViewTemplate(template.Id);
                variants.Add(result, $"{template.Name}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolveIsWorksetVisible()
        {
            var workSets = new FilteredWorksetCollector(view.Document).OfKind(WorksetKind.UserWorkset).ToWorksets();
            var variants = Variants.Values<bool>(workSets.Count);
            foreach (var workSet in workSets)
            {
                var result = view.IsWorksetVisible(workSet.Id);
                variants.Add(result, $"{workSet.Name}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolveWorksetVisibility()
        {
            var workSets = new FilteredWorksetCollector(view.Document).OfKind(WorksetKind.UserWorkset).ToWorksets();
            var variants = Variants.Values<WorksetVisibility>(workSets.Count);
            foreach (var workSet in workSets)
            {
                var result = view.GetWorksetVisibility(workSet.Id);
                variants.Add(result, $"{workSet.Name}: {result}");
            }

            return variants.Consume();
        }

        IVariant ResolveSupportsWorksharingDisplayMode()
        {
            var values = Enum.GetValues(typeof(WorksharingDisplayMode));
            var variants = Variants.Values<bool>(values.Length);

            foreach (WorksharingDisplayMode mode in values)
            {
                var result = view.SupportsWorksharingDisplayMode(mode);
                variants.Add(result, $"{mode.ToString()}: {result}");
            }

            return variants.Consume();
        }
#if REVIT2022_OR_GREATER

        IVariant ResolveColorFillSchemeId()
        {
            var categories = view.Document.Settings.Categories;
            var variants = Variants.Values<ElementId>(categories.Size);
            foreach (Category category in categories)
            {
                var result = view.GetColorFillSchemeId(category.Id);
                variants.Add(result, category.Name);
            }

            return variants.Consume();
        }
#endif
    }

    public override void RegisterExtensions(IExtensionManager manager)
    {
        manager.Register(nameof(SpatialFieldManager.GetSpatialFieldManager), () => Variants.Value(SpatialFieldManager.GetSpatialFieldManager(view)));
        manager.Register("GetInstances", () => Variants.Value(view.Document.CollectElements(view.Id).Instances().ToElements()));
        manager.Register(nameof(ReferenceableViewUtils.GetReferencedViewId), () => Variants.Value(ReferenceableViewUtils.GetReferencedViewId(view.Document, view.Id)));
        manager.Register(nameof(ReferenceableViewUtils.ChangeReferencedView), Variants.NotSupported);

        _ = nameof(ElementTransformUtils.GetTransformFromViewToView); //Api compile-time compability check
        manager.Register("GetTransformFromViewToView", Variants.NotSupported);

        _ = nameof(ElementTransformUtils.CopyElements); //Api compile-time compability check
        manager.Register("CopyElementsBetweenViews", Variants.NotSupported);
#if REVIT2023_OR_GREATER
        manager.Register(nameof(BoundaryValidation.IsValidBoundaryOnView), Variants.NotSupported);
#endif
#if REVIT2024_OR_GREATER
        _ = nameof(RebarBendingDetail.Create); //Api compile-time compability check
        manager.Register("CreateBendingDetail", Variants.NotSupported);

#endif
    }
}