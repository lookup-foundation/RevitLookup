using Bogus;

namespace RevitLookup.UI.Playground.Mocks.Models;

/// <summary>
///     Demonstrates deferred members. Cheap metadata is evaluated eagerly, while the expensive
///     operations are deferred by <c>DeferredSampleDescriptor</c> and shown with a "Force evaluate" button.
/// </summary>
[PublicAPI]
public sealed class DeferredSample
{
    public DeferredSample()
    {
        var faker = new Faker();
        Title = $"{faker.Commerce.ProductName()} Report";
        Owner = faker.Name.FullName();
        GeneratedOn = faker.Date.Recent();
        RecordCount = faker.Random.Int(1_000, 1_000_000);
    }

    public string Title { get; }
    public string Owner { get; }
    public DateTime GeneratedOn { get; }
    public int RecordCount { get; }

    public string CalculateTotals()
    {
        return $"Aggregated {RecordCount} records";
    }

    public string BuildChart()
    {
        return "Chart rendered";
    }

    public string ExportToPdf()
    {
        return "report.pdf";
    }
}