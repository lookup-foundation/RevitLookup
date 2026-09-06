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
///     Represents a Revit direct-context 3D server that renders <see cref="XYZ" /> coordinate visualization geometry into the active view.
/// </summary>
public sealed class XyzVisualizationServer : DirectContext3DServer
{
    private readonly RenderingBufferStorage[] _axisBuffers = Enumerable.Range(0, 3)
        .Select(static _ => new RenderingBufferStorage())
        .ToArray();

    private readonly XYZ[] _normals =
    [
        XYZ.BasisX,
        XYZ.BasisY,
        XYZ.BasisZ
    ];

    private readonly RenderingBufferStorage[] _planeBuffers = Enumerable.Range(0, 3)
        .Select(static _ => new RenderingBufferStorage())
        .ToArray();

    private double _axisLength;

    private bool _drawPlane;
    private bool _drawXAxis;
    private bool _drawYAxis;
    private bool _drawZAxis;
    private XYZ _point = null!;

    private double _transparency;

    private Color _xColor = Color.InvalidColorValue;
    private Color _yColor = Color.InvalidColorValue;
    private Color _zColor = Color.InvalidColorValue;

    /// <inheritdoc />
    public override string GetName()
    {
        return "XYZ visualization server";
    }

    /// <inheritdoc />
    public override string GetDescription()
    {
        return "XYZ geometry visualization";
    }

    /// <inheritdoc />
    public override bool UseInTransparentPass(View view)
    {
        return _drawPlane && _transparency > 0;
    }

    /// <inheritdoc />
    public override Outline GetBoundingBox(View view)
    {
        var minPoint = new XYZ(_point.X - _axisLength, _point.Y - _axisLength, _point.Z - _axisLength);
        var maxPoint = new XYZ(_point.X + _axisLength, _point.Y + _axisLength, _point.Z + _axisLength);

        return new Outline(minPoint, maxPoint);
    }

    /// <summary>
    ///     Registers the server for the specified point and enables rendering.
    /// </summary>
    /// <param name="point">The point the coordinate axes are drawn from.</param>
    public void Register(XYZ point)
    {
        _point = point;
        Register();
    }

    /// <summary>
    ///     Updates the color of the X axis and refreshes the open views.
    /// </summary>
    /// <param name="value">The new X axis color.</param>
    public void UpdateXColor(Color value)
    {
        UpdateViews(() =>
        {
            _xColor = value;
            HasEffectsUpdates = true;
        });
    }

    /// <summary>
    ///     Updates the color of the Y axis and refreshes the open views.
    /// </summary>
    /// <param name="value">The new Y axis color.</param>
    public void UpdateYColor(Color value)
    {
        UpdateViews(() =>
        {
            _yColor = value;
            HasEffectsUpdates = true;
        });
    }

    /// <summary>
    ///     Updates the color of the Z axis and refreshes the open views.
    /// </summary>
    /// <param name="value">The new Z axis color.</param>
    public void UpdateZColor(Color value)
    {
        UpdateViews(() =>
        {
            _zColor = value;
            HasEffectsUpdates = true;
        });
    }

    /// <summary>
    ///     Updates the length of the coordinate axes and refreshes the open views.
    /// </summary>
    /// <param name="value">The new axis length.</param>
    public void UpdateAxisLength(double value)
    {
        UpdateViews(() =>
        {
            _axisLength = value;
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
    ///     Updates whether the coordinate plane is drawn and refreshes the open views.
    /// </summary>
    /// <param name="visible">A value indicating whether the plane is drawn.</param>
    public void UpdatePlaneVisibility(bool visible)
    {
        UpdateViews(() => { _drawPlane = visible; });
    }

    /// <summary>
    ///     Updates whether the X axis is drawn and refreshes the open views.
    /// </summary>
    /// <param name="visible">A value indicating whether the X axis is drawn.</param>
    public void UpdateXAxisVisibility(bool visible)
    {
        UpdateViews(() => { _drawXAxis = visible; });
    }

    /// <summary>
    ///     Updates whether the Y axis is drawn and refreshes the open views.
    /// </summary>
    /// <param name="visible">A value indicating whether the Y axis is drawn.</param>
    public void UpdateYAxisVisibility(bool visible)
    {
        UpdateViews(() => { _drawYAxis = visible; });
    }

    /// <summary>
    ///     Updates whether the Z axis is drawn and refreshes the open views.
    /// </summary>
    /// <param name="visible">A value indicating whether the Z axis is drawn.</param>
    public void UpdateZAxisVisibility(bool visible)
    {
        UpdateViews(() => { _drawZAxis = visible; });
    }

    /// <inheritdoc />
    protected override bool AreBuffersValid()
    {
        return Array.TrueForAll(_planeBuffers, static buffer => buffer.IsValid())
               && Array.TrueForAll(_axisBuffers, static buffer => buffer.IsValid());
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    protected override void RenderBuffers()
    {
        if (_drawXAxis)
        {
            FlushLineBuffer(_axisBuffers[0]);
            if (_drawPlane)
            {
                FlushTriangleBuffer(_planeBuffers[0], _transparency);
            }
        }

        if (_drawYAxis)
        {
            FlushLineBuffer(_axisBuffers[1]);
            if (_drawPlane)
            {
                FlushTriangleBuffer(_planeBuffers[1], _transparency);
            }
        }

        if (_drawZAxis)
        {
            FlushLineBuffer(_axisBuffers[2]);
            if (_drawPlane)
            {
                FlushTriangleBuffer(_planeBuffers[2], _transparency);
            }
        }
    }

    /// <inheritdoc />
    protected override void DisposeBuffers()
    {
        foreach (var buffer in _planeBuffers)
        {
            buffer.Dispose();
        }

        foreach (var buffer in _axisBuffers)
        {
            buffer.Dispose();
        }
    }
}
