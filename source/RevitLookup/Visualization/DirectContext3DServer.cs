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
using RevitLookup.Visualization.Rendering;

namespace RevitLookup.Visualization;

/// <summary>
///     Represents a Revit direct-context 3D server that renders ad-hoc visualization geometry into the registered document's open views.
/// </summary>
public abstract partial class DirectContext3DServer : IDirectContext3DServer
{
    private readonly Guid _guid = Guid.NewGuid();
    private readonly Lock _renderLock = new();
    private UIDocument? _uiDocument;

    /// <summary>
    ///     Gets or sets a value indicating whether the geometry buffers must be remapped before the next render pass.
    /// </summary>
    protected bool HasGeometryUpdates { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether the render effects must be reapplied before the next render pass.
    /// </summary>
    protected bool HasEffectsUpdates { get; set; } = true;

    /// <inheritdoc />
    public abstract string GetName();

    /// <inheritdoc />
    public abstract string GetDescription();

    /// <inheritdoc />
    public abstract bool UseInTransparentPass(View view);

    /// <inheritdoc />
    public abstract Outline? GetBoundingBox(View view);

    /// <inheritdoc />
    public Guid GetServerId()
    {
        return _guid;
    }

    /// <inheritdoc />
    public string GetVendorId()
    {
        return "RevitLookup";
    }

    /// <inheritdoc />
    public ExternalServiceId GetServiceId()
    {
        return ExternalServices.BuiltInExternalServices.DirectContext3DService;
    }

    /// <inheritdoc />
    public string GetApplicationId()
    {
        return string.Empty;
    }

    /// <inheritdoc />
    public string GetSourceId()
    {
        return string.Empty;
    }

    /// <inheritdoc />
    public bool UsesHandles()
    {
        return false;
    }

    /// <inheritdoc />
    public bool CanExecute(View view)
    {
        if (_uiDocument is null)
        {
            return false;
        }

        return view.Document.Equals(_uiDocument.Document);
    }

    /// <inheritdoc />
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

    /// <summary>
    ///     Registers the server with the Revit direct-context 3D external service and enables it for the active document.
    /// </summary>
    protected void Register()
    {
        RegisterServerEvent.Raise();
    }

    /// <summary>
    ///     Unregisters the server from the Revit direct-context 3D external service and disposes its render buffers.
    /// </summary>
    public void Unregister()
    {
        UnregisterServerEvent.Raise();
    }

    /// <summary>
    ///     Returns a value indicating whether the current render buffers are valid and can be rendered without remapping.
    /// </summary>
    /// <returns><see langword="true" /> if the buffers are valid; otherwise, <see langword="false" />.</returns>
    protected abstract bool AreBuffersValid();

    /// <summary>
    ///     Maps the visualization geometry into the render buffers.
    /// </summary>
    protected abstract void MapGeometryBuffer();

    /// <summary>
    ///     Applies the current colors and transparency to the render buffers' effects.
    /// </summary>
    protected abstract void UpdateEffects();

    /// <summary>
    ///     Flushes the render buffers to the active view.
    /// </summary>
    protected abstract void RenderBuffers();

    /// <summary>
    ///     Disposes the render buffers.
    /// </summary>
    /// <remarks>The base implementation does nothing. A derived server overrides it to dispose its own buffers.</remarks>
    protected virtual void DisposeBuffers()
    {
    }

    /// <summary>
    ///     Runs <paramref name="updateAction" /> under the render lock and refreshes all open views of the registered document.
    /// </summary>
    /// <param name="updateAction">The action that mutates the server's rendering state.</param>
    protected void UpdateViews(Action updateAction)
    {
        lock (_renderLock)
        {
            updateAction();
        }

        _uiDocument?.UpdateAllOpenViews();
    }

    /// <summary>
    ///     Flushes a triangle render buffer to the active view.
    /// </summary>
    /// <param name="buffer">The buffer holding the triangle geometry.</param>
    /// <param name="transparency">The transparency level the buffer is drawn with.</param>
    protected static void FlushTriangleBuffer(RenderingBufferStorage buffer, double transparency)
    {
        var isTransparentPass = DrawContext.IsTransparentPass();
        if ((isTransparentPass && transparency > 0) || (!isTransparentPass && transparency == 0))
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

    /// <summary>
    ///     Flushes a line render buffer to the active view.
    /// </summary>
    /// <param name="buffer">The buffer holding the line geometry.</param>
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
        if (application.ActiveUIDocument is null)
        {
            return;
        }

        _uiDocument = application.ActiveUIDocument;

        var directContextService = (MultiServerService)ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DirectContext3DService);
        var serverIds = directContextService.GetActiveServerIds();

        directContextService.AddServer(this);
        serverIds.Add(GetServerId());
        directContextService.SetActiveServers(serverIds);

        _uiDocument.UpdateAllOpenViews();
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private void UnregisterServer(UIApplication application)
    {
        var directContextService = (MultiServerService)ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DirectContext3DService);
        directContextService.RemoveServer(GetServerId());
        DisposeBuffers();

        _uiDocument?.UpdateAllOpenViews();
    }

    /// <summary>
    ///     An event that is raised when rendering the scene throws an exception.
    /// </summary>
    public event EventHandler<RenderFailedEventArgs>? RenderFailed;
}
