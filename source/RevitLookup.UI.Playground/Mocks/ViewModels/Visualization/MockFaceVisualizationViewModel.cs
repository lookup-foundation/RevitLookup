using System.Windows.Media;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using RevitLookup.Abstractions.ViewModels.Visualization;

namespace RevitLookup.UI.Playground.Mocks.ViewModels.Visualization;

/// <summary>
///     Represents a Playground mock of <see cref="IFaceVisualizationViewModel" /> that fabricates its initial values with <c>Bogus</c> and no-ops the visualization server.
/// </summary>
[UsedImplicitly]
public sealed partial class MockFaceVisualizationViewModel : ObservableObject, IFaceVisualizationViewModel
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MockFaceVisualizationViewModel" /> class with fake appearance settings.
    /// </summary>
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

    /// <inheritdoc />
    [ObservableProperty]
    public partial double Extrusion { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial double Transparency { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial Color SurfaceColor { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial Color MeshColor { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial Color NormalVectorColor { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool ShowSurface { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool ShowMeshGrid { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool ShowNormalVector { get; set; }

    /// <inheritdoc />
    public double MinExtrusion { get; }

    /// <inheritdoc />
    public void RegisterServer(object face)
    {
    }

    /// <inheritdoc />
    public void UnregisterServer()
    {
    }
}
