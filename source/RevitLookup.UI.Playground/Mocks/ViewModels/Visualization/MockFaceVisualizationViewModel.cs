using System.Windows.Media;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using RevitLookup.Abstractions.ViewModels.Visualization;

namespace RevitLookup.UI.Playground.Mocks.ViewModels.Visualization;

[UsedImplicitly]
public sealed partial class MockFaceVisualizationViewModel : ObservableObject, IFaceVisualizationViewModel
{
    public MockFaceVisualizationViewModel()
    {
        var faker = new Faker();

        MinExtrusion = 0;
        Transparency = faker.Random.Double(0, 100);
        Extrusion = faker.Random.Double(0, 24);
        SurfaceColor = Color.FromRgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte());
        MeshColor = Color.FromRgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte());
        NormalVectorColor = Color.FromRgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte());

        ShowSurface = faker.Random.Bool();
        ShowMeshGrid = faker.Random.Bool();
        ShowNormalVector = faker.Random.Bool();
    }
    
    [ObservableProperty]
    public partial double Extrusion { get; set; }

    [ObservableProperty]
    public partial double Transparency { get; set; }

    [ObservableProperty]
    public partial Color SurfaceColor { get; set; }

    [ObservableProperty]
    public partial Color MeshColor { get; set; }

    [ObservableProperty]
    public partial Color NormalVectorColor { get; set; }

    [ObservableProperty]
    public partial bool ShowSurface { get; set; }

    [ObservableProperty]
    public partial bool ShowMeshGrid { get; set; }

    [ObservableProperty]
    public partial bool ShowNormalVector { get; set; }
    
    public double MinExtrusion { get; }

    public void RegisterServer(object face)
    {
    }

    public void UnregisterServer()
    {
    }
}