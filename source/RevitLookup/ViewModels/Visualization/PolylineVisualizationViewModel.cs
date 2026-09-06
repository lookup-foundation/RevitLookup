using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.Settings;
using RevitLookup.Abstractions.ViewModels.Visualization;
using RevitLookup.Visualization;
using Color = System.Windows.Media.Color;

namespace RevitLookup.ViewModels.Visualization;

/// <summary>
///     Represents the view model for polyline visualization, rendering a tessellated <see cref="Curve" /> or <see cref="Edge" /> through a dedicated Revit visualization server.
/// </summary>
/// <param name="settingsService">The service that persists and supplies the polyline visualization settings.</param>
/// <param name="notificationService">The service used to report rendering failures.</param>
/// <param name="logger">The logger used to record rendering failures.</param>
[UsedImplicitly]
public sealed partial class PolylineVisualizationViewModel(
    ISettingsService settingsService,
    INotificationService notificationService,
    ILogger<PolylineVisualizationViewModel> logger)
    : ObservableObject, IPolylineVisualizationViewModel
{
    private readonly PolylineVisualizationServer _server = new();

    /// <inheritdoc />
    [ObservableProperty]
    public partial double Diameter { get; set; } = settingsService.VisualizationSettings.PolylineSettings.Diameter;

    /// <inheritdoc />
    [ObservableProperty]
    public partial double Transparency { get; set; } = settingsService.VisualizationSettings.PolylineSettings.Transparency;

    /// <inheritdoc />
    [ObservableProperty]
    public partial Color SurfaceColor { get; set; } = settingsService.VisualizationSettings.PolylineSettings.SurfaceColor;

    /// <inheritdoc />
    [ObservableProperty]
    public partial Color CurveColor { get; set; } = settingsService.VisualizationSettings.PolylineSettings.CurveColor;

    /// <inheritdoc />
    [ObservableProperty]
    public partial Color DirectionColor { get; set; } = settingsService.VisualizationSettings.PolylineSettings.DirectionColor;

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool ShowSurface { get; set; } = settingsService.VisualizationSettings.PolylineSettings.ShowSurface;

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool ShowCurve { get; set; } = settingsService.VisualizationSettings.PolylineSettings.ShowCurve;

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool ShowDirection { get; set; } = settingsService.VisualizationSettings.PolylineSettings.ShowDirection;

    /// <inheritdoc />
    public double MinThickness => settingsService.VisualizationSettings.PolylineSettings.MinThickness;

    /// <inheritdoc />
    /// <exception cref="ArgumentException"><paramref name="curveOrEdge" /> is not a <see cref="Curve" /> or <see cref="Edge" />.</exception>
    public void RegisterServer(object curveOrEdge)
    {
        Initialize();
        _server.RenderFailed += HandleRenderFailure;

        switch (curveOrEdge)
        {
            case Curve curve:
                _server.Register(curve.Tessellate());
                break;
            case Edge edge:
                _server.Register(edge.Tessellate());
                break;
            default:
                throw new ArgumentException("Unexpected polyline type", nameof(curveOrEdge));
        }
    }

    /// <inheritdoc />
    public void UnregisterServer()
    {
        _server.RenderFailed -= HandleRenderFailure;
        _server.Unregister();
    }

    private void Initialize()
    {
        UpdateShowSurface(ShowSurface);
        UpdateShowCurve(ShowCurve);
        UpdateShowDirection(ShowDirection);

        UpdateSurfaceColor(SurfaceColor);
        UpdateCurveColor(CurveColor);
        UpdateDirectionColor(DirectionColor);

        UpdateTransparency(Transparency);
        UpdateDiameter(Diameter);
    }

    private void HandleRenderFailure(object? sender, RenderFailedEventArgs args)
    {
        LogRenderError(logger, args.ExceptionObject);
        notificationService.ShowError("Render error", args.ExceptionObject);
    }

    partial void OnSurfaceColorChanged(Color value)
    {
        settingsService.VisualizationSettings.PolylineSettings.SurfaceColor = value;
        UpdateSurfaceColor(value);
    }

    partial void OnCurveColorChanged(Color value)
    {
        settingsService.VisualizationSettings.PolylineSettings.CurveColor = value;
        UpdateCurveColor(value);
    }

    partial void OnDirectionColorChanged(Color value)
    {
        settingsService.VisualizationSettings.PolylineSettings.DirectionColor = value;
        UpdateDirectionColor(value);
    }

    partial void OnDiameterChanged(double value)
    {
        settingsService.VisualizationSettings.PolylineSettings.Diameter = value;
        UpdateDiameter(value);
    }

    partial void OnTransparencyChanged(double value)
    {
        settingsService.VisualizationSettings.PolylineSettings.Transparency = value;
        UpdateTransparency(value);
    }

    partial void OnShowSurfaceChanged(bool value)
    {
        settingsService.VisualizationSettings.PolylineSettings.ShowSurface = value;
        UpdateShowSurface(value);
    }

    partial void OnShowCurveChanged(bool value)
    {
        settingsService.VisualizationSettings.PolylineSettings.ShowCurve = value;
        UpdateShowCurve(value);
    }

    partial void OnShowDirectionChanged(bool value)
    {
        settingsService.VisualizationSettings.PolylineSettings.ShowDirection = value;
        UpdateShowDirection(value);
    }

    private void UpdateSurfaceColor(Color value)
    {
        _server.UpdateSurfaceColor(new Autodesk.Revit.DB.Color(value.R, value.G, value.B));
    }

    private void UpdateCurveColor(Color value)
    {
        _server.UpdateCurveColor(new Autodesk.Revit.DB.Color(value.R, value.G, value.B));
    }

    private void UpdateDirectionColor(Color value)
    {
        _server.UpdateDirectionColor(new Autodesk.Revit.DB.Color(value.R, value.G, value.B));
    }

    private void UpdateDiameter(double value)
    {
        _server.UpdateDiameter(value / 12);
    }

    private void UpdateTransparency(double value)
    {
        _server.UpdateTransparency(value / 100);
    }

    private void UpdateShowSurface(bool value)
    {
        _server.UpdateSurfaceVisibility(value);
    }

    private void UpdateShowCurve(bool value)
    {
        _server.UpdateCurveVisibility(value);
    }

    private void UpdateShowDirection(bool value)
    {
        _server.UpdateDirectionVisibility(value);
    }

    [LoggerMessage(LogLevel.Error, "Render error")]
    private static partial void LogRenderError(ILogger<PolylineVisualizationViewModel> logger, Exception exception);
}
