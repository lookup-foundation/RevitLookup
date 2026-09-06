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
///     Defines a contract that represents the data for face visualization.
/// </summary>
public interface IFaceVisualizationViewModel
{
    /// <summary>
    ///     Gets the minimum extrusion value for the face.
    /// </summary>
    double MinExtrusion { get; }

    /// <summary>
    ///     Gets or sets the extrusion value for the face.
    /// </summary>
    double Extrusion { get; set; }

    /// <summary>
    ///     Gets or sets the transparency level of visualization.
    /// </summary>
    double Transparency { get; set; }

    /// <summary>
    ///     Gets or sets the color of the face surface.
    /// </summary>
    Color SurfaceColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the face mesh.
    /// </summary>
    Color MeshColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the face normal vectors.
    /// </summary>
    Color NormalVectorColor { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the face surface.
    /// </summary>
    bool ShowSurface { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the face mesh grid.
    /// </summary>
    bool ShowMeshGrid { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the face normal vectors.
    /// </summary>
    bool ShowNormalVector { get; set; }

    /// <summary>
    ///     Registers the visualization server for the specified face.
    /// </summary>
    /// <param name="face">The Revit <c>Face</c> to visualize.</param>
    void RegisterServer(object face);

    /// <summary>
    ///     Unregisters the face visualization server.
    /// </summary>
    void UnregisterServer();
}
