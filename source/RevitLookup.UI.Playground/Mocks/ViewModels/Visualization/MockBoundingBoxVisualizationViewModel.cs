using System.Windows.Media;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using RevitLookup.Abstractions.ViewModels.Visualization;

namespace RevitLookup.UI.Playground.Mocks.ViewModels.Visualization;

[UsedImplicitly]
public sealed partial class MockBoundingBoxVisualizationViewModel : ObservableObject, IBoundingBoxVisualizationViewModel
{
    public MockBoundingBoxVisualizationViewModel()
    {
        var faker = new Faker();

        Transparency = faker.Random.Double(0, 100);
        SurfaceColor = Color.FromRgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte());
        EdgeColor = Color.FromRgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte());
        AxisColor = Color.FromRgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte());

        ShowSurface = faker.Random.Bool();
        ShowEdge = faker.Random.Bool();
        ShowAxis = faker.Random.Bool();
    }
    
    [ObservableProperty]
    public partial double Transparency { get; set; }

    [ObservableProperty]
    public partial Color SurfaceColor { get; set; }

    [ObservableProperty]
    public partial Color EdgeColor { get; set; }

    [ObservableProperty]
    public partial Color AxisColor { get; set; }

    [ObservableProperty]
    public partial bool ShowSurface { get; set; }

    [ObservableProperty]
    public partial bool ShowEdge { get; set; }

    [ObservableProperty]
    public partial bool ShowAxis { get; set; }

    public void RegisterServer(object boundingBoxXyz)
    {
    }

    public void UnregisterServer()
    {
    }
}