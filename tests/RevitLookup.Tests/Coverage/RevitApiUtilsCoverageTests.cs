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

using RevitLookup.Tests.Abstractions;

namespace RevitLookup.Tests.Coverage;

/// <summary>
///     Reports the Revit API utility methods and the descriptor files resolving them.
/// </summary>
public sealed class RevitApiUtilsCoverageTests : RevitApiReportTest
{
    /// <summary>
    ///     Attaches a Markdown table of every Revit API utility method and the descriptor file resolving it.
    /// </summary>
    [Test]
    public async Task CoversEveryStaticMethod()
    {
        var assembly = typeof(Document).Assembly;

        var rows = GetUtilityMethodRows(assembly);

        await Assert.That(rows).IsNotEmpty();

        await rows
            .OrderBy(row => row.DescriptorFiles.Count is 0 ? 0 : 1)
            .ThenBy(row => row.QualifiedName, StringComparer.Ordinal)
            .ToMarkdownTable().CreateMarkdownArtifactAsync($"{assembly.GetName().Name}-utils-{Application.VersionNumber}");
    }
}
