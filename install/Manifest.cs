namespace Installer;

/// <summary>
///     Represents the content and identity of the installer packages.
/// </summary>
/// <remarks>
///     The manifest carries what changes from release to release.
///     The presentation, the target platform, and the directory layout are fixed by the installer.
/// </remarks>
[PublicAPI]
[Serializable]
public sealed record Manifest
{
    /// <summary>
    ///     Gets the name of the product the packages install.
    /// </summary>
    public required string ProductName { get; init; }

    /// <summary>
    ///     Gets the identity shared by every release of the product.
    /// </summary>
    /// <remarks>
    ///     A release published under the installed value upgrades it in place.
    ///     A release published under a new value installs alongside its predecessor.
    /// </remarks>
    public required Guid UpgradeCode { get; init; }

    /// <summary>
    ///     Gets the version written into the MSI database as the <see href="https://learn.microsoft.com/windows/win32/msi/productversion">ProductVersion</see> property.
    /// </summary>
    /// <remarks>Windows Installer compares this value against the installed package when it resolves an upgrade.</remarks>
    public required Version PackageVersion { get; init; }

    /// <summary>
    ///     Gets the version the release is published under.
    /// </summary>
    /// <remarks>The file name of every produced package includes this value.</remarks>
    /// <example>
    ///     2026.1.3 <br />
    ///     2026.1.3-beta.2.250101
    /// </example>
    public required string ReleaseVersion { get; init; }

    /// <summary>
    ///     Gets the absolute path of the directory the packages are written to.
    /// </summary>
    public required string OutputDirectory { get; init; }

    /// <summary>
    ///     Gets the add-in content the packages install.
    /// </summary>
    public required IReadOnlyList<AddinContent> Content { get; init; }

    /// <summary>
    ///     Represents the add-in files installed for a single Revit version.
    /// </summary>
    [PublicAPI]
    [Serializable]
    public sealed record AddinContent
    {
        /// <summary>
        ///     Gets the Revit version the files target.
        /// </summary>
        /// <value>The four-digit Revit release year.</value>
        public required int RevitVersion { get; init; }

        /// <summary>
        ///     Gets the file sets installed for the Revit version.
        /// </summary>
        public required IReadOnlyList<FileSet> Files { get; init; }
    }

    /// <summary>
    ///     Represents a set of files selected from a source directory.
    /// </summary>
    [PublicAPI]
    [Serializable]
    public sealed record FileSet
    {
        /// <summary>
        ///     Gets the name identifying the file set in the build output.
        /// </summary>
        public required string Role { get; init; }

        /// <summary>
        ///     Gets the source directory the patterns are matched against.
        /// </summary>
        /// <value>A path relative to the directory holding the manifest file.</value>
        public required string BasePath { get; init; }

        /// <summary>
        ///     Gets the glob patterns selecting the files.
        /// </summary>
        /// <remarks>The patterns follow the <see href="https://learn.microsoft.com/dotnet/core/extensions/file-globbing">.NET file globbing</see> format.</remarks>
        public required IReadOnlyList<string> Include { get; init; }

        /// <summary>
        ///     Gets the glob patterns excluding files from the selection.
        /// </summary>
        /// <value>Defaults to an empty list.</value>
        /// <remarks>The patterns follow the <see href="https://learn.microsoft.com/dotnet/core/extensions/file-globbing">.NET file globbing</see> format.</remarks>
        public IReadOnlyList<string> Exclude { get; init; } = [];
    }
}
