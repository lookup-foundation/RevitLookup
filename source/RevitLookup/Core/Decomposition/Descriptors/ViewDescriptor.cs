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
#endif

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class ViewDescriptor(View view) : ElementDescriptor(view)
{
    public override Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(View.CanCategoryBeHidden) => () => VariantsResolver.ResolveCategories(view.Document.Settings.Categories, view.CanCategoryBeHidden),
            nameof(View.CanCategoryBeHiddenTemporary) => () => VariantsResolver.ResolveCategories(view.Document.Settings.Categories, view.CanCategoryBeHiddenTemporary),
            nameof(View.CanViewBeDuplicated) => () => VariantsResolver.ResolveEnum<ViewDuplicateOption, bool>(view.CanViewBeDuplicated),
            nameof(View.GetCategoryHidden) => () => VariantsResolver.ResolveCategories(view.Document.Settings.Categories, view.GetCategoryHidden),
            nameof(View.GetCategoryOverrides) => () => VariantsResolver.ResolveCategories(view.Document.Settings.Categories, view.GetCategoryOverrides),
            nameof(View.GetIsFilterEnabled) => () => VariantsResolver.ResolveFilters(view.GetFilters(), view.Document, view.GetIsFilterEnabled),
            nameof(View.GetFilterOverrides) => () => VariantsResolver.ResolveFilters(view.GetFilters(), view.Document, view.GetFilterOverrides),
            nameof(View.GetFilterVisibility) => () => VariantsResolver.ResolveFilters(view.GetFilters(), view.Document, view.GetFilterVisibility),
            nameof(View.GetWorksetVisibility) => () => VariantsResolver.ResolveWorksets(new FilteredWorksetCollector(view.Document).OfKind(WorksetKind.UserWorkset).ToWorksets(), view.GetWorksetVisibility),
            nameof(View.IsCategoryOverridable) => () => VariantsResolver.ResolveCategories(view.Document.Settings.Categories, view.IsCategoryOverridable),
            nameof(View.IsFilterApplied) => () => VariantsResolver.ResolveFilters(view.GetFilters(), view.Document, view.IsFilterApplied),
            nameof(View.IsInTemporaryViewMode) => () => VariantsResolver.ResolveEnum<TemporaryViewMode, bool>(view.IsInTemporaryViewMode),
            nameof(View.IsValidViewTemplate) => ResolveIsValidViewTemplate,
            nameof(View.IsWorksetVisible) => () => VariantsResolver.ResolveWorksets(new FilteredWorksetCollector(view.Document).OfKind(WorksetKind.UserWorkset).ToWorksets(), view.IsWorksetVisible),
            nameof(View.SupportsWorksharingDisplayMode) => () => VariantsResolver.ResolveEnum<WorksharingDisplayMode, bool>(view.SupportsWorksharingDisplayMode),
#if REVIT2022_OR_GREATER
            nameof(View.GetColorFillSchemeId) => () => VariantsResolver.ResolveCategories(view.Document.Settings.Categories, view.GetColorFillSchemeId),
#endif
            _ => null
        };

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
    }

    public override void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define("GetInstances").Register(() => Variants.Value(view.Document.CollectElements(view.Id).Instances().ToElements()));
        manager.Define(nameof(SpatialFieldManager.GetSpatialFieldManager)).Register(() => Variants.Value(SpatialFieldManager.GetSpatialFieldManager(view)));
        manager.Define(nameof(ReferenceableViewUtils.GetReferencedViewId)).Register(() => Variants.Value(ReferenceableViewUtils.GetReferencedViewId(view.Document, view.Id)));
        manager.Define(nameof(ReferenceableViewUtils.ChangeReferencedView)).AsNotSupported();
        manager.Define(nameof(ElementTransformUtils.GetTransformFromViewToView)).AsNotSupported();
#if REVIT2024_OR_GREATER
        manager.Define(nameof(DetailElementOrderUtils.GetDrawOrderForDetails)).AsNotSupported();
#endif
    }
}