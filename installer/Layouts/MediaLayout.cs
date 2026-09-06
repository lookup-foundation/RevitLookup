using System.Xml.Linq;

namespace Installer.Layouts;

/// <summary>
///     Represents the cabinets the packages split their content into, one per file set role.
/// </summary>
/// <remarks>
///     Windows Installer copies the content of a cabinet before the content of the cabinets that follow it.
///     The order the roles appear in the manifest is the order the packages install them in.
/// </remarks>
public sealed class MediaLayout
{
    private readonly string[] _cabinets;
    private readonly string[] _roles;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MediaLayout" /> class.
    /// </summary>
    /// <param name="content">The add-in content the cabinets are resolved from.</param>
    public MediaLayout(IReadOnlyList<Manifest.AddinContent> content)
    {
        _roles =
        [
            .. content
                .SelectMany(addin => addin.Files)
                .Select(fileSet => fileSet.Role)
                .Distinct(StringComparer.OrdinalIgnoreCase)
        ];

        _cabinets = [.. _roles.Select(role => $"{role}.cab")];
    }

    /// <summary>
    ///     Resolves the cabinet the specified file set role is packaged into.
    /// </summary>
    /// <param name="role">The role of the file set.</param>
    /// <returns>The number of the cabinet, counted from one.</returns>
    /// <exception cref="InvalidDataException">The role belongs to no cabinet of the layout.</exception>
    public int ResolveDiskId(string role)
    {
        var index = Array.FindIndex(_roles, candidate => string.Equals(candidate, role, StringComparison.OrdinalIgnoreCase));
        if (index < 0)
        {
            throw new InvalidDataException($"The file set role belongs to no cabinet: {role}");
        }

        return index + 1;
    }

    /// <summary>
    ///     Writes the cabinets into the WiX source the project generates.
    /// </summary>
    /// <param name="document">The generated WiX source document.</param>
    /// <exception cref="InvalidDataException">The document holds no package.</exception>
    /// <remarks>The project models a single medium and offers no model for the rest; the source carries them instead.</remarks>
    public void WriteToWixSource(XDocument document)
    {
        var package = document.Descendants().FirstOrDefault(element => element.Name.LocalName == "Package");
        if (package is null)
        {
            throw new InvalidDataException("The generated WiX source holds no package");
        }

        var wixNamespace = package.Name.Namespace;
        var generatedMedia = package.Elements(wixNamespace + "Media").ToList();
        var replacement = _cabinets
            .Select((cabinet, index) => new XElement(wixNamespace + "Media",
                new XAttribute("Id", index + 1),
                new XAttribute("Cabinet", cabinet),
                new XAttribute("EmbedCab", "yes")))
            .ToList();

        var anchor = generatedMedia.LastOrDefault() ?? package.Elements(wixNamespace + "SummaryInformation").LastOrDefault();
        if (anchor is null)
        {
            package.AddFirst(replacement);
        }
        else
        {
            anchor.AddAfterSelf(replacement);
        }

        foreach (var element in generatedMedia)
        {
            element.Remove();
        }
    }
}
