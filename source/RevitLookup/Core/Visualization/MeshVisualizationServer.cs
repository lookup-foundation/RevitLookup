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
using RevitLookup.Core.Visualization.Buffers;
using RevitLookup.Core.Visualization.Helpers;

namespace RevitLookup.Core.Visualization;

public sealed class MeshVisualizationServer : DirectContext3DServer
{
    private Mesh _mesh = null!;

    private RenderingBufferStorage[] _normalBuffers = [];

    private double _extrusion;
    private double _transparency;

    private Color _surfaceColor = Color.InvalidColorValue;
    private Color _meshColor = Color.InvalidColorValue;
    private Color _normalColor = Color.InvalidColorValue;

    private bool _drawSurface;
    private bool _drawMeshGrid;
    private bool _drawNormalVector;

    private readonly RenderingBufferStorage _surfaceBuffer = new();
    private readonly RenderingBufferStorage _meshGridBuffer = new();

    public override string GetName() => "Mesh visualization server";
    public override string GetDescription() => "Mesh geometry visualization";
    public override bool UseInTransparentPass(View view) => _drawSurface && _transparency > 0;

    public override Outline? GetBoundingBox(View view)
    {
        if (_mesh.Vertices.Count == 0) return null;

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

    public void Register(Mesh mesh)
    {
        _mesh = mesh;
        _normalBuffers = Enumerable.Range(0, _mesh.Vertices.Count)
            .Select(_ => new RenderingBufferStorage())
            .ToArray();

        Register();
    }

    public void UpdateSurfaceColor(Color value) => UpdateViews(() =>
    {
        _surfaceColor = value;
        HasEffectsUpdates = true;
    });

    public void UpdateMeshGridColor(Color value) => UpdateViews(() =>
    {
        _meshColor = value;
        HasEffectsUpdates = true;
    });

    public void UpdateNormalVectorColor(Color value) => UpdateViews(() =>
    {
        _normalColor = value;
        HasEffectsUpdates = true;
    });

    public void UpdateExtrusion(double value) => UpdateViews(() =>
    {
        _extrusion = value;
        HasGeometryUpdates = true;
    });

    public void UpdateTransparency(double value) => UpdateViews(() =>
    {
        _transparency = value;
        HasEffectsUpdates = true;
    });

    public void UpdateSurfaceVisibility(bool visible) => UpdateViews(() => { _drawSurface = visible; });

    public void UpdateMeshGridVisibility(bool visible) => UpdateViews(() => { _drawMeshGrid = visible; });

    public void UpdateNormalVectorVisibility(bool visible) => UpdateViews(() => { _drawNormalVector = visible; });

    protected override bool AreBuffersValid()
    {
        return _surfaceBuffer.IsValid() && _meshGridBuffer.IsValid();
    }

    protected override void MapGeometryBuffer()
    {
        RenderHelper.MapSurfaceBuffer(_surfaceBuffer, _mesh, _extrusion);
        RenderHelper.MapMeshGridBuffer(_meshGridBuffer, _mesh, _extrusion);
        MapNormalsBuffer();
    }

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

    protected override void RenderBuffers()
    {
        if (_drawSurface) FlushTriangleBuffer(_surfaceBuffer, _transparency);
        if (_drawMeshGrid) FlushLineBuffer(_meshGridBuffer);

        if (_drawNormalVector)
        {
            foreach (var buffer in _normalBuffers)
            {
                FlushLineBuffer(buffer);
            }
        }
    }

    private void MapNormalsBuffer()
    {
        var area = RenderGeometryHelper.ComputeMeshSurfaceArea(_mesh);
        var offset = RenderGeometryHelper.InterpolateOffsetByArea(area);
        var normalLength = RenderGeometryHelper.InterpolateAxisLengthByArea(area);

        for (var i = 0; i < _mesh.Vertices.Count; i++)
        {
            var vertex = _mesh.Vertices[i];
            var buffer = _normalBuffers[i];
            var normal = RenderGeometryHelper.GetMeshVertexNormal(_mesh, i, _mesh.DistributionOfNormals);

            RenderHelper.MapNormalVectorBuffer(buffer, vertex + normal * (offset + _extrusion), normal, normalLength);
        }
    }
}