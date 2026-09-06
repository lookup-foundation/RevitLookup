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
///     Defines a contract that represents the data for mesh visualization.
/// </summary>
public interface IMeshVisualizationViewModel
{
    /// <summary>
    ///     Gets the minimum extrusion value for the mesh.
    /// </summary>
    double MinExtrusion { get; }

    /// <summary>
    ///     Gets or sets the extrusion value for the mesh.
    /// </summary>
    double Extrusion { get; set; }

    /// <summary>
    ///     Gets or sets the transparency level of visualization.
    /// </summary>
    double Transparency { get; set; }

    /// <summary>
    ///     Gets or sets the color of the mesh surface.
    /// </summary>
    Color SurfaceColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the mesh grid.
    /// </summary>
    Color MeshColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the mesh normal vectors.
    /// </summary>
    Color NormalVectorColor { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the mesh surface.
    /// </summary>
    bool ShowSurface { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the mesh grid.
    /// </summary>
    bool ShowMeshGrid { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show the mesh normal vectors.
    /// </summary>
    bool ShowNormalVector { get; set; }

    /// <summary>
    ///     Registers the visualization server for the specified mesh.
    /// </summary>
    /// <param name="mesh">The Revit <c>Mesh</c> to visualize.</param>
    void RegisterServer(object mesh);

    /// <summary>
    ///     Unregisters the mesh visualization server.
    /// </summary>
    void UnregisterServer();
}
