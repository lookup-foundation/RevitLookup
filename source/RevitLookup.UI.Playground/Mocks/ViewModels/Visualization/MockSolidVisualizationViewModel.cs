using System.Windows.Media;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using RevitLookup.Abstractions.ViewModels.Visualization;

namespace RevitLookup.UI.Playground.Mocks.ViewModels.Visualization;

/// <summary>
///     Represents a Playground mock of <see cref="ISolidVisualizationViewModel" /> that fabricates its initial values with <c>Bogus</c> and no-ops the visualization server.
/// </summary>
[UsedImplicitly]
public sealed partial class MockSolidVisualizationViewModel : ObservableObject, ISolidVisualizationViewModel
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MockSolidVisualizationViewModel" /> class with fake appearance settings.
    /// </summary>
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

    /// <inheritdoc />
    [ObservableProperty]
    public partial double Scale { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial double Transparency { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial Color FaceColor { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial Color EdgeColor { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool ShowFace { get; set; }

    /// <inheritdoc />
    [ObservableProperty]
    public partial bool ShowEdge { get; set; }

    /// <inheritdoc />
    public void RegisterServer(object solid)
    {
    }

    /// <inheritdoc />
    public void UnregisterServer()
    {
    }
}
