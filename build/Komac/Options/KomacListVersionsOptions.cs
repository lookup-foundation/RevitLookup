using ModularPipelines.Attributes;
using ModularPipelines.Options;

namespace Build.Komac.Options;

[PublicAPI]
[Serializable]
[CliSubCommand("list-versions")]
public sealed record KomacListVersionsOptions : CommandLineToolOptions
{
    [CliArgument(0, Placement = ArgumentPlacement.ImmediatelyAfterCommand)]
    public required string PackageIdentifier { get; set; }

    [CliOption("--token")]
    public string? Token { get; set; }
}
