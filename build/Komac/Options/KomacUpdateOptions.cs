using ModularPipelines.Attributes;
using ModularPipelines.Options;

namespace Build.Komac.Options;

[PublicAPI]
[Serializable]
[CliSubCommand("update")]
public sealed record KomacUpdateOptions : CommandLineToolOptions
{
    [CliArgument(0, Placement = ArgumentPlacement.ImmediatelyAfterCommand)]
    public required string PackageIdentifier { get; set; }

    [CliOption("--version")]
    public string? Version { get; set; }

    [CliOption("--urls", AllowMultiple = true)]
    public IEnumerable<string>? Urls { get; set; }

    [CliOption("--token")]
    public string? Token { get; set; }

    [CliFlag("--submit")]
    public bool? Submit { get; set; }

    [CliFlag("--dry-run")]
    public bool? DryRun { get; set; }

    [CliOption("--created-with")]
    public string? CreatedWith { get; set; }

    [CliOption("--created-with-url")]
    public string? CreatedWithUrl { get; set; }
}
