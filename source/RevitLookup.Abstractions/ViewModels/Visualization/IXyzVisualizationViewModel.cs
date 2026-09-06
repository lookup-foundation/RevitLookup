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
///     Defines a contract that represents the data for XYZ coordinate visualization.
/// </summary>
public interface IXyzVisualizationViewModel
{
    /// <summary>
    ///     Gets the minimum length of the coordinate axes.
    /// </summary>
    double MinAxisLength { get; }

    /// <summary>
    ///     Gets or sets the length of the coordinate axes.
    /// </summary>
    double AxisLength { get; set; }

    /// <summary>
    ///     Gets or sets the transparency level of visualization.
    /// </summary>
    double Transparency { get; set; }

    /// <summary>
    ///     Gets or sets the color of the X axis.
    /// </summary>
    Color XColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the Y axis.
    /// </summary>
    Color YColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the Z axis.
    /// </summary>
    Color ZColor { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the coordinate plane.
    /// </summary>
    bool ShowPlane { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the X axis.
    /// </summary>
    bool ShowXAxis { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the Y axis.
    /// </summary>
    bool ShowYAxis { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the Z axis.
    /// </summary>
    bool ShowZAxis { get; set; }

    /// <summary>
    ///     Registers the visualization server for the specified point.
    /// </summary>
    /// <param name="xyz">The Revit <c>XYZ</c> point to visualize.</param>
    void RegisterServer(object xyz);

    /// <summary>
    ///     Unregisters the XYZ visualization server.
    /// </summary>
    void UnregisterServer();
}
