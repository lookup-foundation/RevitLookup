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

public sealed class SolidVisualizationServer : DirectContext3DServer
{
    private Solid _solid = null!;

    private double _transparency;
    private double _scale;

    private Color _faceColor = Color.InvalidColorValue;
    private Color _edgeColor = Color.InvalidColorValue;

    private bool _drawFace;
    private bool _drawEdge;

    private readonly List<RenderingBufferStorage> _faceBuffers = new(4);
    private readonly List<RenderingBufferStorage> _edgeBuffers = new(8);

    public override string GetName() => "Solid visualization server";
    public override string GetDescription() => "Solid geometry visualization";
    public override bool UseInTransparentPass(View view) => _drawFace && _transparency > 0;

    public override Outline GetBoundingBox(View view)
    {
        var boundingBox = _solid.GetBoundingBox();
        var minPoint = boundingBox.Transform.OfPoint(boundingBox.Min);
        var maxPoint = boundingBox.Transform.OfPoint(boundingBox.Max);

        return new Outline(minPoint, maxPoint);
    }

    public void Register(Solid solid)
    {
        _solid = solid;
        Register();
    }

    public void UpdateFaceColor(Color value) => UpdateViews(() =>
    {
        _faceColor = value;
        HasEffectsUpdates = true;
    });

    public void UpdateEdgeColor(Color value) => UpdateViews(() =>
    {
        _edgeColor = value;
        HasEffectsUpdates = true;
    });

    public void UpdateTransparency(double value) => UpdateViews(() =>
    {
        _transparency = value;
        HasEffectsUpdates = true;
    });

    public void UpdateScale(double value) => UpdateViews(() =>
    {
        _scale = value;
        HasGeometryUpdates = true;
        HasEffectsUpdates = true;
        _faceBuffers.Clear();
        _edgeBuffers.Clear();
    });

    public void UpdateFaceVisibility(bool value) => UpdateViews(() => { _drawFace = value; });

    public void UpdateEdgeVisibility(bool value) => UpdateViews(() => { _drawEdge = value; });

    protected override bool AreBuffersValid()
    {
        return _faceBuffers.TrueForAll(buffer => buffer.IsValid())
               && _edgeBuffers.TrueForAll(buffer => buffer.IsValid());
    }

    protected override void MapGeometryBuffer()
    {
        var scaledSolid = RenderGeometryHelper.ScaleSolid(_solid, _scale);

        var faceIndex = 0;
        foreach (Face face in scaledSolid.Faces)
        {
            var buffer = GetOrCreateBuffer(_faceBuffers, faceIndex++);
            var triangulation = face.Triangulate();
            if (triangulation is null) continue;
            
            RenderHelper.MapSurfaceBuffer(buffer, triangulation, 0);
        }

        var edgeIndex = 0;
        foreach (Edge edge in scaledSolid.Edges)
        {
            var buffer = GetOrCreateBuffer(_edgeBuffers, edgeIndex++);
            var tessellation = edge.Tessellate();
            if (tessellation is null) continue;
            
            RenderHelper.MapCurveBuffer(buffer, tessellation);
        }
    }

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

    protected override void DisposeBuffers()
    {
        foreach (var buffer in _faceBuffers) buffer.Dispose();
        foreach (var buffer in _edgeBuffers) buffer.Dispose();
    }

    private static RenderingBufferStorage GetOrCreateBuffer(List<RenderingBufferStorage> buffers, int index)
    {
        if (buffers.Count > index) return buffers[index];

        var buffer = new RenderingBufferStorage();
        buffers.Add(buffer);
        return buffer;
    }
}