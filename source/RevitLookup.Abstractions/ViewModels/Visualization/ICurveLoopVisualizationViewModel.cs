using System.Windows.Media;

namespace RevitLookup.Abstractions.ViewModels.Visualization;

public interface ICurveLoopVisualizationViewModel
{
    double MinThickness { get; }
    double Diameter { get; set; }
    double Transparency { get; set; }
    Color SurfaceColor { get; set; }
    Color CurveColor { get; set; }
    Color DirectionColor { get; set; }
    bool ShowSurface { get; set; }
    bool ShowCurve { get; set; }
    bool ShowDirection { get; set; }

    void RegisterServer(object curveLoop);
    void UnregisterServer();
}