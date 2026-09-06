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

using Autodesk.Revit.DB.DirectContext3D;
using RevitLookup.Visualization.Rendering;

namespace RevitLookup.Visualization;

/// <summary>
///     Represents a Revit direct-context 3D server that renders <see cref="Mesh" /> visualization geometry into the active view.
/// </summary>
public sealed class MeshVisualizationServer : DirectContext3DServer
{
    private readonly RenderingBufferStorage _meshGridBuffer = new();

    private readonly RenderingBufferStorage _surfaceBuffer = new();
    private bool _drawMeshGrid;
    private bool _drawNormalVector;

    private bool _drawSurface;

    private double _extrusion;
    private Mesh _mesh = null!;
    private Color _meshColor = Color.InvalidColorValue;

    private RenderingBufferStorage[] _normalBuffers = [];
    private Color _normalColor = Color.InvalidColorValue;

    private Color _surfaceColor = Color.InvalidColorValue;
    private double _transparency;

    /// <inheritdoc />
    public override string GetName()
    {
        return "Mesh visualization server";
    }

    /// <inheritdoc />
    public override string GetDescription()
    {
        return "Mesh geometry visualization";
    }

    /// <inheritdoc />
    public override bool UseInTransparentPass(View view)
    {
        return _drawSurface && _transparency > 0;
    }

    /// <inheritdoc />
    public override Outline? GetBoundingBox(View view)
    {
        if (_mesh.Vertices.Count == 0)
        {
            return null;
        }

        var min = _mesh.Vertices[0];
        var max = _mesh.Vertices[0];

        for (var i = 1; i < _mesh.Vertices.Count; i++)
        {
            var vertex = _mesh.Vertices[i];
            min = new XYZ(Math.Min(min.X, vertex.X), Math.Min(min.Y, vertex.Y), Math.Min(min.Z, vertex.Z));
            max = new XYZ(Math.Max(max.X, vertex.X), Math.Max(max.Y, vertex.Y), Math.Max(max.Z, vertex.Z));
        }

        return new Outline(min, max);
    }

    /// <summary>
    ///     Registers the server for the specified mesh and enables rendering.
    /// </summary>
    /// <param name="mesh">The mesh to visualize.</param>
    public void Register(Mesh mesh)
    {
        _mesh = mesh;
        _normalBuffers = Enumerable.Range(0, _mesh.Vertices.Count)
            .Select(static _ => new RenderingBufferStorage())
            .ToArray();

        Register();
    }

    /// <summary>
    ///     Updates the color of the mesh surface and refreshes the open views.
    /// </summary>
    /// <param name="value">The new surface color.</param>
    public void UpdateSurfaceColor(Color value)
    {
        UpdateViews(() =>
        {
            _surfaceColor = value;
            HasEffectsUpdates = true;
        });
    }

    /// <summary>
    ///     Updates the color of the mesh grid and refreshes the open views.
    /// </summary>
    /// <param name="value">The new mesh grid color.</param>
    public void UpdateMeshGridColor(Color value)
    {
        UpdateViews(() =>
        {
            _meshColor = value;
            HasEffectsUpdates = true;
        });
    }

    /// <summary>
    ///     Updates the color of the mesh normal vectors and refreshes the open views.
    /// </summary>
    /// <param name="value">The new normal vector color.</param>
    public void UpdateNormalVectorColor(Color value)
    {
        UpdateViews(() =>
        {
            _normalColor = value;
            HasEffectsUpdates = true;
        });
    }

    /// <summary>
    ///     Updates the extrusion value of the mesh and refreshes the open views.
    /// </summary>
    /// <param name="value">The new extrusion value.</param>
    public void UpdateExtrusion(double value)
    {
        UpdateViews(() =>
        {
            _extrusion = value;
            HasGeometryUpdates = true;
        });
    }

