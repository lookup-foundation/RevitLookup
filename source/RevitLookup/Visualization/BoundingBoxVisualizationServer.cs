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
///     Represents a Revit direct-context 3D server that renders <see cref="BoundingBoxXYZ" /> visualization geometry into the active view.
/// </summary>
public sealed class BoundingBoxVisualizationServer : DirectContext3DServer
{
    private readonly RenderingBufferStorage[] _axisBuffers = Enumerable.Range(0, 6)
        .Select(static _ => new RenderingBufferStorage())
        .ToArray();

    private readonly RenderingBufferStorage _edgeBuffer = new();

    private readonly XYZ[] _normals =
    [
        XYZ.BasisX,
        XYZ.BasisY,
        XYZ.BasisZ
    ];

    private readonly RenderingBufferStorage _surfaceBuffer = new();
    private Color _axisColor = Color.InvalidColorValue;
    private BoundingBoxXYZ _box = null!;
    private bool _drawAxis;
    private bool _drawEdge;

    private bool _drawSurface;
    private Color _edgeColor = Color.InvalidColorValue;

    private Color _surfaceColor = Color.InvalidColorValue;

    private double _transparency;

    /// <inheritdoc />
    public override string GetName()
    {
        return "BoundingBoxXYZ visualization server";
    }

    /// <inheritdoc />
    public override string GetDescription()
    {
        return "BoundingBoxXYZ geometry visualization";
    }

    /// <inheritdoc />
    public override bool UseInTransparentPass(View view)
    {
        return _drawSurface && _transparency > 0;
    }

    /// <inheritdoc />
    public override Outline GetBoundingBox(View view)
    {
        return new Outline(_box.Min, _box.Max);
    }

    /// <summary>
    ///     Registers the server for the specified bounding box and enables rendering.
    /// </summary>
    /// <param name="box">The bounding box to visualize.</param>
    public void Register(BoundingBoxXYZ box)
    {
        _box = box;
        Register();
    }

    /// <summary>
    ///     Updates the color of the bounding box surface and refreshes the open views.
    /// </summary>
    /// <param name="color">The new surface color.</param>
    public void UpdateSurfaceColor(Color color)
    {
        UpdateViews(() =>
        {
            _surfaceColor = color;
            HasEffectsUpdates = true;
        });
    }

    /// <summary>
    ///     Updates the color of the bounding box edges and refreshes the open views.
    /// </summary>
    /// <param name="color">The new edge color.</param>
    public void UpdateEdgeColor(Color color)
    {
        UpdateViews(() =>
        {
            _edgeColor = color;
            HasEffectsUpdates = true;
        });
    }

    /// <summary>
    ///     Updates the color of the bounding box axes and refreshes the open views.
    /// </summary>
    /// <param name="color">The new axis color.</param>
    public void UpdateAxisColor(Color color)
    {
        UpdateViews(() =>
        {
            _axisColor = color;
            HasEffectsUpdates = true;
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
    ///     Updates whether the bounding box surface is drawn and refreshes the open views.
    /// </summary>
    /// <param name="visible">A value indicating whether the surface is drawn.</param>
    public void UpdateSurfaceVisibility(bool visible)
    {
        UpdateViews(() => { _drawSurface = visible; });
    }

    /// <summary>
    ///     Updates whether the bounding box edges are drawn and refreshes the open views.
    /// </summary>
    /// <param name="visible">A value indicating whether the edges are drawn.</param>
    public void UpdateEdgeVisibility(bool visible)
    {
        UpdateViews(() => { _drawEdge = visible; });
    }

    /// <summary>
    ///     Updates whether the bounding box axes are drawn and refreshes the open views.
    /// </summary>
    /// <param name="visible">A value indicating whether the axes are drawn.</param>
    public void UpdateAxisVisibility(bool visible)
    {
        UpdateViews(() => { _drawAxis = visible; });
    }

    /// <inheritdoc />
    protected override bool AreBuffersValid()
    {
        return _surfaceBuffer.IsValid() && _edgeBuffer.IsValid();
    }

    /// <inheritdoc />
    protected override void MapGeometryBuffer()
    {
        RenderHelper.MapBoundingBoxSurfaceBuffer(_surfaceBuffer, _box);
        RenderHelper.MapBoundingBoxEdgeBuffer(_edgeBuffer, _box);
        MapAxisBuffers();
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    protected override void RenderBuffers()
    {
        if (_drawSurface)
        {
            FlushTriangleBuffer(_surfaceBuffer, _transparency);
        }

        if (_drawEdge)
        {
            FlushLineBuffer(_edgeBuffer);
        }

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

    /// <inheritdoc />
    protected override void DisposeBuffers()
    {
        _surfaceBuffer.Dispose();
        _edgeBuffer.Dispose();
        foreach (var buffer in _axisBuffers)
        {
            buffer.Dispose();
        }
    }
}
