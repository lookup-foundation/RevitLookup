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

using Autodesk.Windows;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Decomposition.Descriptors;

/// <summary>
///     Represents the <see cref="Autodesk.Windows.RibbonTab" /> exposed to LookupEngine.
/// </summary>
public sealed class RibbonTabDescriptor : Descriptor, IDescriptorCollector
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RibbonTabDescriptor" /> class.
    /// </summary>
    /// <param name="ribbonTab">The ribbon tab to expose.</param>
    public RibbonTabDescriptor(RibbonTab ribbonTab)
    {
        Name = ribbonTab.Title;
    }
}
