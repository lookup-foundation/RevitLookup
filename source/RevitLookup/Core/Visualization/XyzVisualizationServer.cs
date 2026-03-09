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

public sealed partial class XyzVisualizationServer : IDirectContext3DServer
{
    private XYZ _point = null!; //Cant be null after registration
    private bool _hasEffectsUpdates = true;
    private bool _hasGeometryUpdates = true;

    private readonly Guid _guid = Guid.NewGuid();
    private readonly Lock _renderLock = new();

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

    private double _transparency;
    private double _axisLength;

    private Color _xColor = Color.InvalidColorValue;
    private Color _yColor = Color.InvalidColorValue;
    private Color _zColor = Color.InvalidColorValue;

    private bool _drawPlane;
    private bool _drawXAxis;
    private bool _drawYAxis;
    private bool _drawZAxis;

    public Guid GetServerId() => _guid;
    public string GetVendorId() => "RevitLookup";
    public string GetName() => "XYZ visualization server";
    public string GetDescription() => "XYZ geometry visualization";
    public ExternalServiceId GetServiceId() => ExternalServices.BuiltInExternalServices.DirectContext3DService;
    public string GetApplicationId() => string.Empty;
    public string GetSourceId() => string.Empty;
    public bool UsesHandles() => false;
    public bool CanExecute(View view) => true;
    public bool UseInTransparentPass(View view) => _drawPlane && _transparency > 0;

    public Outline GetBoundingBox(View view)
    {
        var minPoint = new XYZ(_point.X - _axisLength, _point.Y - _axisLength, _point.Z - _axisLength);
        var maxPoint = new XYZ(_point.X + _axisLength, _point.Y + _axisLength, _point.Z + _axisLength);

        return new Outline(minPoint, maxPoint);
    }

    public void RenderScene(View view, DisplayStyle displayStyle)
    {
        lock (_renderLock)
        {
            try
            {
                if (_hasGeometryUpdates)
                {
                    UpdateGeometryBuffer();
                    _hasGeometryUpdates = false;
                }

                if (_hasEffectsUpdates)
                {
                    UpdateEffects();
                    _hasEffectsUpdates = false;
                }

                if (_drawXAxis)
                {
                    RenderAxisBuffer(_axisBuffers[0]);
                    RenderPlaneBuffer(_planeBuffers[0]);
                }

                if (_drawYAxis)
                {
                    RenderAxisBuffer(_axisBuffers[1]);
                    RenderPlaneBuffer(_planeBuffers[1]);
                }

                if (_drawZAxis)
                {
                    RenderAxisBuffer(_axisBuffers[2]);
                    RenderPlaneBuffer(_planeBuffers[2]);
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

    private void RenderPlaneBuffer(RenderingBufferStorage buffer)
    {
        if (!_drawPlane) return;

        var isTransparentPass = DrawContext.IsTransparentPass();
        if (isTransparentPass && _transparency > 0 || !isTransparentPass && _transparency == 0)
        {
            DrawContext.FlushBuffer(buffer.VertexBuffer,
                buffer.VertexBufferCount,
                buffer.IndexBuffer,
                buffer.IndexBufferCount,
                buffer.VertexFormat,
                buffer.EffectInstance, PrimitiveType.TriangleList, 0,
                buffer.PrimitiveCount);
        }
    }

    private void RenderAxisBuffer(RenderingBufferStorage buffer)
    {
        DrawContext.FlushBuffer(buffer.VertexBuffer,
            buffer.VertexBufferCount,
            buffer.IndexBuffer,
            buffer.IndexBufferCount,
            buffer.VertexFormat,
            buffer.EffectInstance, PrimitiveType.LineList, 0,
            buffer.PrimitiveCount);
    }

    private void UpdateGeometryBuffer()
    {
        MapNormalBuffer();
        MapPlaneBuffer();
    }

    private void MapNormalBuffer()
    {
        var normalExtendLength = _axisLength > 1 ? 0.8 : _axisLength * 0.8;
        for (var i = 0; i < _normals.Length; i++)
        {
            var normal = _normals[i];
            var buffer = _axisBuffers[i];
            RenderHelper.MapNormalVectorBuffer(buffer, _point - normal * (_axisLength + normalExtendLength), normal, 2 * (_axisLength + normalExtendLength));
        }
    }

    private void MapPlaneBuffer()
    {
        for (var i = 0; i < _normals.Length; i++)
        {
            var normal = _normals[i];
            var buffer = _planeBuffers[i];
            RenderHelper.MapSideBuffer(buffer, _point - normal * _axisLength, _point + normal * _axisLength);
        }
    }

    private void UpdateEffects()
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

    public void UpdateXColor(Color value)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _xColor = value;
            _hasEffectsUpdates = true;

            uiDocument.UpdateAllOpenViews();
        }
    }

    public void UpdateYColor(Color value)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _yColor = value;
            _hasEffectsUpdates = true;

            uiDocument.UpdateAllOpenViews();
        }
    }

    public void UpdateZColor(Color value)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _zColor = value;
            _hasEffectsUpdates = true;

            uiDocument.UpdateAllOpenViews();
        }
    }

    public void UpdateAxisLength(double value)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _axisLength = value;
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

    public void UpdatePlaneVisibility(bool visible)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _drawPlane = visible;
            uiDocument.UpdateAllOpenViews();
        }
    }

    public void UpdateXAxisVisibility(bool visible)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _drawXAxis = visible;
            uiDocument.UpdateAllOpenViews();
        }
    }

    public void UpdateYAxisVisibility(bool visible)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _drawYAxis = visible;
            uiDocument.UpdateAllOpenViews();
        }
    }

    public void UpdateZAxisVisibility(bool visible)
    {
        var uiDocument = RevitContext.ActiveUiDocument;
        if (uiDocument is null) return;

        lock (_renderLock)
        {
            _drawZAxis = visible;
            uiDocument.UpdateAllOpenViews();
        }
    }

    public void Register(XYZ point)
    {
        _point = point;
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