using RevitLookup.Abstractions.Application;

namespace RevitLookup.ServiceDefaults.Application;

/// <summary>
///     Provides methods to resolve the <see cref="InstallationScope" /> an installation serves.
/// </summary>
/// <remarks>
///     The per-machine installer targets <c>%ProgramFiles%</c> for Revit 2027 and newer, and <c>%ProgramData%</c> for earlier versions.
///     The per-user installer targets <c>%AppData%</c>, and a build output or a manually unpacked release occupies an arbitrary folder.
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
    /// <remarks>The resolution reads no file system entry and throws no exception.</remarks>
    [Pure]
    public static InstallationScope Resolve(string path)
    {
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
