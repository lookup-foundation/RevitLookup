using System.IO;
using System.Text.Json;
using System.Windows.Media;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RevitLookup.Abstractions.Application;
using RevitLookup.Abstractions.Settings;
using Wpf.Ui.Animations;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Color = System.Windows.Media.Color;

namespace RevitLookup.Settings;

/// <summary>
///     Manages the application, LookupEngine, and visualization settings, persisting them to the user's settings folder.
/// </summary>
/// <param name="foldersOptions">The options that resolve the settings file paths.</param>
/// <param name="jsonOptions">The options used to serialize and deserialize the settings files.</param>
/// <param name="logger">The logger this service writes diagnostic records to.</param>
public sealed partial class SettingsService(
    IOptions<ResourceLocationsOptions> foldersOptions,
    IOptions<JsonSerializerOptions> jsonOptions,
    ILogger<SettingsService> logger)
    : ISettingsService
{
    private ApplicationSettings? _applicationSettings;
    private DecompositionSettings? _decompositionSettings;
    private VisualizationSettings? _visualizationSettings;

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">The settings have not been loaded with <see cref="LoadSettings" />.</exception>
    public ApplicationSettings ApplicationSettings => _applicationSettings ?? throw new InvalidOperationException("Application settings is not loaded.");

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">The settings have not been loaded with <see cref="LoadSettings" />.</exception>
    public DecompositionSettings DecompositionSettings => _decompositionSettings ?? throw new InvalidOperationException("Decomposition settings is not loaded.");

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">The settings have not been loaded with <see cref="LoadSettings" />.</exception>
    public VisualizationSettings VisualizationSettings => _visualizationSettings ?? throw new InvalidOperationException("Visualization settings is not loaded.");

    /// <inheritdoc />
    public void SaveSettings()
    {
        SaveApplicationSettings();
        SaveDecompositionSettings();
        SaveVisualizationSettings();
    }

    /// <inheritdoc />
    public void LoadSettings()
    {
        LoadApplicationSettings();
        LoadDecompositionSettings();
        LoadVisualizationSettings();
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public void ResetDecompositionSettings()
    {
        _decompositionSettings = new DecompositionSettings
        {
            IncludeStatic = true,
            IncludeEvents = true,
            IncludeExtensions = true
        };
    }

    /// <inheritdoc />
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

    private void SaveApplicationSettings()
    {
        var path = foldersOptions.Value.ApplicationSettingsPath;
        if (!File.Exists(path))
        {
            Directory.CreateDirectory(foldersOptions.Value.SettingsDirectory);
        }

        var json = JsonSerializer.Serialize(_applicationSettings, jsonOptions.Value);
        File.WriteAllText(path, json);
    }

    private void SaveDecompositionSettings()
    {
        var path = foldersOptions.Value.DecompositionSettingsPath;
        if (!File.Exists(path))
        {
            Directory.CreateDirectory(foldersOptions.Value.SettingsDirectory);
        }

        var json = JsonSerializer.Serialize(_decompositionSettings, jsonOptions.Value);
        File.WriteAllText(path, json);
    }

    private void SaveVisualizationSettings()
    {
        var path = foldersOptions.Value.VisualizationSettingsPath;
        if (!File.Exists(path))
        {
            Directory.CreateDirectory(foldersOptions.Value.SettingsDirectory);
        }

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

    [LoggerMessage(LogLevel.Error, "Application settings loading error")]
    private static partial void LogApplicationSettingsLoadingError(ILogger<SettingsService> logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Decomposition settings loading error")]
    private static partial void LogDecompositionSettingsLoadingError(ILogger<SettingsService> logger, Exception exception);

    [LoggerMessage(LogLevel.Error, "Application settings loading error")]
    private static partial void LogVisualizationSettingsLoadingError(ILogger<SettingsService> logger, Exception exception);
}
