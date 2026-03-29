using System.Windows.Media;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using RevitLookup.Abstractions.ViewModels.Visualization;

namespace RevitLookup.UI.Playground.Mocks.ViewModels.Visualization;

[UsedImplicitly]
public sealed partial class MockSolidVisualizationViewModel : ObservableObject, ISolidVisualizationViewModel
{
    public MockSolidVisualizationViewModel()
    {
        var faker = new Faker();

        Transparency = faker.Random.Double(0, 100);
        Scale = faker.Random.Double(100, 400);
        FaceColor = Color.FromRgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte());
        EdgeColor = Color.FromRgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte());

        ShowFace = faker.Random.Bool();
        ShowEdge = faker.Random.Bool();
    }
    
    [ObservableProperty]
    public partial double Scale { get; set; }

    [ObservableProperty]
    public partial double Transparency { get; set; }

    [ObservableProperty]
    public partial Color FaceColor { get; set; }

    [ObservableProperty]
    public partial Color EdgeColor { get; set; }

    [ObservableProperty]
    public partial bool ShowFace { get; set; }

    [ObservableProperty]
    public partial bool ShowEdge { get; set; }

    public void RegisterServer(object solid)
    {
    }

    public void UnregisterServer()
    {
    }
}