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

public sealed class CurveLoopVisualizationServer : DirectContext3DServer
{
    private IList<XYZ> _vertices = null!;

    private double _transparency;
    private double _diameter;

    private Color _surfaceColor = Color.InvalidColorValue;
    private Color _curveColor = Color.InvalidColorValue;
    private Color _directionColor = Color.InvalidColorValue;

    private bool _drawSurface;
    private bool _drawCurve;
    private bool _drawDirection;

    private readonly RenderingBufferStorage _surfaceBuffer = new();
    private readonly RenderingBufferStorage _curveBuffer = new();
    private readonly List<RenderingBufferStorage> _normalsBuffers = new(1);

    public override string GetName() => "CurveLoop visualization server";
    public override string GetDescription() => "CurveLoop geometry visualization";
    public override bool UseInTransparentPass(View view) => _drawSurface && _transparency > 0;

    public override Outline? GetBoundingBox(View view)
    {
        if (_vertices.Count == 0) return null;

        var min = _vertices[0];
        var max = _vertices[0];

        for (var i = 1; i < _vertices.Count; i++)
        {
            var vertex = _vertices[i];
            min = new XYZ(Math.Min(min.X, vertex.X), Math.Min(min.Y, vertex.Y), Math.Min(min.Z, vertex.Z));
            max = new XYZ(Math.Max(max.X, vertex.X), Math.Max(max.Y, vertex.Y), Math.Max(max.Z, vertex.Z));
        }

        return new Outline(min, max);
    }

    public void Register(IList<XYZ> vertices)
    {
        _vertices = vertices;
        Register();
    }

    public void UpdateSurfaceColor(Color value) => UpdateViews(() =>
    {
        _surfaceColor = value;
        HasEffectsUpdates = true;
    });

    public void UpdateCurveColor(Color value) => UpdateViews(() =>
    {
        _curveColor = value;
        HasEffectsUpdates = true;
    });

    public void UpdateDirectionColor(Color value) => UpdateViews(() =>
    {
        _directionColor = value;
        HasEffectsUpdates = true;
    });

    public void UpdateDiameter(double value) => UpdateViews(() =>
    {
        _diameter = value;
        HasGeometryUpdates = true;
    });

    public void UpdateTransparency(double value) => UpdateViews(() =>
    {
        _transparency = value;
        HasEffectsUpdates = true;
    });

    public void UpdateSurfaceVisibility(bool visible) => UpdateViews(() => { _drawSurface = visible; });

    public void UpdateCurveVisibility(bool visible) => UpdateViews(() => { _drawCurve = visible; });

    public void UpdateDirectionVisibility(bool visible) => UpdateViews(() => { _drawDirection = visible; });

    protected override bool AreBuffersValid()
    {
        return _surfaceBuffer.IsValid() && _curveBuffer.IsValid();
    }

    protected override void MapGeometryBuffer()
    {
        RenderHelper.MapCurveSurfaceBuffer(_surfaceBuffer, _vertices, _diameter);
        RenderHelper.MapCurveBuffer(_curveBuffer, _vertices, _diameter);
        MapDirectionsBuffer();
    }

    protected override void UpdateEffects()
    {
        _surfaceBuffer.EffectInstance ??= new EffectInstance(_surfaceBuffer.FormatBits);
        _surfaceBuffer.EffectInstance.SetColor(_surfaceColor);
        _surfaceBuffer.EffectInstance.SetTransparency(_transparency);

        _curveBuffer.EffectInstance ??= new EffectInstance(_curveBuffer.FormatBits);
        _curveBuffer.EffectInstance.SetColor(_curveColor);

        foreach (var buffer in _normalsBuffers)
        {
            buffer.EffectInstance ??= new EffectInstance(buffer.FormatBits);
            buffer.EffectInstance.SetColor(_directionColor);
        }
    }

    protected override void RenderBuffers()
    {
        if (_drawSurface) FlushTriangleBuffer(_surfaceBuffer, _transparency);
        if (_drawCurve) FlushLineBuffer(_curveBuffer);

        if (_drawDirection)
        {
            foreach (var buffer in _normalsBuffers)
            {
                FlushLineBuffer(buffer);
            }
        }
    }

    private void MapDirectionsBuffer()
    {
        var verticalOffset = 0d;

        for (var i = 0; i < _vertices.Count - 1; i++)
        {
            var startPoint = _vertices[i];
            var endPoint = _vertices[i + 1];
            var centerPoint = (startPoint + endPoint) / 2;
            var buffer = GetOrCreateNormalBuffer(i);

            var segmentVector = endPoint - startPoint;
            var segmentLength = segmentVector.GetLength();
            var segmentDirection = segmentVector.Normalize();
            if (verticalOffset == 0)
            {
                verticalOffset = RenderGeometryHelper.InterpolateOffsetByDiameter(_diameter) + _diameter / 2d;
            }

            var offsetVector = XYZ.BasisX.CrossProduct(segmentDirection).Normalize() * verticalOffset;
            if (offsetVector.IsZeroLength())
            {
                offsetVector = XYZ.BasisY.CrossProduct(segmentDirection).Normalize() * verticalOffset;
            }

            if (offsetVector.Z < 0)
            {
                offsetVector = -offsetVector;
            }

            var arrowLength = segmentLength > 1 ? 1d : segmentLength * 0.6;
            var arrowOrigin = centerPoint + offsetVector - segmentDirection * (arrowLength / 2);

            RenderHelper.MapNormalVectorBuffer(buffer, arrowOrigin, segmentDirection, arrowLength);
        }
    }

    private RenderingBufferStorage GetOrCreateNormalBuffer(int index)
    {
        if (_normalsBuffers.Count > index) return _normalsBuffers[index];

        var buffer = new RenderingBufferStorage();
        _normalsBuffers.Add(buffer);
        return buffer;
    }
}