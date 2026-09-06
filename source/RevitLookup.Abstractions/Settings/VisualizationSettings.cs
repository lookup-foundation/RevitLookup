using System.Text.Json.Serialization;
using System.Windows.Media;

namespace RevitLookup.Abstractions.Settings;

/// <summary>
///     Represents the visualization settings.
/// </summary>
[PublicAPI]
public sealed class VisualizationSettings
{
    /// <summary>
    ///     Gets or sets the bounding box visualization settings.
    /// </summary>
    [JsonPropertyName("BoundingBoxSettings")]
    public required BoundingBoxVisualizationSettings BoundingBoxSettings { get; set; }

    /// <summary>
    ///     Gets or sets the face visualization settings.
    /// </summary>
    [JsonPropertyName("FaceSettings")]
    public required FaceVisualizationSettings FaceSettings { get; set; }

    /// <summary>
    ///     Gets or sets the mesh visualization settings.
    /// </summary>
    [JsonPropertyName("MeshSettings")]
    public required MeshVisualizationSettings MeshSettings { get; set; }

    /// <summary>
    ///     Gets or sets the polyline visualization settings.
    /// </summary>
    [JsonPropertyName("PolylineSettings")]
    public required PolylineVisualizationSettings PolylineSettings { get; set; }

    /// <summary>
    ///     Gets or sets the CurveLoop visualization settings.
    /// </summary>
    [JsonPropertyName("CurveLoopSettings")]
    public required CurveLoopVisualizationSettings CurveLoopSettings { get; set; }

    /// <summary>
    ///     Gets or sets the solid visualization settings.
    /// </summary>
    [JsonPropertyName("SolidSettings")]
    public required SolidVisualizationSettings SolidSettings { get; set; }

    /// <summary>
    ///     Gets or sets the XYZ visualization settings.
    /// </summary>
    [JsonPropertyName("XyzSettings")]
    public required XyzVisualizationSettings XyzSettings { get; set; }
}

/// <summary>
///     Represents the bounding box visualization settings.
/// </summary>
[PublicAPI]
public class BoundingBoxVisualizationSettings
{
    /// <summary>
    ///     Gets or sets the transparency percentage of the bounding box surface.
    /// </summary>
    [JsonPropertyName("Transparency")]
    public double Transparency { get; set; }

