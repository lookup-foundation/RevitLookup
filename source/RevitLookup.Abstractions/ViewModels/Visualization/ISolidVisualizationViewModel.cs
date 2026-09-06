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

using System.Windows.Media;

namespace RevitLookup.Abstractions.ViewModels.Visualization;

/// <summary>
///     Defines a contract that represents the data for solid geometry visualization.
/// </summary>
public interface ISolidVisualizationViewModel
{
    /// <summary>
    ///     Gets or sets the scale factor of visualization.
    /// </summary>
    double Scale { get; set; }

    /// <summary>
    ///     Gets or sets the transparency level of visualization.
    /// </summary>
    double Transparency { get; set; }

    /// <summary>
    ///     Gets or sets the color of the solid faces.
    /// </summary>
    Color FaceColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the solid edges.
    /// </summary>
    Color EdgeColor { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the solid faces.
    /// </summary>
    bool ShowFace { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the solid edges.
    /// </summary>
    bool ShowEdge { get; set; }

    /// <summary>
    ///     Registers the visualization server for the specified solid.
    /// </summary>
    /// <param name="solid">The Revit <c>Solid</c> to visualize.</param>
    void RegisterServer(object solid);

    /// <summary>
    ///     Unregisters the solid visualization server.
    /// </summary>
    void UnregisterServer();
}