    /// <summary>
    ///     Updates the transparency level of the visualization and refreshes the open views.
    /// </summary>
    /// <param name="value">The new transparency level.</param>
    public void UpdateTransparency(double value)
    {
        UpdateViews(() =>
        {
            _transparency = value;
            HasEffectsUpdates = true;
        });
    }

    /// <summary>
    ///     Updates whether the mesh surface is drawn and refreshes the open views.
    /// </summary>
    /// <param name="visible">A value indicating whether the surface is drawn.</param>
    public void UpdateSurfaceVisibility(bool visible)
    {
        UpdateViews(() => { _drawSurface = visible; });
    }

    /// <summary>
    ///     Updates whether the mesh grid is drawn and refreshes the open views.
    /// </summary>
    /// <param name="visible">A value indicating whether the mesh grid is drawn.</param>
    public void UpdateMeshGridVisibility(bool visible)
    {
        UpdateViews(() => { _drawMeshGrid = visible; });
    }

    /// <summary>
    ///     Updates whether the mesh normal vectors are drawn and refreshes the open views.
    /// </summary>
    /// <param name="visible">A value indicating whether the normal vectors are drawn.</param>
    public void UpdateNormalVectorVisibility(bool visible)
    {
        UpdateViews(() => { _drawNormalVector = visible; });
    }

    /// <inheritdoc />
    protected override bool AreBuffersValid()
    {
        return _surfaceBuffer.IsValid() && _meshGridBuffer.IsValid();
    }

    /// <inheritdoc />
    protected override void MapGeometryBuffer()
    {
        RenderHelper.MapSurfaceBuffer(_surfaceBuffer, _mesh, _extrusion);
        RenderHelper.MapMeshGridBuffer(_meshGridBuffer, _mesh, _extrusion);
        MapNormalsBuffer();
    }

    /// <inheritdoc />
    protected override void UpdateEffects()
    {
        _surfaceBuffer.EffectInstance ??= new EffectInstance(_surfaceBuffer.FormatBits);
        _meshGridBuffer.EffectInstance ??= new EffectInstance(_meshGridBuffer.FormatBits);

        _surfaceBuffer.EffectInstance.SetColor(_surfaceColor);
        _meshGridBuffer.EffectInstance.SetColor(_meshColor);
        _surfaceBuffer.EffectInstance.SetTransparency(_transparency);

        foreach (var normalBuffer in _normalBuffers)
        {
            normalBuffer.EffectInstance ??= new EffectInstance(normalBuffer.FormatBits);
            normalBuffer.EffectInstance.SetColor(_normalColor);
        }
    }

    /// <inheritdoc />
    protected override void RenderBuffers()
    {
        if (_drawSurface)
        {
            FlushTriangleBuffer(_surfaceBuffer, _transparency);
        }

        if (_drawMeshGrid)
        {
            FlushLineBuffer(_meshGridBuffer);
        }

        if (_drawNormalVector)
        {
            foreach (var buffer in _normalBuffers)
            {
                FlushLineBuffer(buffer);
            }
        }
    }

    /// <inheritdoc />
    protected override void DisposeBuffers()
    {
        _surfaceBuffer.Dispose();
        _meshGridBuffer.Dispose();
        foreach (var buffer in _normalBuffers)
        {
            buffer.Dispose();
        }
    }

    private void MapNormalsBuffer()
    {
        var area = RenderGeometryHelper.ComputeMeshSurfaceArea(_mesh);
        var offset = RenderGeometryHelper.InterpolateOffsetByArea(area);
        var normalLength = RenderGeometryHelper.InterpolateAxisLengthByArea(area);

        var normals = RenderGeometryHelper.GetMeshVertexNormals(_mesh);
        for (var i = 0; i < _mesh.Vertices.Count; i++)
        {
            var vertex = _mesh.Vertices[i];
            var buffer = _normalBuffers[i];
            var normal = normals[i];

            RenderHelper.MapNormalVectorBuffer(buffer, vertex + normal * (offset + _extrusion), normal, normalLength);
        }
    }
}
