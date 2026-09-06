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
///     Defines a contract that represents the data for polyline visualization.
/// </summary>
public interface IPolylineVisualizationViewModel
{
    /// <summary>
    ///     Gets the minimum thickness of the polyline.
    /// </summary>
    double MinThickness { get; }

    /// <summary>
    ///     Gets or sets the diameter of the polyline.
    /// </summary>
    double Diameter { get; set; }

    /// <summary>
    ///     Gets or sets the transparency level of visualization.
    /// </summary>
    double Transparency { get; set; }

    /// <summary>
    ///     Gets or sets the color of the polyline surface.
    /// </summary>
    Color SurfaceColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the polyline curve.
    /// </summary>
    Color CurveColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the polyline direction indicators.
    /// </summary>
    Color DirectionColor { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the polyline surface.
    /// </summary>
    bool ShowSurface { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the polyline curve.
    /// </summary>
    bool ShowCurve { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the polyline direction indicators.
    /// </summary>
    bool ShowDirection { get; set; }

    /// <summary>
    ///     Registers the visualization server for the specified curve or edge.
    /// </summary>
    /// <param name="curveOrEdge">The Revit <c>Curve</c> or <c>Edge</c> to visualize.</param>
    void RegisterServer(object curveOrEdge);

    /// <summary>
    ///     Unregisters the polyline visualization server.
    /// </summary>
    void UnregisterServer();
}
