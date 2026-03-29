using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.Services.Settings;
using RevitLookup.Abstractions.ViewModels.Visualization;
using RevitLookup.Core.Visualization;
using RevitLookup.Core.Visualization.Events;
using Color = System.Windows.Media.Color;

namespace RevitLookup.ViewModels.Visualization;

[UsedImplicitly]
public sealed partial class BoundingBoxVisualizationViewModel(
    ISettingsService settingsService,
    INotificationService notificationService,
    ILogger<BoundingBoxVisualizationViewModel> logger)
    : ObservableObject, IBoundingBoxVisualizationViewModel
{
    private readonly BoundingBoxVisualizationServer _server = new();

    [ObservableProperty]
    public partial double Transparency { get; set; } = settingsService.VisualizationSettings.BoundingBoxSettings.Transparency;

    [ObservableProperty]
    public partial Color SurfaceColor { get; set; } = settingsService.VisualizationSettings.BoundingBoxSettings.SurfaceColor;

    [ObservableProperty]
    public partial Color EdgeColor { get; set; } = settingsService.VisualizationSettings.BoundingBoxSettings.EdgeColor;

    [ObservableProperty]
    public partial Color AxisColor { get; set; } = settingsService.VisualizationSettings.BoundingBoxSettings.AxisColor;

    [ObservableProperty]
    public partial bool ShowSurface { get; set; } = settingsService.VisualizationSettings.BoundingBoxSettings.ShowSurface;

    [ObservableProperty]
    public partial bool ShowEdge { get; set; } = settingsService.VisualizationSettings.BoundingBoxSettings.ShowEdge;

    [ObservableProperty]
    public partial bool ShowAxis { get; set; } = settingsService.VisualizationSettings.BoundingBoxSettings.ShowAxis;

    public void RegisterServer(object boxObject)
    {
        if (boxObject is not BoundingBoxXYZ box)
        {
            throw new ArgumentException($"Argument must be of type {nameof(BoundingBoxXYZ)}", nameof(boxObject));
        }

        UpdateShowSurface(ShowSurface);
        UpdateShowEdge(ShowEdge);
        UpdateShowAxis(ShowAxis);

        UpdateSurfaceColor(SurfaceColor);
        UpdateEdgeColor(EdgeColor);
        UpdateAxisColor(AxisColor);

        UpdateTransparency(Transparency);

        _server.RenderFailed += HandleRenderFailure;
        _server.Register(box);
    }

    public void UnregisterServer()
    {
        _server.RenderFailed -= HandleRenderFailure;
        _server.Unregister();
    }

    private void HandleRenderFailure(object? sender, RenderFailedEventArgs args)
    {
        logger.LogError(args.ExceptionObject, "Render error");
        notificationService.ShowError("Render error", args.ExceptionObject);
    }

    partial void OnTransparencyChanged(double value)
    {
        settingsService.VisualizationSettings.BoundingBoxSettings.Transparency = value;
        UpdateTransparency(value);
    }

    partial void OnSurfaceColorChanged(Color value)
    {
        settingsService.VisualizationSettings.BoundingBoxSettings.SurfaceColor = value;
        UpdateSurfaceColor(value);
    }

    partial void OnEdgeColorChanged(Color value)
    {
        settingsService.VisualizationSettings.BoundingBoxSettings.EdgeColor = value;
        UpdateEdgeColor(value);
    }

    partial void OnAxisColorChanged(Color value)
    {
        settingsService.VisualizationSettings.BoundingBoxSettings.AxisColor = value;
        UpdateAxisColor(value);
    }

    partial void OnShowSurfaceChanged(bool value)
    {
        settingsService.VisualizationSettings.BoundingBoxSettings.ShowSurface = value;
        UpdateShowSurface(value);
    }

    partial void OnShowEdgeChanged(bool value)
    {
        settingsService.VisualizationSettings.BoundingBoxSettings.ShowEdge = value;
        UpdateShowEdge(value);
    }

    partial void OnShowAxisChanged(bool value)
    {
        settingsService.VisualizationSettings.BoundingBoxSettings.ShowEdge = value;
        UpdateShowAxis(value);
    }

    private void UpdateSurfaceColor(Color value)
    {
        _server.UpdateSurfaceColor(new Autodesk.Revit.DB.Color(value.R, value.G, value.B));
    }

    private void UpdateEdgeColor(Color value)
    {
        _server.UpdateEdgeColor(new Autodesk.Revit.DB.Color(value.R, value.G, value.B));
    }

    private void UpdateAxisColor(Color value)
    {
        _server.UpdateAxisColor(new Autodesk.Revit.DB.Color(value.R, value.G, value.B));
    }

    private void UpdateTransparency(double value)
    {
        _server.UpdateTransparency(value / 100);
    }

    private void UpdateShowSurface(bool value)
    {
        _server.UpdateSurfaceVisibility(value);
    }

    private void UpdateShowEdge(bool value)
    {
        _server.UpdateEdgeVisibility(value);
    }

    private void UpdateShowAxis(bool value)
    {
        _server.UpdateAxisVisibility(value);
    }
}