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
using Autodesk.Revit.DB.DirectContext3D;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class CategoryDescriptor : Descriptor, IDescriptorResolver, IDescriptorExtension, IDescriptorExtension<Document>
{
    private readonly Category _category;

    public CategoryDescriptor(Category category)
    {
        _category = category;
        Name = category.Name;
    }

    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            "AllowsVisibilityControl" => ResolveAllowsVisibilityControl,
            "Visible" => ResolveVisible,
            nameof(Category.GetGraphicsStyle) => ResolveGetGraphicsStyle,
            nameof(Category.GetLinePatternId) => ResolveGetLinePatternId,
            nameof(Category.GetLineWeight) => ResolveGetLineWeight,
            _ => null
        };

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

        IVariant ResolveAllowsVisibilityControl()
        {
            return Variants.Value(_category.get_AllowsVisibilityControl(RevitContext.ActiveView), "Active view");
        }

        IVariant ResolveVisible()
        {
            return Variants.Value(_category.get_Visible(RevitContext.ActiveView), "Active view");
        }
    }

    public void RegisterExtensions(IExtensionManager manager)
    {
#if !REVIT2023_OR_GREATER
        manager.Register("BuiltInCategory", () => Variants.Value((BuiltInCategory) _category.Id.IntegerValue));
#endif
        manager.Register(nameof(DirectContext3DDocumentUtils.IsADirectContext3DHandleCategory),
            () => Variants.Value(DirectContext3DDocumentUtils.IsADirectContext3DHandleCategory(_category.Id)));
        manager.Register(nameof(ParameterFilterUtilities.GetAllFilterableCategories), () => Variants.Value(ParameterFilterUtilities.GetAllFilterableCategories()));

#if REVIT2024_OR_GREATER
        _ = nameof(SSEPointVisibilitySettings.SetVisibility); //Api compile-time compability check
        manager.Register("SetSSEPointVisibility", Variants.NotSupported);
#endif
    }

    public void RegisterExtensions(IExtensionManager<Document> manager)
    {
        manager.Register("GetElements", context => Variants.Value(context.CollectElements()
            .Instances()
#if REVIT2023_OR_GREATER
            .OfCategory(_category.BuiltInCategory)
#else
            .OfCategory((BuiltInCategory) _category.Id.IntegerValue)
#endif
            .ToElements()));
        manager.Register(nameof(DirectContext3DDocumentUtils.GetDirectContext3DHandleInstances),
            context => Variants.Value(DirectContext3DDocumentUtils.GetDirectContext3DHandleInstances(context, _category.Id)));
        manager.Register(nameof(DirectContext3DDocumentUtils.GetDirectContext3DHandleTypes),
            context => Variants.Value(DirectContext3DDocumentUtils.GetDirectContext3DHandleTypes(context, _category.Id)));
        manager.Register(nameof(ParameterFilterUtilities.GetFilterableParametersInCommon),
            context => Variants.Value(ParameterFilterUtilities.GetFilterableParametersInCommon(context, [_category.Id])));
#if REVIT2024_OR_GREATER
        manager.Register("GetSSEPointVisibility",
            context => Variants.Value(SSEPointVisibilitySettings.GetVisibility(context, _category.Id)));
#endif
    }
}