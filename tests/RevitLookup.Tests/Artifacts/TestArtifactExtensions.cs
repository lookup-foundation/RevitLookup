// Copyright (c) Lookup Foundation and Contributors
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// THIS PROGRAM IS PROVIDED "AS IS" AND WITH ALL FAULTS.
// NO IMPLIED WARRANTY OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE IS PROVIDED.
// THERE IS NO GUARANTEE THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.

namespace RevitLookup.Tests.Artifacts;

/// <summary>
///     Provides extension methods for <see cref="string" /> to attach it to test output as a file artifact.
/// </summary>
public static class TestArtifactExtensions
{
    /// <param name="content">The artifact content.</param>
    extension(string content)
    {
        /// <summary>
        ///     Writes the content to a temporary file and attaches it to the running test.
        /// </summary>
        /// <param name="name">The artifact name.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task CreateArtifactAsync(string name)
        {
            await CreateArtifactEntryAsync(name, content, null);
        }

        /// <summary>
        ///     Writes the content to a temporary Markdown file and attaches it to the running test.
        /// </summary>
        /// <param name="name">The artifact name.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task CreateMarkdownArtifactAsync(string name)
        {
            await CreateArtifactEntryAsync(name, content, ".md");
        }
    }

    /// <summary>
    ///     Writes the content to a file in the temporary directory and attaches it to the running test.
    /// </summary>
    /// <param name="name">The artifact name.</param>
    /// <param name="content">The artifact content.</param>
    /// <param name="extension">The file extension to give the temporary file, or <see langword="null" /> to keep the generated one.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="InvalidOperationException">The call happens outside a running test.</exception>
    private static async Task CreateArtifactEntryAsync(string name, string content, string? extension)
    {
        var context = TestContext.Current ?? throw new InvalidOperationException($"The '{name}' artifact cannot be attached outside a running test.");

        var fileName = Path.GetRandomFileName();
        if (extension is not null)
        {
            fileName = Path.ChangeExtension(fileName, extension);
        }

        var artifactPath = Path.Combine(Path.GetTempPath(), fileName);
        await File.WriteAllTextAsync(artifactPath, content, context.Execution.CancellationToken);

        context.Output.WriteLine($"Artifact: {artifactPath}");
        context.Output.AttachArtifact(new Artifact
        {
            File = new FileInfo(artifactPath),
            DisplayName = name
        });
    }
}
