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
///     Represents a Revit direct-context 3D server that renders <see cref="Face" /> visualization geometry into the active view.
/// </summary>
public sealed class FaceVisualizationServer : DirectContext3DServer
{
    private readonly RenderingBufferStorage _meshGridBuffer = new();
    private readonly RenderingBufferStorage _normalBuffer = new();

    private readonly RenderingBufferStorage _surfaceBuffer = new();
    private bool _drawMeshGrid;
    private bool _drawNormalVector;

    private bool _drawSurface;

    private double _extrusion;
    private Face _face = null!;
    private Color _meshColor = Color.InvalidColorValue;
    private Color _normalColor = Color.InvalidColorValue;

    private Color _surfaceColor = Color.InvalidColorValue;
    private double _transparency;

    /// <inheritdoc />
    public override string GetName()
    {
        return "Face visualization server";
    }

    /// <inheritdoc />
    public override string GetDescription()
    {
        return "Face geometry visualization";
    }

    /// <inheritdoc />
    public override bool UseInTransparentPass(View view)
    {
        return _drawSurface && _transparency > 0;
    }

    /// <inheritdoc />
    public override Outline? GetBoundingBox(View view)
    {
        if (_face.Reference is null)
        {
            return null;
        }

        var element = _face.Reference.ElementId.ToElement(view.Document)!;
        var boundingBox = element.get_BoundingBox(null) ?? element.get_BoundingBox(view);
        if (boundingBox is null)
        {
            return null;
        }

        var minPoint = boundingBox.Transform.OfPoint(boundingBox.Min);
        var maxPoint = boundingBox.Transform.OfPoint(boundingBox.Max);

        return new Outline(minPoint, maxPoint);
    }

    /// <summary>
    ///     Registers the server for the specified face and enables rendering.
    /// </summary>
    /// <param name="face">The face to visualize.</param>
    public void Register(Face face)
    {
        _face = face;
        Register();
    }

    /// <summary>
    ///     Updates the color of the face surface and refreshes the open views.
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
    ///     Updates the color of the face mesh grid and refreshes the open views.
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
    ///     Updates the color of the face normal vector and refreshes the open views.
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
    ///     Updates the extrusion value of the face and refreshes the open views.
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
    ///     Updates whether the face surface is drawn and refreshes the open views.
    /// </summary>
    /// <param name="visible">A value indicating whether the surface is drawn.</param>
    public void UpdateSurfaceVisibility(bool visible)
    {
        UpdateViews(() => { _drawSurface = visible; });
    }

    /// <summary>
    ///     Updates whether the face mesh grid is drawn and refreshes the open views.
    /// </summary>
    /// <param name="visible">A value indicating whether the mesh grid is drawn.</param>
    public void UpdateMeshGridVisibility(bool visible)
    {
        UpdateViews(() => { _drawMeshGrid = visible; });
    }

    /// <summary>
    ///     Updates whether the face normal vector is drawn and refreshes the open views.
    /// </summary>
    /// <param name="visible">A value indicating whether the normal vector is drawn.</param>
    public void UpdateNormalVectorVisibility(bool visible)
    {
        UpdateViews(() => { _drawNormalVector = visible; });
    }

    /// <inheritdoc />
    protected override bool AreBuffersValid()
    {
        return _surfaceBuffer.IsValid() && _meshGridBuffer.IsValid() && _normalBuffer.IsValid();
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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
            FlushLineBuffer(_normalBuffer);
        }
    }

    /// <inheritdoc />
    protected override void DisposeBuffers()
    {
        _surfaceBuffer.Dispose();
        _meshGridBuffer.Dispose();
        _normalBuffer.Dispose();
    }
}
