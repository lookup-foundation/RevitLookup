using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.Services.Settings;
using RevitLookup.Abstractions.ViewModels.Visualization;
using RevitLookup.Core.Visualization;
using RevitLookup.Core.Visualization.Events;
using Color = System.Windows.Media.Color;

namespace RevitLookup.ViewModels.Visualization;

[UsedImplicitly]
public sealed partial class FaceVisualizationViewModel(
    ISettingsService settingsService,
    INotificationService notificationService,
    ILogger<FaceVisualizationViewModel> logger)
    : ObservableObject, IFaceVisualizationViewModel
{
    private readonly FaceVisualizationServer _server = new();

    [ObservableProperty]
    public partial double Extrusion { get; set; } = settingsService.VisualizationSettings.FaceSettings.Extrusion;

    [ObservableProperty]
    public partial double Transparency { get; set; } = settingsService.VisualizationSettings.FaceSettings.Transparency;

    [ObservableProperty]
    public partial Color SurfaceColor { get; set; } = settingsService.VisualizationSettings.FaceSettings.SurfaceColor;

    [ObservableProperty]
    public partial Color MeshColor { get; set; } = settingsService.VisualizationSettings.FaceSettings.MeshColor;

    [ObservableProperty]
    public partial Color NormalVectorColor { get; set; } = settingsService.VisualizationSettings.FaceSettings.NormalVectorColor;

    [ObservableProperty]
    public partial bool ShowSurface { get; set; } = settingsService.VisualizationSettings.FaceSettings.ShowSurface;

    [ObservableProperty]
    public partial bool ShowMeshGrid { get; set; } = settingsService.VisualizationSettings.FaceSettings.ShowMeshGrid;

    [ObservableProperty]
    public partial bool ShowNormalVector { get; set; } = settingsService.VisualizationSettings.FaceSettings.ShowNormalVector;

    public double MinExtrusion => settingsService.VisualizationSettings.FaceSettings.MinExtrusion;

    public void RegisterServer(object faceObject)
    {
        if (faceObject is not Face face)
        {
            throw new ArgumentException($"Argument must be of type {nameof(Face)}", nameof(faceObject));
        }

        UpdateShowSurface(ShowSurface);
        UpdateShowMeshGrid(ShowMeshGrid);
        UpdateShowNormalVector(ShowNormalVector);

        UpdateSurfaceColor(SurfaceColor);
        UpdateMeshColor(MeshColor);
        UpdateNormalVectorColor(NormalVectorColor);

        UpdateTransparency(Transparency);
        UpdateExtrusion(Extrusion);

        _server.RenderFailed += HandleRenderFailure;
        _server.Register(face);
    }

    public void UnregisterServer()
    {
        _server.RenderFailed -= HandleRenderFailure;
        _server.Unregister();
    }

    private void HandleRenderFailure(object? sender, RenderFailedEventArgs args)
    {
        LogRenderError(logger, args.ExceptionObject);
        notificationService.ShowError("Render error", args.ExceptionObject);
    }

    partial void OnSurfaceColorChanged(Color value)
    {
        settingsService.VisualizationSettings.FaceSettings.SurfaceColor = value;
        UpdateSurfaceColor(value);
    }

    partial void OnMeshColorChanged(Color value)
    {
        settingsService.VisualizationSettings.FaceSettings.MeshColor = value;
        UpdateMeshColor(value);
    }

    partial void OnNormalVectorColorChanged(Color value)
    {
        settingsService.VisualizationSettings.FaceSettings.NormalVectorColor = value;
        UpdateNormalVectorColor(value);
    }

    partial void OnExtrusionChanged(double value)
    {
        settingsService.VisualizationSettings.FaceSettings.Extrusion = value;
        UpdateExtrusion(value);
    }

    partial void OnTransparencyChanged(double value)
    {
        settingsService.VisualizationSettings.FaceSettings.Transparency = value;
        UpdateTransparency(value);
    }

    partial void OnShowSurfaceChanged(bool value)
    {
        settingsService.VisualizationSettings.FaceSettings.ShowSurface = value;
        UpdateShowSurface(value);
    }

    partial void OnShowMeshGridChanged(bool value)
    {
        settingsService.VisualizationSettings.FaceSettings.ShowMeshGrid = value;
        UpdateShowMeshGrid(value);
    }

    partial void OnShowNormalVectorChanged(bool value)
    {
        settingsService.VisualizationSettings.FaceSettings.ShowNormalVector = value;
        UpdateShowNormalVector(value);
    }

    private void UpdateSurfaceColor(Color value)
    {
        _server.UpdateSurfaceColor(new Autodesk.Revit.DB.Color(value.R, value.G, value.B));
    }

    private void UpdateMeshColor(Color value)
    {
        _server.UpdateMeshGridColor(new Autodesk.Revit.DB.Color(value.R, value.G, value.B));
    }

    private void UpdateNormalVectorColor(Color value)
    {
        _server.UpdateNormalVectorColor(new Autodesk.Revit.DB.Color(value.R, value.G, value.B));
    }

    private void UpdateExtrusion(double value)
    {
        _server.UpdateExtrusion(value / 12);
    }

    private void UpdateTransparency(double value)
    {
        _server.UpdateTransparency(value / 100);
    }

    private void UpdateShowSurface(bool value)
    {
        _server.UpdateSurfaceVisibility(value);
    }

    private void UpdateShowMeshGrid(bool value)
    {
        _server.UpdateMeshGridVisibility(value);
    }

    private void UpdateShowNormalVector(bool value)
    {
        _server.UpdateNormalVectorVisibility(value);
    }

    [LoggerMessage(LogLevel.Error, "Render error")]
    private static partial void LogRenderError(ILogger<FaceVisualizationViewModel> logger, Exception exception);
}