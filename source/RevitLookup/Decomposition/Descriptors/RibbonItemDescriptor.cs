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

namespace RevitLookup.Decomposition.Descriptors;

/// <summary>
///     Represents the <see cref="Autodesk.Revit.UI.RibbonItem" /> or <see cref="Autodesk.Windows.RibbonItem" /> exposed to LookupEngine.
/// </summary>
public sealed class RibbonItemDescriptor : Descriptor, IDescriptorCollector
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RibbonItemDescriptor" /> class.
    /// </summary>
    /// <param name="item">The Revit API ribbon item to expose.</param>
    public RibbonItemDescriptor(RibbonItem item)
    {
        Name = item.ItemText;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RibbonItemDescriptor" /> class.
    /// </summary>
    /// <param name="panel">The underlying ribbon item to expose.</param>
    public RibbonItemDescriptor(Autodesk.Windows.RibbonItem panel)
    {
        Name = panel.TextOverride;
    }
}
