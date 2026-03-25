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
using System.Windows;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class UiElementDescriptor(UIElement uiElement) : DependencyObjectDescriptor(uiElement)
{
    public override Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(UIElement.GetLocalValueEnumerator) => Variants.Empty<LocalValueEnumerator>,
            nameof(UIElement.CaptureMouse) => Variants.Disabled,
            nameof(UIElement.CaptureStylus) => Variants.Disabled,
            nameof(UIElement.Focus) => Variants.Disabled,
            "Enter" => Variants.Disabled,
            _ => null
        };
    }
}