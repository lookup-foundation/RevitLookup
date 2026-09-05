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

using System.Text.RegularExpressions;

namespace RevitLookup.Tests.Coverage.Discovery;

/// <summary>
///     Searches a source tree for the files mentioning a <c>Type.Member</c> name, and reads the types a file matches in a switch.
/// </summary>
/// <remarks>
///     The match is textual and case sensitive. A name written in a comment or in documentation counts as a mention.
/// </remarks>
internal sealed partial class SourceFileIndex
{
    private const string SwitchArmExpression = @"(?<type>[A-Za-z_]\w*)(?:<[^>\r\n]*>)?\s+[A-Za-z_]\w*\s*=>";

    private readonly List<(string FileName, string Content)> _files;
    private readonly Dictionary<string, HashSet<string>> _switchArmTypesByFileName;

    private SourceFileIndex(List<(string FileName, string Content)> files)
    {
        _files = files;
        _switchArmTypesByFileName = BuildSwitchArmIndex(files);
    }

    /// <summary>
    ///     Creates a <see cref="SourceFileIndex" /> reading every C# file below the directory.
    /// </summary>
    /// <param name="sourceDirectory">The source directory to index.</param>
    /// <returns>The <see cref="SourceFileIndex" /> for the files below <paramref name="sourceDirectory" />.</returns>
    /// <exception cref="DirectoryNotFoundException">The directory does not exist.</exception>
    public static SourceFileIndex Build(string sourceDirectory)
    {
        if (!Directory.Exists(sourceDirectory))
        {
            throw new DirectoryNotFoundException($"The indexed source directory does not exist: {sourceDirectory}");
        }

        var files = Directory
            .EnumerateFiles(sourceDirectory, "*.cs", SearchOption.AllDirectories)
            .Select(filePath => (FileName: Path.GetFileName(filePath), Content: File.ReadAllText(filePath)))
            .ToList();

        return new SourceFileIndex(files);
    }

    /// <summary>
    ///     Reads the names of the files containing the qualified name anywhere in their text.
    /// </summary>
    /// <param name="qualifiedName">The <c>Type.Member</c> name to look up.</param>
    /// <returns>The names of the files mentioning <paramref name="qualifiedName" />. An empty list marks a name no indexed file mentions.</returns>
    [Pure]
    public IReadOnlyList<string> FindReferencingFiles(string qualifiedName)
    {
        return
        [
            .. _files
                .Where(file => file.Content.Contains(qualifiedName, StringComparison.Ordinal))
                .Select(file => file.FileName)
        ];
    }

    /// <summary>
    ///     Reports whether the file carries a switch arm matching the type.
    /// </summary>
    /// <param name="fileName">The file name to read, without a directory.</param>
    /// <param name="typeName">The short name of the matched type, without generic arity.</param>
    /// <returns><see langword="true" /> if the file carries a switch arm matching the type; otherwise, <see langword="false" />.</returns>
    /// <remarks>
    ///     A declaration pattern names its type in code, so a comment grouping the arms contributes no name.
    ///     An expression-bodied member of the same file reads as an arm matching its return type.
    /// </remarks>
    [Pure]
    public bool HasSwitchArm(string fileName, string typeName)
    {
        return _switchArmTypesByFileName.TryGetValue(fileName, out var typeNames) && typeNames.Contains(typeName);
    }

    /// <summary>
    ///     Groups the types the switch arms match by the file declaring the arms.
    /// </summary>
    private static Dictionary<string, HashSet<string>> BuildSwitchArmIndex(List<(string FileName, string Content)> files)
    {
        var index = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

        foreach (var file in files)
        {
            if (!index.TryGetValue(file.FileName, out var typeNames))
            {
                typeNames = new HashSet<string>(StringComparer.Ordinal);
                index[file.FileName] = typeNames;
            }

            foreach (var match in SwitchArmPattern().Matches(file.Content).Cast<Match>())
            {
                typeNames.Add(match.Groups["type"].Value);
            }
        }

        return index;
    }
#if NET

    /// <summary>
    ///     Matches the declaration pattern of a switch arm, capturing the matched type.
    /// </summary>
    [GeneratedRegex(SwitchArmExpression)]
    private static partial Regex SwitchArmPattern();
#else
    private static readonly Regex CompiledSwitchArmPattern = new(SwitchArmExpression, RegexOptions.Compiled);

    /// <summary>
    ///     Matches the declaration pattern of a switch arm, capturing the matched type.
    /// </summary>
    private static Regex SwitchArmPattern()
    {
        return CompiledSwitchArmPattern;
    }
#endif
}
