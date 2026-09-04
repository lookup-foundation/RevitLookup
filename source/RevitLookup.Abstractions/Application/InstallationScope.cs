namespace RevitLookup.Abstractions.Application;

/// <summary>
///     Determines the set of users an installation serves.
/// </summary>
public enum InstallationScope
{
    /// <summary>
    ///     The installation serves the current user.
    /// </summary>
    PerUser,

    /// <summary>
    ///     The installation serves every user of the machine.
    /// </summary>
    PerMachine
}
