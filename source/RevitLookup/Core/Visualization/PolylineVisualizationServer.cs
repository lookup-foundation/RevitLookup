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
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;
using RevitLookup.Core.Visualization.Buffers;
using RevitLookup.Core.Visualization.Events;
using RevitLookup.Core.Visualization.Helpers;

namespace RevitLookup.Core.Visualization;

public sealed partial class PolylineVisualizationServer : IDirectContext3DServer
{
    private IList<XYZ> _vertices = null!; //Cant be null after registration
    private bool _hasEffectsUpdates = true;
    private bool _hasGeometryUpdates = true;

    private readonly Guid _guid = Guid.NewGuid();
    private readonly Lock _renderLock = new();

    private readonly RenderingBufferStorage _surfaceBuffer = new();
    private readonly RenderingBufferStorage _curveBuffer = new();
    private readonly List<RenderingBufferStorage> _normalsBuffers = new(1);

    private double _transparency;
    private double _diameter;

    private Color _surfaceColor = Color.InvalidColorValue;
    private Color _curveColor = Color.InvalidColorValue;
    private Color _directionColor = Color.InvalidColorValue;

    private bool _drawCurve;
    private bool _drawDirection;
    private bool _drawSurface;

    public Guid GetServerId() => _guid;
    public string GetVendorId() => "RevitLookup";
    public string GetName() => "Polyline visualization server";
    public string GetDescription() => "Polyline geometry visualization";
    public ExternalServiceId GetServiceId() => ExternalServices.BuiltInExternalServices.DirectContext3DService;
    public string GetApplicationId() => string.Empty;
    public string GetSourceId() => string.Empty;
    public bool UsesHandles() => false;
    public bool CanExecute(View view) => true;
    public bool UseInTransparentPass(View view) => _drawSurface && _transparency > 0;
    public Outline? GetBoundingBox(View view) => null;

    public void RenderScene(View view, DisplayStyle displayStyle)
    {
        lock (_renderLock)
        {
            try
            {
                if (_hasGeometryUpdates || !_surfaceBuffer.IsValid() || !_curveBuffer.IsValid())
                {
                    MapGeometryBuffer();
                    _hasGeometryUpdates = false;
                }

                if (_hasEffectsUpdates)
                {
                    UpdateEffects();
                    _hasEffectsUpdates = false;
                }

                if (_drawSurface)
                {
                    var isTransparentPass = DrawContext.IsTransparentPass();
                    if (isTransparentPass && _transparency > 0 || !isTransparentPass && _transparency == 0)
                    {
                        DrawContext.FlushBuffer(_surfaceBuffer.VertexBuffer,
                            _surfaceBuffer.VertexBufferCount,
                            _surfaceBuffer.IndexBuffer,
                            _surfaceBuffer.IndexBufferCount,
                            _surfaceBuffer.VertexFormat,
                            _surfaceBuffer.EffectInstance, PrimitiveType.TriangleList, 0,
                            _surfaceBuffer.PrimitiveCount);
                    }
                }

                if (_drawCurve)
                {
                    DrawContext.FlushBuffer(_curveBuffer.VertexBuffer,
                        _curveBuffer.VertexBufferCount,
                        _curveBuffer.IndexBuffer,
                        _curveBuffer.IndexBufferCount,
                        _curveBuffer.VertexFormat,
                        _curveBuffer.EffectInstance, PrimitiveType.LineList, 0,
                        _curveBuffer.PrimitiveCount);
                }

                if (_drawDirection)
                {
                    foreach (var buffer in _normalsBuffers)
                    {
                        DrawContext.FlushBuffer(buffer.VertexBuffer,
                            buffer.VertexBufferCount,
                            buffer.IndexBuffer,
                            buffer.IndexBufferCount,
                            buffer.VertexFormat,
                            buffer.EffectInstance, PrimitiveType.LineList, 0,
                            buffer.PrimitiveCount);
                    }
                }
            }
            catch (Exception exception)
            {
                RenderFailed?.Invoke(this, new RenderFailedEventArgs
                {
                    ExceptionObject = exception
                });
            }
        }
    }

    private void MapGeometryBuffer()
    {
        RenderHelper.MapCurveSurfaceBuffer(_surfaceBuffer, _vertices, _diameter);
        RenderHelper.MapCurveBuffer(_curveBuffer, _vertices, _diameter);
        MapDirectionsBuffer();
    }

    private void MapDirectionsBuffer()
    {
        var verticalOffset = 0d;

        for (var i = 0; i < _vertices.Count - 1; i++)
        {
            var startPoint = _vertices[i];
            var endPoint = _vertices[i + 1];
            var centerPoint = (startPoint + endPoint) / 2;
            var buffer = CreateNormalBuffer(i);

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


    private RenderingBufferStorage CreateNormalBuffer(int vertexIndex)
    {
        RenderingBufferStorage buffer;
        if (_normalsBuffers.Count > vertexIndex)
        {
            buffer = _normalsBuffers[vertexIndex];
        }
        else
        {
            buffer = new RenderingBufferStorage();
            _normalsBuffers.Add(buffer);
        }

        return buffer;
    }

    private void UpdateEffects()
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

    public void UpdateSurfaceColor(Color value)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _surfaceColor = value;
            _hasEffectsUpdates = true;

            uiDocument.UpdateAllOpenViews();
        }
    }

    public void UpdateCurveColor(Color value)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _curveColor = value;
            _hasEffectsUpdates = true;

            uiDocument.UpdateAllOpenViews();
        }
    }

    public void UpdateDirectionColor(Color value)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _directionColor = value;
            _hasEffectsUpdates = true;

            uiDocument.UpdateAllOpenViews();
        }
    }

    public void UpdateDiameter(double value)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _diameter = value;
            _hasGeometryUpdates = true;

            uiDocument.UpdateAllOpenViews();
        }
    }

    public void UpdateTransparency(double value)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _transparency = value;
            _hasEffectsUpdates = true;

            uiDocument.UpdateAllOpenViews();
        }
    }


    public void UpdateSurfaceVisibility(bool visible)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _drawSurface = visible;
            uiDocument.UpdateAllOpenViews();
        }
    }

    public void UpdateCurveVisibility(bool visible)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _drawCurve = visible;
            uiDocument.UpdateAllOpenViews();
        }
    }

    public void UpdateDirectionVisibility(bool visible)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _drawDirection = visible;
            uiDocument.UpdateAllOpenViews();
        }
    }

    public void Register(IList<XYZ> vertices)
    {
        _vertices = vertices;
        RegisterServerEvent.Raise();
    }

    public void Unregister()
    {
        UnregisterServerEvent.Raise();
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private void RegisterServer(UIApplication application)
    {
        if (application.ActiveUIDocument is null) return;

        var directContextService = (MultiServerService) ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DirectContext3DService);
        var serverIds = directContextService.GetActiveServerIds();

        directContextService.AddServer(this);
        serverIds.Add(GetServerId());
        directContextService.SetActiveServers(serverIds);

        application.ActiveUIDocument.UpdateAllOpenViews();
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private void UnregisterServer(UIApplication application)
    {
        var directContextService = (MultiServerService) ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DirectContext3DService);
        directContextService.RemoveServer(GetServerId());

        application.ActiveUIDocument?.UpdateAllOpenViews();
    }

    public event EventHandler<RenderFailedEventArgs>? RenderFailed;
}