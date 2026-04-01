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
using System.Windows.Media;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public class DependencyObjectDescriptor(DependencyObject dependencyObject) : Descriptor, IDescriptorResolver, IDescriptorExtension
{
    public virtual Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(DependencyObject.GetLocalValueEnumerator) => Variants.Empty<LocalValueEnumerator?>,
            _ => null
        };
    }

    public virtual void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define("GetVisualParent").Register(() => Variants.Value(VisualTreeHelper.GetParent(dependencyObject)));
        manager.Define("GetVisualChild").Register(RegisterGetVisualChild);
        manager.Define("GetVisualChildrenCount").Register(() => Variants.Value(VisualTreeHelper.GetChildrenCount(dependencyObject)));
        manager.Define("GetLogicalParent").Register(() => Variants.Value(LogicalTreeHelper.GetParent(dependencyObject)));
        manager.Define("GetLogicalChildren").Register(() => Variants.Value(LogicalTreeHelper.GetChildren(dependencyObject)));
        return;

        IVariant RegisterGetVisualChild()
        {
            var count = VisualTreeHelper.GetChildrenCount(dependencyObject);
            var variants = Variants.Values<DependencyObject>(count);
            for (var i = 0; i < count; i++)
            {
                variants.Add(VisualTreeHelper.GetChild(dependencyObject, i));
            }

            return variants.Consume();
        }
    }
}