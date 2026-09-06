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
///     Represents a Revit direct-context 3D server that renders <see cref="Solid" /> visualization geometry into the active view.
/// </summary>
public sealed class SolidVisualizationServer : DirectContext3DServer
{
    private readonly List<RenderingBufferStorage> _edgeBuffers = new(8);

    private readonly List<RenderingBufferStorage> _faceBuffers = new(4);
    private bool _drawEdge;

    private bool _drawFace;
    private Color _edgeColor = Color.InvalidColorValue;

    private Color _faceColor = Color.InvalidColorValue;
    private double _scale;
    private Solid _solid = null!;

    private double _transparency;

    /// <inheritdoc />
    public override string GetName()
    {
        return "Solid visualization server";
    }

    /// <inheritdoc />
    public override string GetDescription()
    {
        return "Solid geometry visualization";
    }

    /// <inheritdoc />
    public override bool UseInTransparentPass(View view)
    {
        return _drawFace && _transparency > 0;
    }

    /// <inheritdoc />
    public override Outline GetBoundingBox(View view)
    {
        var boundingBox = _solid.GetBoundingBox();
        var minPoint = boundingBox.Transform.OfPoint(boundingBox.Min);
        var maxPoint = boundingBox.Transform.OfPoint(boundingBox.Max);

        return new Outline(minPoint, maxPoint);
    }

    /// <summary>
    ///     Registers the server for the specified solid and enables rendering.
    /// </summary>
    /// <param name="solid">The solid to visualize.</param>
    public void Register(Solid solid)
    {
        _solid = solid;
        Register();
    }

    /// <summary>
    ///     Updates the color of the solid faces and refreshes the open views.
    /// </summary>
    /// <param name="value">The new face color.</param>
    public void UpdateFaceColor(Color value)
    {
        UpdateViews(() =>
        {
            _faceColor = value;
            HasEffectsUpdates = true;
        });
    }

    /// <summary>
    ///     Updates the color of the solid edges and refreshes the open views.
    /// </summary>
    /// <param name="value">The new edge color.</param>
    public void UpdateEdgeColor(Color value)
    {
        UpdateViews(() =>
        {
            _edgeColor = value;
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
    ///     Updates the scale factor of the visualization and refreshes the open views.
    /// </summary>
    /// <param name="value">The new scale factor.</param>
    /// <remarks>Discards the cached face and edge buffers, forcing a full geometry remap.</remarks>
    public void UpdateScale(double value)
    {
        UpdateViews(() =>
        {
            _scale = value;
            HasGeometryUpdates = true;
            HasEffectsUpdates = true;
            _faceBuffers.Clear();
            _edgeBuffers.Clear();
        });
    }

    /// <summary>
    ///     Updates whether the solid faces are drawn and refreshes the open views.
    /// </summary>
    /// <param name="value">A value indicating whether the faces are drawn.</param>
    public void UpdateFaceVisibility(bool value)
    {
        UpdateViews(() => { _drawFace = value; });
    }

    /// <summary>
    ///     Updates whether the solid edges are drawn and refreshes the open views.
    /// </summary>
    /// <param name="value">A value indicating whether the edges are drawn.</param>
    public void UpdateEdgeVisibility(bool value)
    {
        UpdateViews(() => { _drawEdge = value; });
    }

    /// <inheritdoc />
    protected override bool AreBuffersValid()
    {
        return _faceBuffers.TrueForAll(static buffer => buffer.IsValid())
               && _edgeBuffers.TrueForAll(static buffer => buffer.IsValid());
    }

    /// <inheritdoc />
    protected override void MapGeometryBuffer()
    {
        var scaledSolid = RenderGeometryHelper.ScaleSolid(_solid, _scale);

        var faceIndex = 0;
        foreach (var face in scaledSolid.Faces.EnumerateValues())
        {
            var buffer = GetOrCreateBuffer(_faceBuffers, faceIndex++);
            var triangulation = face.Triangulate();
            if (triangulation is null)
            {
                continue;
            }

            RenderHelper.MapSurfaceBuffer(buffer, triangulation, 0);
        }

        var edgeIndex = 0;
        foreach (var edge in scaledSolid.Edges.EnumerateValues())
        {
            var buffer = GetOrCreateBuffer(_edgeBuffers, edgeIndex++);
            var tessellation = edge.Tessellate();
            if (tessellation is null)
            {
                continue;
            }

            RenderHelper.MapCurveBuffer(buffer, tessellation);
        }
    }

    /// <inheritdoc />
    protected override void UpdateEffects()
    {
        foreach (var buffer in _faceBuffers)
        {
            buffer.EffectInstance ??= new EffectInstance(buffer.FormatBits);
            buffer.EffectInstance.SetColor(_faceColor);
            buffer.EffectInstance.SetTransparency(_transparency);
        }

        foreach (var buffer in _edgeBuffers)
        {
            buffer.EffectInstance ??= new EffectInstance(buffer.FormatBits);
            buffer.EffectInstance.SetColor(_edgeColor);
        }
    }

    /// <inheritdoc />
    protected override void RenderBuffers()
    {
        if (_drawFace)
        {
            foreach (var buffer in _faceBuffers)
            {
                FlushTriangleBuffer(buffer, _transparency);
            }
        }

        if (_drawEdge)
        {
            foreach (var buffer in _edgeBuffers)
            {
                FlushLineBuffer(buffer);
            }
        }
    }

    /// <inheritdoc />
    protected override void DisposeBuffers()
    {
        foreach (var buffer in _faceBuffers)
        {
            buffer.Dispose();
        }

        foreach (var buffer in _edgeBuffers)
        {
            buffer.Dispose();
        }
    }

    private static RenderingBufferStorage GetOrCreateBuffer(List<RenderingBufferStorage> buffers, int index)
    {
        if (buffers.Count > index)
        {
            return buffers[index];
        }

        var buffer = new RenderingBufferStorage();
        buffers.Add(buffer);
        return buffer;
    }
}
