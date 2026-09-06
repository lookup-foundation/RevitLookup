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
///     Defines a contract that represents the data for bounding box visualization.
/// </summary>
public interface IBoundingBoxVisualizationViewModel
{
    /// <summary>
    ///     Gets or sets the transparency level of visualization.
    /// </summary>
    double Transparency { get; set; }

    /// <summary>
    ///     Gets or sets the color of the bounding box surface.
    /// </summary>
    Color SurfaceColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the bounding box edges.
    /// </summary>
    Color EdgeColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the bounding box axes.
    /// </summary>
    Color AxisColor { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the bounding box surface.
    /// </summary>
    bool ShowSurface { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the bounding box edges.
    /// </summary>
    bool ShowEdge { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the bounding box axes.
    /// </summary>
    bool ShowAxis { get; set; }

    /// <summary>
    ///     Registers the visualization server for the specified bounding box.
    /// </summary>
    /// <param name="boundingBoxXyz">The Revit <c>BoundingBoxXYZ</c> to visualize.</param>
    void RegisterServer(object boundingBoxXyz);

    /// <summary>
    ///     Unregisters the bounding box visualization server.
    /// </summary>
    void UnregisterServer();
}
