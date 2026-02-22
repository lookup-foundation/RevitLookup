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
///     Represents the data for solid geometry visualization.
/// </summary>
public interface ISolidVisualizationViewModel
{
    /// <summary>
    ///     The scale factor of visualization.
    /// </summary>
    double Scale { get; set; }

    /// <summary>
    ///     The transparency level of visualization.
    /// </summary>
    double Transparency { get; set; }

    /// <summary>
    ///     The color of solid faces.
    /// </summary>
    Color FaceColor { get; set; }

    /// <summary>
    ///     The color of solid edges.
    /// </summary>
    Color EdgeColor { get; set; }

    /// <summary>
    ///     Whether to show solid faces.
    /// </summary>
    bool ShowFace { get; set; }

    /// <summary>
    ///     Whether to show solid edges.
    /// </summary>
    bool ShowEdge { get; set; }

    /// <summary>
    ///     Register Solid visualization server.
    /// </summary>
    void RegisterServer(object solid);

    /// <summary>
    ///     Unregister Solid visualization server.
    /// </summary>
    void UnregisterServer();
}