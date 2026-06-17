using System.Windows.Media;
using Bogus;

namespace RevitLookup.UI.Playground.Mocks.Models;

/// <summary>
///     A single object that brings together every value kind and member kind the grid can render,
///     so one window can exercise all cell templates, row styles, and decomposition options at once.
///     Synthetic computed, deferred, disabled, and unsupported members are added by <c>MixedSampleDescriptor</c>.
/// </summary>
[PublicAPI]
public sealed class MixedSample
{
    private readonly string _checksum;

    public MixedSample()
    {
        var faker = new Faker();
        Name = faker.Commerce.ProductName();
        Description = faker.Lorem.Sentence();
        Tint = Color.FromArgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte());
        Revision = faker.Random.Int(1, 10);
        Author = new PlaceholderSample();
        _checksum = faker.Random.Guid().ToString();

        var swatches = new List<Color>(8);
        for (var i = 0; i < swatches.Capacity; i++)
        {
            swatches.Add(Color.FromArgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte()));
        }

        Swatches = swatches;
    }

    public string Name { get; }
    public string Description { get; }
    public Color Tint { get; }
    public string Comment => string.Empty;
    public string? Note => null;
    public Uri? Reference => null;
    public string Diagnostics => throw new InvalidOperationException("The sample element failed during evaluation");
    public IReadOnlyList<Color> Swatches { get; }
    public PlaceholderSample Author { get; }

    public static string DefaultCategory { get; } = "Samples";
    public int Revision;
    private string Checksum => _checksum;

    public event EventHandler? Changed;

    public string Refresh()
    {
        Changed?.Invoke(this, EventArgs.Empty);
        return $"Revision {Revision} ({Checksum})";
    }

    public void Delete()
    {
    }
}