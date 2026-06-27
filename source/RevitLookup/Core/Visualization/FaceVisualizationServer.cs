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

public sealed class FaceVisualizationServer : DirectContext3DServer
{
    private Face _face = null!;

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
    private readonly RenderingBufferStorage _normalBuffer = new();

    public override string GetName() => "Face visualization server";
    public override string GetDescription() => "Face geometry visualization";
    public override bool UseInTransparentPass(View view) => _drawSurface && _transparency > 0;

    public override Outline? GetBoundingBox(View view)
    {
        if (_face.Reference is null) return null;

        var element = _face.Reference.ElementId.ToElement(view.Document)!;
        var boundingBox = element.get_BoundingBox(null) ?? element.get_BoundingBox(view);
        if (boundingBox is null) return null;

        var minPoint = boundingBox.Transform.OfPoint(boundingBox.Min);
        var maxPoint = boundingBox.Transform.OfPoint(boundingBox.Max);

        return new Outline(minPoint, maxPoint);
    }

    public void Register(Face face)
    {
        _face = face;
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
        return _surfaceBuffer.IsValid() && _meshGridBuffer.IsValid() && _normalBuffer.IsValid();
    }

    protected override void MapGeometryBuffer()
    {
        var mesh = _face.Triangulate();
        var faceBox = _face.GetBoundingBox();
        var center = (faceBox.Min + faceBox.Max) / 2;
        var normal = _face.ComputeNormal(center);
        var offset = RenderGeometryHelper.InterpolateOffsetByArea(_face.Area);
        var normalLength = RenderGeometryHelper.InterpolateAxisLengthByArea(_face.Area);

        RenderHelper.MapSurfaceBuffer(_surfaceBuffer, mesh, _extrusion);
        RenderHelper.MapMeshGridBuffer(_meshGridBuffer, mesh, _extrusion);
        RenderHelper.MapNormalVectorBuffer(_normalBuffer, _face.Evaluate(center) + normal * (offset + _extrusion),
            normal, normalLength);
    }

    protected override void UpdateEffects()
    {
        _surfaceBuffer.EffectInstance ??= new EffectInstance(_surfaceBuffer.FormatBits);
        _meshGridBuffer.EffectInstance ??= new EffectInstance(_meshGridBuffer.FormatBits);
        _normalBuffer.EffectInstance ??= new EffectInstance(_normalBuffer.FormatBits);

        _surfaceBuffer.EffectInstance.SetColor(_surfaceColor);
        _meshGridBuffer.EffectInstance.SetColor(_meshColor);
        _normalBuffer.EffectInstance.SetColor(_normalColor);
        _surfaceBuffer.EffectInstance.SetTransparency(_transparency);
    }

    protected override void RenderBuffers()
    {
        if (_drawSurface) FlushTriangleBuffer(_surfaceBuffer, _transparency);
        if (_drawMeshGrid) FlushLineBuffer(_meshGridBuffer);
        if (_drawNormalVector) FlushLineBuffer(_normalBuffer);
    }

    protected override void DisposeBuffers()
    {
        _surfaceBuffer.Dispose();
        _meshGridBuffer.Dispose();
        _normalBuffer.Dispose();
    }
}