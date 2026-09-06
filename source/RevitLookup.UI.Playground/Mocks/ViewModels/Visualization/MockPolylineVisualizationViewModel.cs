using System.Windows.Media;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using RevitLookup.Abstractions.ViewModels.Visualization;

namespace RevitLookup.UI.Playground.Mocks.ViewModels.Visualization;

/// <summary>
///     Represents a Playground mock of <see cref="IPolylineVisualizationViewModel" /> that fabricates its initial values with <c>Bogus</c> and no-ops the visualization server.
/// </summary>
[UsedImplicitly]
public sealed partial class MockPolylineVisualizationViewModel : ObservableObject, IPolylineVisualizationViewModel
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MockPolylineVisualizationViewModel" /> class with fake appearance settings.
    /// </summary>
    public MockPolylineVisualizationViewModel()
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

    /// <inheritdoc />
    [ObservableProperty]
    public partial double Diameter { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial double Transparency { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial Color SurfaceColor { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial Color CurveColor { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial Color DirectionColor { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool ShowSurface { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool ShowCurve { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool ShowDirection { get; set; }

    /// <inheritdoc />
    public double MinThickness { get; }

    /// <inheritdoc />
    public void RegisterServer(object curveOrEdge)
    {
    }

    /// <inheritdoc />
    public void UnregisterServer()
    {
    }
}
