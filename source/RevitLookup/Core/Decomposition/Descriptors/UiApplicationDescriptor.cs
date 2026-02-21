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

using Autodesk.Revit.UI;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class UiApplicationDescriptor : Descriptor, IDescriptorExtension
{
    public void RegisterExtensions(IExtensionManager manager)
    {
        manager.Register(nameof(UIThemeManager.CurrentTheme), () => Variants.Value(UIThemeManager.CurrentTheme));
#if REVIT2024_OR_GREATER
        manager.Register(nameof(UIThemeManager.CurrentCanvasTheme), () => Variants.Value(UIThemeManager.CurrentCanvasTheme));
        manager.Register(nameof(UIThemeManager.FollowSystemColorTheme), () => Variants.Value(UIThemeManager.FollowSystemColorTheme));
        manager.Register(nameof(UIThemeManager.GetThemeName), ResolveGetThemeName);
        return;

        IVariant ResolveGetThemeName()
        {
            var themes = Enum.GetValues<UITheme>();
            var values = Variants.Values<string>(themes.Length);

            foreach (var theme in themes)
            {
                var name = UIThemeManager.GetThemeName(theme);
                values.Add(name, $"{theme}: {name}");
            }

            return values.Consume();
        }
#endif
    }
}