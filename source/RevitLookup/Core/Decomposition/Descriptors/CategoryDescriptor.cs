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

using Autodesk.Revit.DB.DirectContext3D;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class CategoryDescriptor : Descriptor, IDescriptorConfigurator, IDescriptorConfigurator<Document>
{
    private readonly Category _category;

    public CategoryDescriptor(Category category)
    {
        _category = category;
        Name = category.Name;
    }

    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(Category.Dispose)).Disable();
        configuration.Member("AllowsVisibilityControl").Resolve(() => Variants.Value(_category.get_AllowsVisibilityControl(RevitContext.ActiveView), "Active view"));
        configuration.Member("Visible").Resolve(() => Variants.Value(_category.get_Visible(RevitContext.ActiveView), "Active view"));
        configuration.Member(nameof(Category.GetGraphicsStyle)).Resolve(ResolveGetGraphicsStyle);
        configuration.Member(nameof(Category.GetLinePatternId)).Resolve(ResolveGetLinePatternId);
        configuration.Member(nameof(Category.GetLineWeight)).Resolve(ResolveGetLineWeight);

        configuration.Extension(nameof(DirectContext3DDocumentUtils.IsADirectContext3DHandleCategory)).Register(() => DirectContext3DDocumentUtils.IsADirectContext3DHandleCategory(_category.Id));
        configuration.Extension(nameof(ParameterFilterUtilities.GetAllFilterableCategories)).AsStatic().Register(ParameterFilterUtilities.GetAllFilterableCategories);
#if !REVIT2023_OR_GREATER
        configuration.Extension("BuiltInCategory").Register(() => (BuiltInCategory) _category.Id.IntegerValue);
#endif
#if REVIT2024_OR_GREATER
        configuration.Extension("SetSSEPointVisibility").Map(nameof(SSEPointVisibilitySettings.SetVisibility)).NotSupported();
#endif
        return;

        IVariant ResolveGetLineWeight()
        {
            return Variants.Values<int?>(2)
                .Add(_category.GetLineWeight(GraphicsStyleType.Cut), "Cut")
                .Add(_category.GetLineWeight(GraphicsStyleType.Projection), "Projection")
                .Consume();
        }

        IVariant ResolveGetLinePatternId()
        {
            return Variants.Values<ElementId>(2)
                .Add(_category.GetLinePatternId(GraphicsStyleType.Cut), "Cut")
                .Add(_category.GetLinePatternId(GraphicsStyleType.Projection), "Projection")
                .Consume();
        }

        IVariant ResolveGetGraphicsStyle()
        {
            return Variants.Values<GraphicsStyle>(2)
                .Add(_category.GetGraphicsStyle(GraphicsStyleType.Cut), "Cut")
                .Add(_category.GetGraphicsStyle(GraphicsStyleType.Projection), "Projection")
                .Consume();
        }
    }

    void IDescriptorConfigurator<Document>.Configure(IMemberConfigurator<Document> configuration)
    {
        configuration.Extension(nameof(ParameterFilterUtilities.GetFilterableParametersInCommon)).Register(context => ParameterFilterUtilities.GetFilterableParametersInCommon(context, [_category.Id]));
        configuration.Extension("GetElements").Register(context => context.CollectElements()
            .Instances()
#if REVIT2023_OR_GREATER
            .OfCategory(_category.BuiltInCategory)
#else
            .OfCategory((BuiltInCategory) _category.Id.IntegerValue)
#endif
            .ToElements());

        if (DirectContext3DDocumentUtils.IsADirectContext3DHandleCategory(_category.Id))
        {
            configuration.Extension(nameof(DirectContext3DDocumentUtils.GetDirectContext3DHandleInstances)).Register(context => DirectContext3DDocumentUtils.GetDirectContext3DHandleInstances(context, _category.Id));
            configuration.Extension(nameof(DirectContext3DDocumentUtils.GetDirectContext3DHandleTypes)).Register(context => DirectContext3DDocumentUtils.GetDirectContext3DHandleTypes(context, _category.Id));
        }

#if REVIT2024_OR_GREATER
        configuration.Extension("GetSSEPointVisibility").Register(context => SSEPointVisibilitySettings.GetVisibility(context, _category.Id));
#endif
    }
}