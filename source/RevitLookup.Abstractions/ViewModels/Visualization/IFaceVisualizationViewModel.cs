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
///     Represents the data for face visualization.
/// </summary>
public interface IFaceVisualizationViewModel
{
    /// <summary>
    ///     The minimum extrusion value for face.
    /// </summary>
    double MinExtrusion { get; }

    /// <summary>
    ///     The extrusion value for face.
    /// </summary>
    double Extrusion { get; set; }

    /// <summary>
    ///     The transparency level of visualization.
    /// </summary>
    double Transparency { get; set; }

    /// <summary>
    ///     The color of face surface.
    /// </summary>
    Color SurfaceColor { get; set; }

    /// <summary>
    ///     The color of face mesh.
    /// </summary>
    Color MeshColor { get; set; }

    /// <summary>
    ///     The color of face normal vectors.
    /// </summary>
    Color NormalVectorColor { get; set; }

    /// <summary>
    ///     Whether to show face surface.
    /// </summary>
    bool ShowSurface { get; set; }

    /// <summary>
    ///     Whether to show face mesh grid.
    /// </summary>
    bool ShowMeshGrid { get; set; }

    /// <summary>
    ///     Whether to show face normal vectors.
    /// </summary>
    bool ShowNormalVector { get; set; }

    /// <summary>
    ///     Register Face visualization server.
    /// </summary>
    void RegisterServer(object face);

    /// <summary>
    ///     Unregister Face visualization server.
    /// </summary>
    void UnregisterServer();
}