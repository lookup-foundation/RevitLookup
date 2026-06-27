using System.IO;
using System.Text.Json;
using System.Windows.Media;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RevitLookup.Abstractions.Models.Settings;
using RevitLookup.Abstractions.Options;
using RevitLookup.Abstractions.Services.Settings;
using Wpf.Ui.Animations;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Color = System.Windows.Media.Color;

namespace RevitLookup.Services.Settings;

public sealed partial class SettingsService(
    IOptions<ResourceLocationsOptions> foldersOptions,
    IOptions<JsonSerializerOptions> jsonOptions,
    ILogger<SettingsService> logger)
    : ISettingsService
{
    private ApplicationSettings? _applicationSettings;
    private DecompositionSettings? _decompositionSettings;
    private VisualizationSettings? _visualizationSettings;

    public ApplicationSettings ApplicationSettings => _applicationSettings ?? throw new InvalidOperationException("Application settings is not loaded.");
    public DecompositionSettings DecompositionSettings => _decompositionSettings ?? throw new InvalidOperationException("Decomposition settings is not loaded.");
    public VisualizationSettings VisualizationSettings => _visualizationSettings ?? throw new InvalidOperationException("Visualization settings is not loaded.");

    public void SaveSettings()
    {
        SaveApplicationSettings();
        SaveDecompositionSettings();
        SaveVisualizationSettings();
    }

    public void LoadSettings()
    {
        LoadApplicationSettings();
        LoadDecompositionSettings();
        LoadVisualizationSettings();
    }

    private void SaveApplicationSettings()
    {
        var path = foldersOptions.Value.ApplicationSettingsPath;
        if (!File.Exists(path)) Directory.CreateDirectory(foldersOptions.Value.SettingsDirectory);

        var json = JsonSerializer.Serialize(_applicationSettings, jsonOptions.Value);
        File.WriteAllText(path, json);
    }

    private void SaveDecompositionSettings()
    {
        var path = foldersOptions.Value.DecompositionSettingsPath;
        if (!File.Exists(path)) Directory.CreateDirectory(foldersOptions.Value.SettingsDirectory);

        var json = JsonSerializer.Serialize(_decompositionSettings, jsonOptions.Value);
        File.WriteAllText(path, json);
    }

    private void SaveVisualizationSettings()
    {
        var path = foldersOptions.Value.VisualizationSettingsPath;
        if (!File.Exists(path)) Directory.CreateDirectory(foldersOptions.Value.SettingsDirectory);

        var json = JsonSerializer.Serialize(_visualizationSettings, jsonOptions.Value);
        File.WriteAllText(path, json);
    }

    private void LoadApplicationSettings()
    {
        var path = foldersOptions.Value.ApplicationSettingsPath;
        if (!File.Exists(path))
        {
            ResetApplicationSettings();
            return;
        }

        try
        {
            using var config = File.OpenRead(path);
            _applicationSettings = JsonSerializer.Deserialize<ApplicationSettings>(config, jsonOptions.Value);
        }
        catch (Exception exception)
        {
            LogApplicationSettingsLoadingError(logger, exception);
        }
        
        if (_applicationSettings is null)
        {
            ResetApplicationSettings();
        }
    }

    private void LoadDecompositionSettings()
    {
        var path = foldersOptions.Value.DecompositionSettingsPath;
        if (!File.Exists(path))
        {
            ResetDecompositionSettings();
            return;
        }

        try
        {
            using var config = File.OpenRead(path);
            _decompositionSettings = JsonSerializer.Deserialize<DecompositionSettings>(config, jsonOptions.Value);
        }
        catch (Exception exception)
        {
            LogDecompositionSettingsLoadingError(logger, exception);
        }

        if (_decompositionSettings is null)
        {
            ResetDecompositionSettings();
        }
    }

    private void LoadVisualizationSettings()
    {
        var path = foldersOptions.Value.VisualizationSettingsPath;
        if (!File.Exists(path))
        {
            ResetVisualizationSettings();
            return;
        }

        try
        {
            using var config = File.OpenRead(path);
            _visualizationSettings = JsonSerializer.Deserialize<VisualizationSettings>(config, jsonOptions.Value);
        }
        catch (Exception exception)
        {
            LogVisualizationSettingsLoadingError(logger, exception);
        }

        if (_visualizationSettings is null)
        {
            ResetVisualizationSettings();
        }
    }

    public void ResetApplicationSettings()
    {
        _applicationSettings = new ApplicationSettings
        {
#if REVIT2024_OR_GREATER
            Theme = ApplicationTheme.Auto,
#else
            Theme = ApplicationTheme.Light,
#endif
            Background = WindowBackdropType.None,
            Transition = Transition.None,
            UseHardwareRendering = true
        };
    }

    public void ResetDecompositionSettings()
    {
        _decompositionSettings = new DecompositionSettings
        {
            IncludeStatic = true,
            IncludeEvents = true,
            IncludeExtensions = true,
        };
    }

    public void ResetVisualizationSettings()
    {
        _visualizationSettings = new VisualizationSettings
        {
            BoundingBoxSettings = new BoundingBoxVisualizationSettings
            {
                Transparency = 60,
                SurfaceColor = Colors.DodgerBlue,
                EdgeColor = Color.FromArgb(255, 30, 81, 255),
                AxisColor = Color.FromArgb(255, 255, 89, 30),
                ShowSurface = true,
                ShowEdge = true,
                ShowAxis = true
            },
            FaceSettings = new FaceVisualizationSettings
            {
                Transparency = 20,
                Extrusion = RevitApiContext.Application.VertexTolerance * 12,
                MinExtrusion = RevitApiContext.Application.VertexTolerance * 12,
                SurfaceColor = Colors.DodgerBlue,
                MeshColor = Color.FromArgb(255, 30, 81, 255),
                NormalVectorColor = Color.FromArgb(255, 255, 89, 30),
                ShowSurface = true,
                ShowMeshGrid = true,
                ShowNormalVector = true
            },
            MeshSettings = new MeshVisualizationSettings
            {
                Transparency = 20,
                Extrusion = RevitApiContext.Application.VertexTolerance * 12,
                MinExtrusion = RevitApiContext.Application.VertexTolerance * 12,
                SurfaceColor = Colors.DodgerBlue,
                MeshColor = Color.FromArgb(255, 30, 81, 255),
                NormalVectorColor = Color.FromArgb(255, 255, 89, 30),
                ShowSurface = true,
                ShowMeshGrid = true,
                ShowNormalVector = true
            },
            PolylineSettings = new PolylineVisualizationSettings
            {
                Transparency = 20,
                Diameter = 2,
                MinThickness = 0.1,
                SurfaceColor = Colors.DodgerBlue,
                CurveColor = Color.FromArgb(255, 30, 81, 255),
                DirectionColor = Color.FromArgb(255, 255, 89, 30),
                ShowSurface = true,
                ShowCurve = true,
                ShowDirection = true
            },
            CurveLoopSettings = new CurveLoopVisualizationSettings
            {
                Transparency = 20,
                Diameter = 2,
                MinThickness = 0.1,
                SurfaceColor = Colors.DodgerBlue,
                CurveColor = Color.FromArgb(255, 30, 81, 255),
                DirectionColor = Color.FromArgb(255, 255, 89, 30),
                ShowSurface = true,
                ShowCurve = true,
                ShowDirection = true
            },
            SolidSettings = new SolidVisualizationSettings
            {
                Transparency = 20,
                Scale = 100,
                FaceColor = Colors.DodgerBlue,
                EdgeColor = Color.FromArgb(255, 30, 81, 255),
                ShowFace = true,
                ShowEdge = true
            },
            XyzSettings = new XyzVisualizationSettings
            {
                Transparency = 0,
                AxisLength = 6,
                MinAxisLength = 0.1,
                XColor = Color.FromArgb(255, 30, 227, 255),
                YColor = Color.FromArgb(255, 30, 144, 255),
                ZColor = Color.FromArgb(255, 30, 81, 255),
                ShowPlane = true,
                ShowXAxis = true,
                ShowYAxis = true,
                ShowZAxis = true
            }
        };
    }

    [LoggerMessage(LogLevel.Error, "Application settings loading error")]
    private static partial void LogApplicationSettingsLoadingError(ILogger<SettingsService> logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Decomposition settings loading error")]
    private static partial void LogDecompositionSettingsLoadingError(ILogger<SettingsService> logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Application settings loading error")]
    private static partial void LogVisualizationSettingsLoadingError(ILogger<SettingsService> logger, Exception exception);
}