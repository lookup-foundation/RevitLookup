namespace Installer;

/// <summary>
///     Represents the content and identity of the installer packages.
/// </summary>
/// <remarks>
///     The manifest carries what changes from release to release.
///     The presentation, the target platform, and the directory layout are fixed by the installer.
/// </remarks>
[PublicAPI]
public sealed record Manifest
{
    /// <summary>
    ///     Gets the name of the product the packages install.
    /// </summary>
    public required string ProductName { get; init; }

    /// <summary>
    ///     Gets the version Windows Installer compares against the installed package when it resolves an upgrade.
    /// </summary>
    /// <remarks>
    ///     The comparison covers the major, minor, and build components; the revision component takes no part in it.
    ///     The <see href="https://learn.microsoft.com/windows/win32/msi/productversion">ProductVersion</see> property of the MSI database carries the value.
    /// </remarks>
    public required Version ProductVersion { get; init; }

    /// <summary>
    ///     Gets the identity shared by every release of the product.
    /// </summary>
    /// <remarks>
    ///     A release published under the installed value upgrades it in place.
    ///     A release published under a new value installs alongside its predecessor.
    /// </remarks>
    public required Guid UpgradeCode { get; init; }

    /// <summary>
    ///     Gets the version the release is published under.
    /// </summary>
    /// <remarks>The file name of every produced package includes this value.</remarks>
    /// <example>
    ///     2026.1.3-alpha.1.250101 <br />
    ///     2026.1.3-beta.2.250101 <br />
    ///     2026.1.3
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
        /// <remarks>The packages install the sets in the order their roles first appear in the manifest.</remarks>
        public required IReadOnlyList<FileSet> Files { get; init; }
    }

    /// <summary>
    ///     Represents a set of files selected from a source directory.
    /// </summary>
    [PublicAPI]
    public sealed record FileSet
    {
        /// <summary>
        ///     Gets the name of the installation stage the file set belongs to.
        /// </summary>
        /// <remarks>
        ///     File sets sharing a role install together, and every role installs after the roles declared before it.
        ///     A file a running application picks up belongs in a role declared after the roles holding the files it depends on.
        /// </remarks>
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
