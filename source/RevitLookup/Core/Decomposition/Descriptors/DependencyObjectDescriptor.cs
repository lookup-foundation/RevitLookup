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

using System.Windows;
using System.Windows.Media;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public class DependencyObjectDescriptor(DependencyObject dependencyObject) : Descriptor, IDescriptorConfigurator
{
    public virtual void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(DependencyObject.GetLocalValueEnumerator)).Resolve(Variants.Empty<LocalValueEnumerator?>);

        configuration.Extension("GetVisualParent").Register(() => VisualTreeHelper.GetParent(dependencyObject));
        configuration.Extension("GetVisualChild").Register(RegisterGetVisualChild);
        configuration.Extension("GetVisualChildrenCount").Register(() => VisualTreeHelper.GetChildrenCount(dependencyObject));
        configuration.Extension("GetLogicalParent").Register(() => LogicalTreeHelper.GetParent(dependencyObject));
        configuration.Extension("GetLogicalChildren").Register(() => LogicalTreeHelper.GetChildren(dependencyObject));
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