    /// <summary>
    ///     Gets or sets the color of the bounding box surface.
    /// </summary>
    [JsonPropertyName("SurfaceColor")]
    public Color SurfaceColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the bounding box edges.
    /// </summary>
    [JsonPropertyName("EdgeColor")]
    public Color EdgeColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the bounding box axes.
    /// </summary>
    [JsonPropertyName("AxisColor")]
    public Color AxisColor { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the bounding box surface is rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowSurface")]
    public bool ShowSurface { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the bounding box edges are rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowEdge")]
    public bool ShowEdge { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the bounding box axes are rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowAxis")]
    public bool ShowAxis { get; set; }
}

/// <summary>
///     Represents the face visualization settings.
/// </summary>
[PublicAPI]
public class FaceVisualizationSettings
{
    /// <summary>
    ///     Gets or sets the transparency percentage of the face surface.
    /// </summary>
    [JsonPropertyName("Transparency")]
    public double Transparency { get; set; }

    /// <summary>
    ///     Gets or sets the extrusion distance used to give the flat face visible thickness.
    /// </summary>
    [JsonPropertyName("Extrusion")]
    public double Extrusion { get; set; }

    /// <summary>
    ///     Gets or sets the minimum extrusion distance applied when the model's tolerance would otherwise produce an imperceptible extrusion.
    /// </summary>
    [JsonPropertyName("MinExtrusion")]
    public double MinExtrusion { get; set; }

    /// <summary>
    ///     Gets or sets the color of the face surface.
    /// </summary>
    [JsonPropertyName("SurfaceColor")]
    public Color SurfaceColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the face mesh grid.
    /// </summary>
    [JsonPropertyName("MeshColor")]
    public Color MeshColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the face normal vector.
    /// </summary>
    [JsonPropertyName("NormalVectorColor")]
    public Color NormalVectorColor { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the face surface is rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowSurface")]
    public bool ShowSurface { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the face mesh grid is rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowMeshGrid")]
    public bool ShowMeshGrid { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the face normal vector is rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowNormalVector")]
    public bool ShowNormalVector { get; set; }
}

/// <summary>
///     Represents the mesh visualization settings.
/// </summary>
[PublicAPI]
public class MeshVisualizationSettings
{
    /// <summary>
    ///     Gets or sets the transparency percentage of the mesh surface.
    /// </summary>
    [JsonPropertyName("Transparency")]
    public double Transparency { get; set; }

    /// <summary>
    ///     Gets or sets the extrusion distance used to give the flat mesh visible thickness.
    /// </summary>
    [JsonPropertyName("Extrusion")]
    public double Extrusion { get; set; }

    /// <summary>
    ///     Gets or sets the minimum extrusion distance applied when the model's tolerance would otherwise produce an imperceptible extrusion.
    /// </summary>
    [JsonPropertyName("MinExtrusion")]
    public double MinExtrusion { get; set; }

    /// <summary>
    ///     Gets or sets the color of the mesh surface.
    /// </summary>
    [JsonPropertyName("SurfaceColor")]
    public Color SurfaceColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the mesh grid.
    /// </summary>
    [JsonPropertyName("MeshColor")]
    public Color MeshColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the mesh normal vector.
    /// </summary>
    [JsonPropertyName("NormalVectorColor")]
    public Color NormalVectorColor { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the mesh surface is rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowSurface")]
    public bool ShowSurface { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the mesh grid is rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowMeshGrid")]
    public bool ShowMeshGrid { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the mesh normal vector is rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowNormalVector")]
    public bool ShowNormalVector { get; set; }
}

/// <summary>
///     Represents the polyline visualization settings.
/// </summary>
[PublicAPI]
public class PolylineVisualizationSettings
{
    /// <summary>
    ///     Gets or sets the transparency percentage of the polyline surface.
    /// </summary>
    [JsonPropertyName("Transparency")]
    public double Transparency { get; set; }

    /// <summary>
    ///     Gets or sets the diameter of the rendered polyline tube.
    /// </summary>
    [JsonPropertyName("Diameter")]
    public double Diameter { get; set; }

    /// <summary>
    ///     Gets or sets the minimum thickness applied when the model's tolerance would otherwise produce an imperceptible curve.
    /// </summary>
    [JsonPropertyName("MinThickness")]
    public double MinThickness { get; set; }

    /// <summary>
    ///     Gets or sets the color of the polyline surface.
    /// </summary>
    [JsonPropertyName("SurfaceColor")]
    public Color SurfaceColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the polyline curve.
    /// </summary>
    [JsonPropertyName("CurveColor")]
    public Color CurveColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the polyline direction indicator.
    /// </summary>
    [JsonPropertyName("DirectionColor")]
    public Color DirectionColor { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the polyline surface is rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowSurface")]
    public bool ShowSurface { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the polyline curve is rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowCurve")]
    public bool ShowCurve { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the polyline direction indicator is rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowDirection")]
    public bool ShowDirection { get; set; }
}

/// <summary>
///     Represents the CurveLoop visualization settings.
/// </summary>
[PublicAPI]
public class CurveLoopVisualizationSettings
{
    /// <summary>
    ///     Gets or sets the transparency percentage of the curve loop surface.
    /// </summary>
    [JsonPropertyName("Transparency")]
    public double Transparency { get; set; }

    /// <summary>
    ///     Gets or sets the diameter of the rendered curve loop tube.
    /// </summary>
    [JsonPropertyName("Diameter")]
    public double Diameter { get; set; }

    /// <summary>
    ///     Gets or sets the minimum thickness applied when the model's tolerance would otherwise produce an imperceptible curve.
    /// </summary>
    [JsonPropertyName("MinThickness")]
    public double MinThickness { get; set; }

    /// <summary>
    ///     Gets or sets the color of the curve loop surface.
    /// </summary>
    [JsonPropertyName("SurfaceColor")]
    public Color SurfaceColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the curve loop.
    /// </summary>
    [JsonPropertyName("CurveColor")]
    public Color CurveColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the curve loop direction indicator.
    /// </summary>
    [JsonPropertyName("DirectionColor")]
    public Color DirectionColor { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the curve loop surface is rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowSurface")]
    public bool ShowSurface { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the curve loop is rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowCurve")]
    public bool ShowCurve { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the curve loop direction indicator is rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowDirection")]
    public bool ShowDirection { get; set; }
}

/// <summary>
///     Represents the solid visualization settings.
/// </summary>
[PublicAPI]
public sealed class SolidVisualizationSettings
{
    /// <summary>
    ///     Gets or sets the transparency percentage of the solid faces.
    /// </summary>
    [JsonPropertyName("Transparency")]
    public double Transparency { get; set; }

    /// <summary>
    ///     Gets or sets the scale percentage applied to the solid before rendering.
    /// </summary>
    [JsonPropertyName("Scale")]
    public double Scale { get; set; }

    /// <summary>
    ///     Gets or sets the color of the solid faces.
    /// </summary>
    [JsonPropertyName("FaceColor")]
    public Color FaceColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the solid edges.
    /// </summary>
    [JsonPropertyName("EdgeColor")]
    public Color EdgeColor { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the solid faces are rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowFace")]
    public bool ShowFace { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the solid edges are rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowEdge")]
    public bool ShowEdge { get; set; }
}

/// <summary>
///     Represents the XYZ visualization settings.
/// </summary>
[PublicAPI]
public class XyzVisualizationSettings
{
    /// <summary>
    ///     Gets or sets the transparency percentage of the XYZ plane.
    /// </summary>
    [JsonPropertyName("Transparency")]
    public double Transparency { get; set; }

    /// <summary>
    ///     Gets or sets the length of the rendered axes.
    /// </summary>
    [JsonPropertyName("AxisLength")]
    public double AxisLength { get; set; }

    /// <summary>
    ///     Gets or sets the minimum axis length applied when the model's tolerance would otherwise produce an imperceptible axis.
    /// </summary>
    [JsonPropertyName("MinAxisLength")]
    public double MinAxisLength { get; set; }

    /// <summary>
    ///     Gets or sets the color of the X axis.
    /// </summary>
    [JsonPropertyName("XColor")]
    public Color XColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the Y axis.
    /// </summary>
    [JsonPropertyName("YColor")]
    public Color YColor { get; set; }

    /// <summary>
    ///     Gets or sets the color of the Z axis.
    /// </summary>
    [JsonPropertyName("ZColor")]
    public Color ZColor { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the origin plane is rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowPlane")]
    public bool ShowPlane { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the X axis is rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowXAxis")]
    public bool ShowXAxis { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the Y axis is rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowYAxis")]
    public bool ShowYAxis { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the Z axis is rendered.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("ShowZAxis")]
    public bool ShowZAxis { get; set; }
}
