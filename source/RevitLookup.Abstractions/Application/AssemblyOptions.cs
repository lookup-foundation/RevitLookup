namespace RevitLookup.Abstractions.Application;

/// <summary>
///     Represents runtime information about the application assembly.
/// </summary>
public sealed class AssemblyOptions
{
    /// <summary>
    ///     Gets or sets the display name of the target framework the assembly runs on.
    /// </summary>
    public required string Framework { get; set; }

    /// <summary>
    ///     Gets or sets the version of the running assembly.
    /// </summary>
    public required Version Version { get; set; }

    /// <summary>
    ///     Gets or sets the set of users the current installation serves.
    /// </summary>
    public required InstallationScope InstallationScope { get; set; }
}
