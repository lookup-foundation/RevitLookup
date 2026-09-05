using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using WixSharp;

namespace Installer.Layouts;

/// <summary>
///     Provides extension methods for a list of <see cref="Manifest.AddinContent" /> to lay the installation out as one feature per Revit version.
/// </summary>
public static class FeatureLayout
{
    /// <param name="content">The add-in content to lay out.</param>
    extension(IReadOnlyList<Manifest.AddinContent> content)
    {
        /// <summary>
        ///     Creates the <see cref="Dir" /> tree installing the add-in of every Revit version.
        /// </summary>
        /// <param name="contentRoot">The directory the content base paths are resolved against.</param>
        /// <param name="scope">The <see cref="InstallScope" /> the packages install under.</param>
        /// <returns>The <see cref="Dir" /> array the packages install.</returns>
        /// <exception cref="DirectoryNotFoundException">A file set points to a directory that is absent.</exception>
        /// <exception cref="InvalidDataException">A file set selects no files.</exception>
        /// <remarks>One entry is an <see cref="InstallDir" /> holding the path the user can change.</remarks>
        public Dir[] CreateFeatureLayout(DirectoryInfo contentRoot, InstallScope scope)
        {
            var revitFeature = new Feature
            {
                Name = "Revit Add-in",
                Description = "Revit add-in installation files",
                Display = FeatureDisplay.expand
            };

            var directories = new List<Dir>();
            foreach (var addinsRoot in content.GroupBy(addin => ResolveAddinsRoot(addin.RevitVersion, scope)))
            {
                var versionDirectories = addinsRoot
                    .Select(addin => CreateVersionDirectory(addin, revitFeature, contentRoot))
                    .Cast<WixEntity>()
                    .ToArray();

                directories.Add(directories.Count is 0
                    ? new InstallDir(addinsRoot.Key, versionDirectories)
                    : new Dir(addinsRoot.Key, versionDirectories));
            }

            return [.. directories];
        }
    }

    /// <summary>
    ///     Creates the directory holding the add-in of a single Revit version.
    /// </summary>
    private static Dir CreateVersionDirectory(Manifest.AddinContent addin, Feature revitFeature, DirectoryInfo contentRoot)
    {
        var fileVersion = addin.RevitVersion.ToString();
        var feature = new Feature
        {
            Name = fileVersion,
            Description = $"Install add-in for Revit {fileVersion}",
            ConfigurableDir = $"INSTALL{fileVersion}"
        };

        revitFeature.Add(feature);

        var fileSets = addin.Files
            .Select(fileSet => CreateFiles(fileSet, feature, contentRoot))
            .Cast<WixEntity>()
            .ToArray();

        return new Dir(new Id($"INSTALL{fileVersion}"), fileVersion, fileSets);
    }

    /// <summary>
    ///     Selects the files of a single file set and assigns them to the specified feature.
    /// </summary>
    private static Files CreateFiles(Manifest.FileSet fileSet, Feature feature, DirectoryInfo contentRoot)
    {
        var basePath = Path.GetFullPath(Path.Combine(contentRoot.FullName, fileSet.BasePath));
        if (!Directory.Exists(basePath))
        {
            throw new DirectoryNotFoundException($"The {fileSet.Role} file set points to a missing directory: {basePath}");
        }

        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
        matcher.AddIncludePatterns(fileSet.Include);
        matcher.AddExcludePatterns(fileSet.Exclude);

        var matchingResult = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(basePath)));
        if (!matchingResult.HasMatches)
        {
            throw new InvalidDataException($"The {fileSet.Role} file set selects no files under: {basePath}");
        }

        var selectedFiles = matchingResult.Files
            .Select(match => Path.GetFullPath(Path.Combine(basePath, match.Path)))
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        LogSelectedFiles(fileSet, selectedFiles);

        var selectedPaths = selectedFiles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return new Files(feature, Path.Combine(basePath, "*.*"), path => selectedPaths.Contains(Path.GetFullPath(path)));
    }

    /// <summary>
    ///     Resolves the Revit add-ins directory the content is installed under.
    /// </summary>
    private static string ResolveAddinsRoot(int revitVersion, InstallScope scope)
    {
        if (scope is InstallScope.perUser)
        {
            return @"%AppDataFolder%\Autodesk\Revit\Addins";
        }

        return revitVersion switch
        {
            >= 2027 => @"%ProgramFiles%\Autodesk\Revit\Addins",
            _ => @"%CommonAppDataFolder%\Autodesk\Revit\Addins"
        };
    }

    /// <summary>
    ///     Writes the files of a single file set to the build log.
    /// </summary>
    private static void LogSelectedFiles(Manifest.FileSet fileSet, string[] selectedFiles)
    {
        Console.WriteLine($"{fileSet.Role} files for Revit add-in ({selectedFiles.Length}):");

        foreach (var selectedFile in selectedFiles)
        {
            Console.WriteLine($"- {selectedFile}");
        }
    }
}
