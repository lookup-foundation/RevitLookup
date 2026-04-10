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

public sealed class BoundingBoxVisualizationServer : DirectContext3DServer
{
    private BoundingBoxXYZ _box = null!;

    private double _transparency;

    private Color _surfaceColor = Color.InvalidColorValue;
    private Color _edgeColor = Color.InvalidColorValue;
    private Color _axisColor = Color.InvalidColorValue;

    private bool _drawSurface;
    private bool _drawEdge;
    private bool _drawAxis;

    private readonly RenderingBufferStorage _surfaceBuffer = new();
    private readonly RenderingBufferStorage _edgeBuffer = new();

    private readonly RenderingBufferStorage[] _axisBuffers = Enumerable.Range(0, 6)
        .Select(_ => new RenderingBufferStorage())
        .ToArray();

    private readonly XYZ[] _normals =
    [
        XYZ.BasisX,
        XYZ.BasisY,
        XYZ.BasisZ
    ];

    public override string GetName() => "BoundingBoxXYZ visualization server";
    public override string GetDescription() => "BoundingBoxXYZ geometry visualization";
    public override bool UseInTransparentPass(View view) => _drawSurface && _transparency > 0;

    public override Outline GetBoundingBox(View view)
    {
        return new Outline(_box.Min, _box.Max);
    }

    public void Register(BoundingBoxXYZ box)
    {
        _box = box;
        Register();
    }

    public void UpdateSurfaceColor(Color color) => UpdateViews(() =>
    {
        _surfaceColor = color;
        HasEffectsUpdates = true;
    });

    public void UpdateEdgeColor(Color color) => UpdateViews(() =>
    {
        _edgeColor = color;
        HasEffectsUpdates = true;
    });

    public void UpdateAxisColor(Color color) => UpdateViews(() =>
    {
        _axisColor = color;
        HasEffectsUpdates = true;
    });

    public void UpdateTransparency(double value) => UpdateViews(() =>
    {
        _transparency = value;
        HasEffectsUpdates = true;
    });

    public void UpdateSurfaceVisibility(bool visible) => UpdateViews(() => { _drawSurface = visible; });

    public void UpdateEdgeVisibility(bool visible) => UpdateViews(() => { _drawEdge = visible; });

    public void UpdateAxisVisibility(bool visible) => UpdateViews(() => { _drawAxis = visible; });

    protected override bool AreBuffersValid()
    {
        return _surfaceBuffer.IsValid() && _edgeBuffer.IsValid();
    }

    protected override void MapGeometryBuffer()
    {
        RenderHelper.MapBoundingBoxSurfaceBuffer(_surfaceBuffer, _box);
        RenderHelper.MapBoundingBoxEdgeBuffer(_edgeBuffer, _box);
        MapAxisBuffers();
    }

    protected override void UpdateEffects()
    {
        _surfaceBuffer.EffectInstance ??= new EffectInstance(_surfaceBuffer.FormatBits);
        _surfaceBuffer.EffectInstance.SetColor(_surfaceColor);
        _surfaceBuffer.EffectInstance.SetTransparency(_transparency);

        _edgeBuffer.EffectInstance ??= new EffectInstance(_edgeBuffer.FormatBits);
        _edgeBuffer.EffectInstance.SetColor(_edgeColor);

        foreach (var buffer in _axisBuffers)
        {
            buffer.EffectInstance ??= new EffectInstance(buffer.FormatBits);
            buffer.EffectInstance.SetColor(_axisColor);
        }
    }

    protected override void RenderBuffers()
    {
        if (_drawSurface) FlushTriangleBuffer(_surfaceBuffer, _transparency);
        if (_drawEdge) FlushLineBuffer(_edgeBuffer);

        if (_drawAxis)
        {
            foreach (var buffer in _axisBuffers)
            {
                FlushLineBuffer(buffer);
            }
        }
    }

    private void MapAxisBuffers()
    {
        var unitVector = new XYZ(1, 1, 1);
        var minPoint = _box.Transform.OfPoint(_box.Min);
        var maxPoint = _box.Transform.OfPoint(_box.Max);
        var axisLength = RenderGeometryHelper.InterpolateAxisLengthByPoints(minPoint, maxPoint);

        for (var i = 0; i < _normals.Length; i++)
        {
            var normal = _normals[i];
            var minBuffer = _axisBuffers[i];
            var maxBuffer = _axisBuffers[i + _normals.Length];

            RenderHelper.MapNormalVectorBuffer(minBuffer,
                minPoint - unitVector * RevitApiContext.Application.ShortCurveTolerance, normal, axisLength);
            RenderHelper.MapNormalVectorBuffer(maxBuffer,
                maxPoint + unitVector * RevitApiContext.Application.ShortCurveTolerance, -normal, axisLength);
        }
    }
}