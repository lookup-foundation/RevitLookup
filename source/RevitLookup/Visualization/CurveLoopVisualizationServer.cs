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
///     Represents a Revit direct-context 3D server that renders <see cref="CurveLoop" /> visualization geometry into the active view.
/// </summary>
public sealed class CurveLoopVisualizationServer : DirectContext3DServer
{
    private readonly RenderingBufferStorage _curveBuffer = new();
    private readonly List<RenderingBufferStorage> _normalsBuffers = new(1);

    private readonly RenderingBufferStorage _surfaceBuffer = new();
    private Color _curveColor = Color.InvalidColorValue;
    private double _diameter;
    private Color _directionColor = Color.InvalidColorValue;
    private bool _drawCurve;
    private bool _drawDirection;

    private bool _drawSurface;

    private Color _surfaceColor = Color.InvalidColorValue;

    private double _transparency;
    private IList<XYZ> _vertices = null!;

    /// <inheritdoc />
    public override string GetName()
    {
        return "CurveLoop visualization server";
    }

    /// <inheritdoc />
    public override string GetDescription()
    {
        return "CurveLoop geometry visualization";
    }

    /// <inheritdoc />
    public override bool UseInTransparentPass(View view)
    {
        return _drawSurface && _transparency > 0;
    }

    /// <inheritdoc />
    public override Outline? GetBoundingBox(View view)
    {
        if (_vertices.Count == 0)
        {
            return null;
        }

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

    /// <summary>
    ///     Registers the server for the specified curve loop and enables rendering.
    /// </summary>
    /// <param name="vertices">The tessellated vertices of the curve loop to visualize.</param>
    public void Register(IList<XYZ> vertices)
    {
        _vertices = vertices;
        Register();
    }

    /// <summary>
    ///     Updates the color of the curve loop surface and refreshes the open views.
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
    ///     Updates the color of the curve loop and refreshes the open views.
    /// </summary>
    /// <param name="value">The new curve color.</param>
    public void UpdateCurveColor(Color value)
    {
        UpdateViews(() =>
        {
            _curveColor = value;
            HasEffectsUpdates = true;
        });
    }

    /// <summary>
    ///     Updates the color of the direction indicators and refreshes the open views.
    /// </summary>
    /// <param name="value">The new direction indicator color.</param>
    public void UpdateDirectionColor(Color value)
    {
        UpdateViews(() =>
        {
            _directionColor = value;
            HasEffectsUpdates = true;
        });
    }

    /// <summary>
    ///     Updates the diameter of the curve loop tube and refreshes the open views.
    /// </summary>
    /// <param name="value">The new diameter.</param>
    public void UpdateDiameter(double value)
    {
        UpdateViews(() =>
        {
            _diameter = value;
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
    ///     Updates whether the curve loop surface is drawn and refreshes the open views.
    /// </summary>
    /// <param name="visible">A value indicating whether the surface is drawn.</param>
    public void UpdateSurfaceVisibility(bool visible)
    {
        UpdateViews(() => { _drawSurface = visible; });
    }

    /// <summary>
    ///     Updates whether the curve loop is drawn and refreshes the open views.
    /// </summary>
    /// <param name="visible">A value indicating whether the curve is drawn.</param>
    public void UpdateCurveVisibility(bool visible)
    {
        UpdateViews(() => { _drawCurve = visible; });
    }

    /// <summary>
    ///     Updates whether the direction indicators are drawn and refreshes the open views.
    /// </summary>
    /// <param name="visible">A value indicating whether the direction indicators are drawn.</param>
    public void UpdateDirectionVisibility(bool visible)
    {
        UpdateViews(() => { _drawDirection = visible; });
    }

    /// <inheritdoc />
    protected override bool AreBuffersValid()
    {
        return _surfaceBuffer.IsValid() && _curveBuffer.IsValid();
    }

    /// <inheritdoc />
    protected override void MapGeometryBuffer()
    {
        RenderHelper.MapCurveSurfaceBuffer(_surfaceBuffer, _vertices, _diameter);
        RenderHelper.MapCurveBuffer(_curveBuffer, _vertices, _diameter);
        MapDirectionsBuffer();
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    protected override void RenderBuffers()
    {
        if (_drawSurface)
        {
            FlushTriangleBuffer(_surfaceBuffer, _transparency);
        }

        if (_drawCurve)
        {
            FlushLineBuffer(_curveBuffer);
        }

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

    /// <inheritdoc />
    protected override void DisposeBuffers()
    {
        _surfaceBuffer.Dispose();
        _curveBuffer.Dispose();
        foreach (var buffer in _normalsBuffers)
        {
            buffer.Dispose();
        }
    }

    private RenderingBufferStorage GetOrCreateNormalBuffer(int index)
    {
        if (_normalsBuffers.Count > index)
        {
            return _normalsBuffers[index];
        }

        var buffer = new RenderingBufferStorage();
        _normalsBuffers.Add(buffer);
        return buffer;
    }
}
