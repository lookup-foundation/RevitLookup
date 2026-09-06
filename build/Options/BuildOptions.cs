namespace Build.Options;

/// <summary>
///     Represents the build settings used to compile and package the add-in.
/// </summary>
[PublicAPI]
public sealed record BuildOptions
{
    /// <summary>
    ///     Gets the application versions keyed by compile configuration.
    /// </summary>
    /// <example>
    ///     1.0.0-alpha.1.250101 <br />
    ///     1.0.0-beta.2.250101 <br />
    ///     1.0.0
    /// </example>
    public Dictionary<string, string> Versions { get; init; } = [];

    /// <summary>
    ///     Gets the path to the build output directory.
    /// </summary>
    public string OutputDirectory { get; init; } = "output";
}
