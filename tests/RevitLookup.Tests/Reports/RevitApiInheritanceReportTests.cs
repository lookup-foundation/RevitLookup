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

using System.Collections;
using RevitLookup.Tests.Unit.Abstractions;

namespace RevitLookup.Tests.Unit.Reports;

public sealed class RevitApiInheritanceReportTests : RevitApiInheritanceReportTest
{
    [Test]
    [Arguments("RevitAPI")]
    public async Task Report_RevitApi_ApiObjectEnumerables(string assemblyName)
    {
        // Arrange
        var fileName = $"{assemblyName}-Inheritance-ApiObjectEnumerable-{Application.VersionNumber}.md";

        // Act
        var entries = BuildEntries(assemblyName, typeof(APIObject), typeof(IEnumerable));
        var report = InheritanceReportBuilder.GenerateMarkdown(entries);

        // Assert
        await AttachReportAsync(report, fileName);
    }

    [Test]
    [Arguments("RevitAPI")]
    public async Task Report_RevitApi_AllEnumerables(string assemblyName)
    {
        // Arrange
        var fileName = $"{assemblyName}-Inheritance-Enumerable-{Application.VersionNumber}.md";

        // Act
        var entries = BuildEntries(assemblyName, typeof(IEnumerable));
        var report = InheritanceReportBuilder.GenerateMarkdown(entries);

        // Assert
        await AttachReportAsync(report, fileName);
    }
}