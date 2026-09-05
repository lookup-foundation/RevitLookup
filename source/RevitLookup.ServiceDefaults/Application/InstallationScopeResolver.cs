using RevitLookup.Abstractions.Application;

namespace RevitLookup.ServiceDefaults.Application;

/// <summary>
///     Provides methods to resolve the <see cref="InstallationScope" /> an installation serves.
/// </summary>
/// <remarks>
///     The scope follows the location of the installed files alone.
///     A build output and a manually unpacked release serve the current user.
/// </remarks>
[PublicAPI]
public static class InstallationScopeResolver
{
    private static readonly Environment.SpecialFolder[] MachineWideFolders =
    [
        Environment.SpecialFolder.ProgramFiles,
        Environment.SpecialFolder.ProgramFilesX86,
        Environment.SpecialFolder.CommonApplicationData
    ];

    /// <summary>
    ///     Resolves the scope the installation at the specified path serves.
    /// </summary>
    /// <param name="path">The absolute path of an installed file or directory.</param>
    /// <returns>
    ///     <see cref="InstallationScope.PerMachine" /> when <paramref name="path" /> resides in a machine-wide folder;
    ///     otherwise <see cref="InstallationScope.PerUser" />.
    /// </returns>
    /// <remarks>An empty path serves the current user.</remarks>
    [Pure]
    public static InstallationScope Resolve(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        foreach (var folder in MachineWideFolders)
        {
            var root = Environment.GetFolderPath(folder);
            if (root.Length == 0)
            {
                continue;
            }

            if (ResidesIn(path, root))
            {
                return InstallationScope.PerMachine;
            }
        }

        return InstallationScope.PerUser;
    }

    private static bool ResidesIn(string path, string folder)
    {
        if (!path.StartsWith(folder, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (path.Length == folder.Length)
        {
            return true;
        }

        if (folder[^1] is '\\' or '/')
        {
            return true;
        }

        return path[folder.Length] is '\\' or '/';
    }
}
