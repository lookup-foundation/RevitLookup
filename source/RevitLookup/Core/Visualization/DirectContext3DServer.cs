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

namespace RevitLookup.Core.Visualization;

public abstract partial class DirectContext3DServer : IDirectContext3DServer
{
    private UIDocument? _uiDocument;

    private readonly Guid _guid = Guid.NewGuid();
    private readonly Lock _renderLock = new();

    protected bool HasGeometryUpdates { get; set; } = true;
    protected bool HasEffectsUpdates { get; set; } = true;

    public abstract string GetName();
    public abstract string GetDescription();
    public abstract bool UseInTransparentPass(View view);
    public abstract Outline? GetBoundingBox(View view);    
    
    public Guid GetServerId()
    {
        return _guid;
    }

    public string GetVendorId()
    {
        return "RevitLookup";
    }

    public ExternalServiceId GetServiceId()
    {
        return ExternalServices.BuiltInExternalServices.DirectContext3DService;
    }

    public string GetApplicationId()
    {
        return string.Empty;
    }

    public string GetSourceId()
    {
        return string.Empty;
    }

    public bool UsesHandles()
    {
        return false;
    }

    public bool CanExecute(View view)
    {
        if (_uiDocument is null) return false;

        return view.Document.Equals(_uiDocument.Document);
    }

    public void RenderScene(View view, DisplayStyle displayStyle)
    {
        lock (_renderLock)
        {
            try
            {
                if (HasGeometryUpdates || !AreBuffersValid())
                {
                    MapGeometryBuffer();
                    HasGeometryUpdates = false;
                }

                if (HasEffectsUpdates)
                {
                    UpdateEffects();
                    HasEffectsUpdates = false;
                }

                RenderBuffers();
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

    protected void Register()
    {
        RegisterServerEvent.Raise();
    }
    
    public void Unregister()
    {
        UnregisterServerEvent.Raise();
    }

    protected abstract bool AreBuffersValid();
    protected abstract void MapGeometryBuffer();
    protected abstract void UpdateEffects();
    protected abstract void RenderBuffers();

    protected virtual void DisposeBuffers()
    {
    }

    protected void UpdateViews(Action updateAction)
    {
        lock (_renderLock)
        {
            updateAction();
        }

        _uiDocument?.UpdateAllOpenViews();
    }

    protected static void FlushTriangleBuffer(RenderingBufferStorage buffer, double transparency)
    {
        var isTransparentPass = DrawContext.IsTransparentPass();
        if (isTransparentPass && transparency > 0 || !isTransparentPass && transparency == 0)
        {
            DrawContext.FlushBuffer(
                buffer.VertexBuffer,
                buffer.VertexBufferCount,
                buffer.IndexBuffer,
                buffer.IndexBufferCount,
                buffer.VertexFormat,
                buffer.EffectInstance,
                PrimitiveType.TriangleList, 0,
                buffer.PrimitiveCount);
        }
    }

    protected static void FlushLineBuffer(RenderingBufferStorage buffer)
    {
        DrawContext.FlushBuffer(
            buffer.VertexBuffer,
            buffer.VertexBufferCount,
            buffer.IndexBuffer,
            buffer.IndexBufferCount,
            buffer.VertexFormat,
            buffer.EffectInstance,
            PrimitiveType.LineList, 0,
            buffer.PrimitiveCount);
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private void RegisterServer(UIApplication application)
    {
        if (application.ActiveUIDocument is null) return;

        _uiDocument = application.ActiveUIDocument;

        var directContextService = (MultiServerService) ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DirectContext3DService);
        var serverIds = directContextService.GetActiveServerIds();

        directContextService.AddServer(this);
        serverIds.Add(GetServerId());
        directContextService.SetActiveServers(serverIds);

        _uiDocument.UpdateAllOpenViews();
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private void UnregisterServer(UIApplication application)
    {
        var directContextService = (MultiServerService) ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DirectContext3DService);
        directContextService.RemoveServer(GetServerId());
        DisposeBuffers();

        _uiDocument?.UpdateAllOpenViews();
    }
    
    public event EventHandler<RenderFailedEventArgs>? RenderFailed;
}