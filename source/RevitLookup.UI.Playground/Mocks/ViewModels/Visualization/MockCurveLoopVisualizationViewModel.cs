using System.Windows.Media;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using RevitLookup.Abstractions.ViewModels.Visualization;

namespace RevitLookup.UI.Playground.Mocks.ViewModels.Visualization;

[UsedImplicitly]
public sealed partial class MockCurveLoopVisualizationViewModel : ObservableObject, ICurveLoopVisualizationViewModel
{
    public MockCurveLoopVisualizationViewModel()
    {
        var faker = new Faker();

        MinThickness = 0;
        Transparency = faker.Random.Double(0, 100);
        Diameter = faker.Random.Double(0, 6);
        SurfaceColor = Color.FromRgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte());
        CurveColor = Color.FromRgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte());
        DirectionColor = Color.FromRgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte());

        ShowSurface = faker.Random.Bool();
        ShowCurve = faker.Random.Bool();
        ShowDirection = faker.Random.Bool();
    }
    
    [ObservableProperty]
    public partial double Diameter { get; set; }

    [ObservableProperty]
    public partial double Transparency { get; set; }

    [ObservableProperty]
    public partial Color SurfaceColor { get; set; }

    [ObservableProperty]
    public partial Color CurveColor { get; set; }

    [ObservableProperty]
    public partial Color DirectionColor { get; set; }

    [ObservableProperty]
    public partial bool ShowSurface { get; set; }

    [ObservableProperty]
    public partial bool ShowCurve { get; set; }

    [ObservableProperty]
    public partial bool ShowDirection { get; set; }

    public double MinThickness { get; }

    public void RegisterServer(object curveOrEdge)
    {
    }

    public void UnregisterServer()
    {
    }
}