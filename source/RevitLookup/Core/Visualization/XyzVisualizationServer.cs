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

public sealed class XyzVisualizationServer : DirectContext3DServer
{
    private XYZ _point = null!;

    private double _transparency;
    private double _axisLength;

    private Color _xColor = Color.InvalidColorValue;
    private Color _yColor = Color.InvalidColorValue;
    private Color _zColor = Color.InvalidColorValue;

    private bool _drawPlane;
    private bool _drawXAxis;
    private bool _drawYAxis;
    private bool _drawZAxis;

    private readonly RenderingBufferStorage[] _planeBuffers = Enumerable.Range(0, 3)
        .Select(_ => new RenderingBufferStorage())
        .ToArray();

    private readonly RenderingBufferStorage[] _axisBuffers = Enumerable.Range(0, 3)
        .Select(_ => new RenderingBufferStorage())
        .ToArray();

    private readonly XYZ[] _normals =
    [
        XYZ.BasisX,
        XYZ.BasisY,
        XYZ.BasisZ
    ];

    public override string GetName() => "XYZ visualization server";
    public override string GetDescription() => "XYZ geometry visualization";
    public override bool UseInTransparentPass(View view) => _drawPlane && _transparency > 0;

    public override Outline GetBoundingBox(View view)
    {
        var minPoint = new XYZ(_point.X - _axisLength, _point.Y - _axisLength, _point.Z - _axisLength);
        var maxPoint = new XYZ(_point.X + _axisLength, _point.Y + _axisLength, _point.Z + _axisLength);

        return new Outline(minPoint, maxPoint);
    }

    public void Register(XYZ point)
    {
        _point = point;
        Register();
    }

    public void UpdateXColor(Color value) => UpdateViews(() =>
    {
        _xColor = value;
        HasEffectsUpdates = true;
    });

    public void UpdateYColor(Color value) => UpdateViews(() =>
    {
        _yColor = value;
        HasEffectsUpdates = true;
    });

    public void UpdateZColor(Color value) => UpdateViews(() =>
    {
        _zColor = value;
        HasEffectsUpdates = true;
    });

    public void UpdateAxisLength(double value) => UpdateViews(() =>
    {
        _axisLength = value;
        HasGeometryUpdates = true;
    });

    public void UpdateTransparency(double value) => UpdateViews(() =>
    {
        _transparency = value;
        HasEffectsUpdates = true;
    });

    public void UpdatePlaneVisibility(bool visible) => UpdateViews(() => { _drawPlane = visible; });

    public void UpdateXAxisVisibility(bool visible) => UpdateViews(() => { _drawXAxis = visible; });

    public void UpdateYAxisVisibility(bool visible) => UpdateViews(() => { _drawYAxis = visible; });

    public void UpdateZAxisVisibility(bool visible) => UpdateViews(() => { _drawZAxis = visible; });

    protected override bool AreBuffersValid()
    {
        return Array.TrueForAll(_planeBuffers, buffer => buffer.IsValid())
               && Array.TrueForAll(_axisBuffers, buffer => buffer.IsValid());
    }

    protected override void MapGeometryBuffer()
    {
        var normalExtendLength = _axisLength > 1 ? 0.8 : _axisLength * 0.8;

        for (var i = 0; i < _normals.Length; i++)
        {
            var normal = _normals[i];
            RenderHelper.MapNormalVectorBuffer(_axisBuffers[i], _point - normal * (_axisLength + normalExtendLength),
                normal, 2 * (_axisLength + normalExtendLength));
            RenderHelper.MapSideBuffer(_planeBuffers[i], _point - normal * _axisLength, _point + normal * _axisLength);
        }
    }

    protected override void UpdateEffects()
    {
        foreach (var buffer in _planeBuffers)
        {
            buffer.EffectInstance ??= new EffectInstance(buffer.FormatBits);
            buffer.EffectInstance.SetTransparency(_transparency);
        }

        _planeBuffers[0].EffectInstance!.SetColor(_xColor);
        _planeBuffers[1].EffectInstance!.SetColor(_yColor);
        _planeBuffers[2].EffectInstance!.SetColor(_zColor);

        foreach (var buffer in _axisBuffers)
        {
            buffer.EffectInstance ??= new EffectInstance(buffer.FormatBits);
        }

        _axisBuffers[0].EffectInstance!.SetColor(_xColor);
        _axisBuffers[1].EffectInstance!.SetColor(_yColor);
        _axisBuffers[2].EffectInstance!.SetColor(_zColor);
    }

    protected override void RenderBuffers()
    {
        if (_drawXAxis)
        {
            FlushLineBuffer(_axisBuffers[0]);
            if (_drawPlane) FlushTriangleBuffer(_planeBuffers[0], _transparency);
        }

        if (_drawYAxis)
        {
            FlushLineBuffer(_axisBuffers[1]);
            if (_drawPlane) FlushTriangleBuffer(_planeBuffers[1], _transparency);
        }

        if (_drawZAxis)
        {
            FlushLineBuffer(_axisBuffers[2]);
            if (_drawPlane) FlushTriangleBuffer(_planeBuffers[2], _transparency);
        }
    }
}