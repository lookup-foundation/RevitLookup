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

using System.Windows.Controls;

namespace RevitLookup.Abstractions.Decomposition;

/// <summary>
///     Defines a contract that expands the context menu of a UI component.
/// </summary>
public interface IContextMenuConnector
{
    /// <summary>
    ///     Registers the context menu extension on the specified <paramref name="contextMenu" />.
    /// </summary>
    /// <param name="contextMenu">The context menu to extend.</param>
    /// <param name="serviceProvider">The service provider used to resolve the dependencies of the added menu items.</param>
    void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider);
}
