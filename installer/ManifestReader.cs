using System.Text.Json;

namespace Installer;

/// <summary>
///     Provides extension methods for <see cref="FileInfo" /> to read the installer manifest.
/// </summary>
public static class ManifestReader
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <param name="file">The file holding the manifest.</param>
    extension(FileInfo file)
    {
        /// <summary>
        ///     Reads the installer manifest from the JSON document the file holds.
        /// </summary>
        /// <returns>The <see cref="Manifest" /> the file describes.</returns>
        /// <exception cref="FileNotFoundException">The file is absent.</exception>
        /// <exception cref="JsonException">The file holds malformed JSON or omits a required property.</exception>
        /// <exception cref="InvalidDataException">The file holds the JSON <c>null</c> literal.</exception>
        public Manifest ReadManifest()
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException("The installer manifest was not found", file.FullName);
            }

            using var stream = file.OpenRead();
            var manifest = JsonSerializer.Deserialize<Manifest>(stream, SerializerOptions);
            if (manifest is null)
            {
                throw new InvalidDataException($"The installer manifest holds no content: {file.FullName}");
            }

            return manifest;
        }
    }
}
