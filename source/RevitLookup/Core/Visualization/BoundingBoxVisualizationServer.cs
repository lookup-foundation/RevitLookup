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

public sealed partial class BoundingBoxVisualizationServer : IDirectContext3DServer
{
    private BoundingBoxXYZ _box = null!; //Cant be null after registration
    private bool _hasGeometryUpdates = true;
    private bool _hasEffectsUpdates = true;

    private readonly Guid _guid = Guid.NewGuid();
    private readonly Lock _renderLock = new();
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

    private double _transparency;

    private bool _drawSurface;
    private bool _drawEdge;
    private bool _drawAxis;

    private Color _surfaceColor = Color.InvalidColorValue;
    private Color _edgeColor = Color.InvalidColorValue;
    private Color _axisColor = Color.InvalidColorValue;

    public Guid GetServerId() => _guid;
    public string GetVendorId() => "RevitLookup";
    public string GetName() => "BoundingBoxXYZ visualization server";
    public string GetDescription() => "BoundingBoxXYZ geometry visualization";
    public ExternalServiceId GetServiceId() => ExternalServices.BuiltInExternalServices.DirectContext3DService;
    public string GetApplicationId() => string.Empty;
    public string GetSourceId() => string.Empty;
    public bool UsesHandles() => false;
    public bool CanExecute(View view) => true;
    public bool UseInTransparentPass(View view) => _drawSurface && _transparency > 0;

    public Outline GetBoundingBox(View view)
    {
        return new Outline(_box.Min, _box.Max);
    }

    public void RenderScene(View view, DisplayStyle displayStyle)
    {
        lock (_renderLock)
        {
            try
            {
                if (_hasGeometryUpdates || !_surfaceBuffer.IsValid() || !_edgeBuffer.IsValid())
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

                if (_drawEdge)
                {
                    DrawContext.FlushBuffer(_edgeBuffer.VertexBuffer,
                        _edgeBuffer.VertexBufferCount,
                        _edgeBuffer.IndexBuffer,
                        _edgeBuffer.IndexBufferCount,
                        _edgeBuffer.VertexFormat,
                        _edgeBuffer.EffectInstance, PrimitiveType.LineList, 0,
                        _edgeBuffer.PrimitiveCount);
                }

                if (_drawAxis)
                {
                    foreach (var buffer in _axisBuffers)
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
        RenderHelper.MapBoundingBoxSurfaceBuffer(_surfaceBuffer, _box);
        RenderHelper.MapBoundingBoxEdgeBuffer(_edgeBuffer, _box);
        MapAxisBuffers();
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

            RenderHelper.MapNormalVectorBuffer(minBuffer, minPoint - unitVector * RevitApiContext.Application.ShortCurveTolerance, normal, axisLength);
            RenderHelper.MapNormalVectorBuffer(maxBuffer, maxPoint + unitVector * RevitApiContext.Application.ShortCurveTolerance, -normal, axisLength);
        }
    }

    private void UpdateEffects()
    {
        _surfaceBuffer.EffectInstance ??= new EffectInstance(_surfaceBuffer.FormatBits);
        _surfaceBuffer.EffectInstance.SetColor(_surfaceColor);
        _surfaceBuffer.EffectInstance.SetTransparency(_transparency);

        _edgeBuffer.EffectInstance ??= new EffectInstance(_edgeBuffer.FormatBits);
        _edgeBuffer.EffectInstance.SetColor(_edgeColor);

        foreach (var buffer in _axisBuffers)
        {
            buffer.EffectInstance ??= new EffectInstance(_edgeBuffer.FormatBits);
            buffer.EffectInstance.SetColor(_axisColor);
        }
    }

    public void UpdateSurfaceColor(Color color)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _surfaceColor = color;
            _hasEffectsUpdates = true;

            uiDocument.UpdateAllOpenViews();
        }
    }

    public void UpdateEdgeColor(Color color)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _edgeColor = color;
            _hasEffectsUpdates = true;

            uiDocument.UpdateAllOpenViews();
        }
    }

    public void UpdateAxisColor(Color color)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _axisColor = color;
            _hasEffectsUpdates = true;

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

    public void UpdateEdgeVisibility(bool visible)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _drawEdge = visible;

            uiDocument.UpdateAllOpenViews();
        }
    }

    public void UpdateAxisVisibility(bool visible)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _drawAxis = visible;

            uiDocument.UpdateAllOpenViews();
        }
    }

    public void Register(BoundingBoxXYZ box)
    {
        _box = box;
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