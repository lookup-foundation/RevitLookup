using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.Services.Settings;
using RevitLookup.Abstractions.ViewModels.Visualization;
using RevitLookup.Core.Visualization;
using RevitLookup.Core.Visualization.Events;
using Color = System.Windows.Media.Color;

namespace RevitLookup.ViewModels.Visualization;

[UsedImplicitly]
public sealed partial class MeshVisualizationViewModel(
    ISettingsService settingsService,
    INotificationService notificationService,
    ILogger<MeshVisualizationViewModel> logger)
    : ObservableObject, IMeshVisualizationViewModel
{
    private readonly MeshVisualizationServer _server = new();

    [ObservableProperty]
    public partial double Extrusion { get; set; } = settingsService.VisualizationSettings.MeshSettings.Extrusion;

    [ObservableProperty]
    public partial double Transparency { get; set; } = settingsService.VisualizationSettings.MeshSettings.Transparency;

    [ObservableProperty]
    public partial Color SurfaceColor { get; set; } = settingsService.VisualizationSettings.MeshSettings.SurfaceColor;

    [ObservableProperty]
    public partial Color MeshColor { get; set; } = settingsService.VisualizationSettings.MeshSettings.MeshColor;

    [ObservableProperty]
    public partial Color NormalVectorColor { get; set; } = settingsService.VisualizationSettings.MeshSettings.NormalVectorColor;

    [ObservableProperty]
    public partial bool ShowSurface { get; set; } = settingsService.VisualizationSettings.MeshSettings.ShowSurface;

    [ObservableProperty]
    public partial bool ShowMeshGrid { get; set; } = settingsService.VisualizationSettings.MeshSettings.ShowMeshGrid;

    [ObservableProperty]
    public partial bool ShowNormalVector { get; set; } = settingsService.VisualizationSettings.MeshSettings.ShowNormalVector;

    public double MinExtrusion => settingsService.VisualizationSettings.MeshSettings.MinExtrusion;

    public void RegisterServer(object meshObject)
    {
        if (meshObject is not Mesh mesh)
        {
            throw new ArgumentException($"Argument must be of type {nameof(Mesh)}", nameof(meshObject));
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
        _server.Register(mesh);
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
        settingsService.VisualizationSettings.MeshSettings.SurfaceColor = value;
        UpdateSurfaceColor(value);
    }

    partial void OnMeshColorChanged(Color value)
    {
        settingsService.VisualizationSettings.MeshSettings.MeshColor = value;
        UpdateMeshColor(value);
    }

    partial void OnNormalVectorColorChanged(Color value)
    {
        settingsService.VisualizationSettings.MeshSettings.NormalVectorColor = value;
        UpdateNormalVectorColor(value);
    }

    partial void OnExtrusionChanged(double value)
    {
        settingsService.VisualizationSettings.MeshSettings.Extrusion = value;
        UpdateExtrusion(value);
    }

    partial void OnTransparencyChanged(double value)
    {
        settingsService.VisualizationSettings.MeshSettings.Transparency = value;
        UpdateTransparency(value);
    }

    partial void OnShowSurfaceChanged(bool value)
    {
        settingsService.VisualizationSettings.MeshSettings.ShowSurface = value;
        UpdateShowSurface(value);
    }

    partial void OnShowMeshGridChanged(bool value)
    {
        settingsService.VisualizationSettings.MeshSettings.ShowMeshGrid = value;
        UpdateShowMeshGrid(value);
    }

    partial void OnShowNormalVectorChanged(bool value)
    {
        settingsService.VisualizationSettings.MeshSettings.ShowNormalVector = value;
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
    private static partial void LogRenderError(ILogger<MeshVisualizationViewModel> logger, Exception exception);
}