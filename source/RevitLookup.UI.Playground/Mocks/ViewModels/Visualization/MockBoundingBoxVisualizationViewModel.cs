using System.Windows.Media;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using RevitLookup.Abstractions.ViewModels.Visualization;

namespace RevitLookup.UI.Playground.Mocks.ViewModels.Visualization;

/// <summary>
///     Represents a Playground mock of <see cref="IBoundingBoxVisualizationViewModel" /> that fabricates its initial values with <c>Bogus</c> and no-ops the visualization server.
/// </summary>
[UsedImplicitly]
public sealed partial class MockBoundingBoxVisualizationViewModel : ObservableObject, IBoundingBoxVisualizationViewModel
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MockBoundingBoxVisualizationViewModel" /> class with fake appearance settings.
    /// </summary>
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

    /// <inheritdoc />
    [ObservableProperty]
    public partial double Transparency { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial Color SurfaceColor { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial Color EdgeColor { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial Color AxisColor { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool ShowSurface { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool ShowEdge { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool ShowAxis { get; set; }

    /// <inheritdoc />
    public void RegisterServer(object boundingBoxXyz)
    {
    }

    /// <inheritdoc />
    public void UnregisterServer()
    {
    }
}
