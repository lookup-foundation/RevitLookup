using System.Windows.Media;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using RevitLookup.Abstractions.ViewModels.Visualization;

namespace RevitLookup.UI.Playground.Mocks.ViewModels.Visualization;

/// <summary>
///     Represents a Playground mock of <see cref="IXyzVisualizationViewModel" /> that fabricates its initial values with <c>Bogus</c> and no-ops the visualization server.
/// </summary>
[UsedImplicitly]
public sealed partial class MockXyzVisualizationViewModel : ObservableObject, IXyzVisualizationViewModel
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MockXyzVisualizationViewModel" /> class with fake appearance settings.
    /// </summary>
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

    /// <inheritdoc />
    [ObservableProperty]
    public partial double AxisLength { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial double Transparency { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial Color XColor { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial Color YColor { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial Color ZColor { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool ShowPlane { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool ShowXAxis { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool ShowYAxis { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool ShowZAxis { get; set; }

    /// <inheritdoc />
    public double MinAxisLength { get; }

    /// <inheritdoc />
    public void RegisterServer(object xyz)
    {
    }

    /// <inheritdoc />
    public void UnregisterServer()
    {
    }
}
