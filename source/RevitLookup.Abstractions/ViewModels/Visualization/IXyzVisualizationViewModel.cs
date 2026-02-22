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
///     Represents the data for XYZ coordinate visualization.
/// </summary>
public interface IXyzVisualizationViewModel
{
    /// <summary>
    ///     The minimum length of coordinate axes.
    /// </summary>
    double MinAxisLength { get; }

    /// <summary>
    ///     The length of coordinate axes.
    /// </summary>
    double AxisLength { get; set; }

    /// <summary>
    ///     The transparency level of visualization.
    /// </summary>
    double Transparency { get; set; }

    /// <summary>
    ///     The color of X axis.
    /// </summary>
    Color XColor { get; set; }

    /// <summary>
    ///     The color of Y axis.
    /// </summary>
    Color YColor { get; set; }

    /// <summary>
    ///     The color of Z axis.
    /// </summary>
    Color ZColor { get; set; }

    /// <summary>
    ///     Whether to show the coordinate plane.
    /// </summary>
    bool ShowPlane { get; set; }

    /// <summary>
    ///     Whether to show the X axis.
    /// </summary>
    bool ShowXAxis { get; set; }

    /// <summary>
    ///     Whether to show the Y axis.
    /// </summary>
    bool ShowYAxis { get; set; }

    /// <summary>
    ///     Whether to show the Z axis.
    /// </summary>
    bool ShowZAxis { get; set; }

    /// <summary>
    ///     Register XYZ server.
    /// </summary>
    void RegisterServer(object xyz);

    /// <summary>
    ///     Unregister XYZ server.
    /// </summary>
    void UnregisterServer();
}