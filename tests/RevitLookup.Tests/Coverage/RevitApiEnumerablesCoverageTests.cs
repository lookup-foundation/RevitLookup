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
using RevitLookup.Tests.Artifacts;
using RevitLookup.Tests.Coverage.Formatters;

namespace RevitLookup.Tests.Coverage;

/// <summary>
///     Reports how <c>EnumerableDescriptor</c> finds out whether each Revit API collection contains any elements.
/// </summary>
public sealed class RevitApiEnumerablesCoverageTests : RevitApiReportTest
{
    /// <summary>
    ///     Attaches a Markdown table of every Revit API collection, leading with the types carrying an <c>IsEmpty</c> or a <c>Count</c> no descriptor switch arm reads yet.
    /// </summary>
    /// <remarks>
    ///     The descriptor asks every collection it decomposes whether it contains any elements.
    ///     A type its switch does not match answers only through an enumerator the descriptor creates and moves.
    ///     Each leading row names a switch arm worth adding to <c>EnumerableDescriptor</c>.
    /// </remarks>
    [Test]
    public async Task CoversEveryEnumerable()
    {
        var assembly = typeof(Document).Assembly;

        var rows = GetEnumerableRows(assembly);

        await Assert.That(rows).IsNotEmpty();

        await rows
            .OrderBy(row => row.Coverage)
            .ThenBy(row => row.TypeName, StringComparer.Ordinal)
            .ToMarkdownTable()
            .CreateMarkdownArtifactAsync($"{assembly.GetName().Name}-enumerables-{Application.VersionNumber}");
    }
}
