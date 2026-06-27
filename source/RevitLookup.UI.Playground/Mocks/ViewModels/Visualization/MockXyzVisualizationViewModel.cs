using System.Windows.Media;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using RevitLookup.Abstractions.ViewModels.Visualization;

namespace RevitLookup.UI.Playground.Mocks.ViewModels.Visualization;

[UsedImplicitly]
public sealed partial class MockXyzVisualizationViewModel : ObservableObject, IXyzVisualizationViewModel
{
    public MockXyzVisualizationViewModel()
    {
        var faker = new Faker();

        MinAxisLength = 0;
        Transparency = faker.Random.Double(0, 100);
        AxisLength = faker.Random.Double(0, 24);
        XColor = Color.FromRgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte());
        YColor = Color.FromRgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte());
        ZColor = Color.FromRgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte());

        ShowPlane = faker.Random.Bool();
        ShowXAxis = faker.Random.Bool();
        ShowYAxis = faker.Random.Bool();
        ShowZAxis = faker.Random.Bool();
    }
    
    [ObservableProperty]
    public partial double AxisLength { get; set; }

    [ObservableProperty]
    public partial double Transparency { get; set; }

    [ObservableProperty]
    public partial Color XColor { get; set; }

    [ObservableProperty]
    public partial Color YColor { get; set; }

    [ObservableProperty]
    public partial Color ZColor { get; set; }

    [ObservableProperty]
    public partial bool ShowPlane { get; set; }

    [ObservableProperty]
    public partial bool ShowXAxis { get; set; }

    [ObservableProperty]
    public partial bool ShowYAxis { get; set; }

    [ObservableProperty]
    public partial bool ShowZAxis { get; set; }

    public double MinAxisLength { get; }

    public void RegisterServer(object xyz)
    {
    }

    public void UnregisterServer()
    {
    }
}