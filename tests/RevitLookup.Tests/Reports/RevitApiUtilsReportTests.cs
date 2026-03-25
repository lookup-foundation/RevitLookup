using RevitLookup.Tests.Unit.Abstractions;

namespace RevitLookup.Tests.Unit.Reports;

public sealed class RevitApiUtilsReportTests : RevitApiReportTest
{
    [Test]
    [Arguments("RevitAPI")]
    public async Task Report_RevitApi_AllUtilsMethods(string assemblyName)
    {
        // Arrange
        var fileName = $"{assemblyName}-Utils-All-{Application.VersionNumber}.md";

        // Act
        var entries = BuildEntries(assemblyName);
        var report = ApiReportBuilder.GenerateMarkdown(entries);

        // Assert
        await AttachReportAsync(report, fileName);
    }

    [Test]
    [Arguments("RevitAPI")]
    public async Task Report_RevitApi_ImplementedUtilsMethods(string assemblyName)
    {
        // Arrange
        var fileName = $"{assemblyName}-Utils-Implemented-{Application.VersionNumber}.md";

        // Act
        var entries = BuildEntries(assemblyName).Where(entry => entry.Descriptors.Count > 0);
        var report = ApiReportBuilder.GenerateMarkdown(entries);

        // Assert
        await AttachReportAsync(report, fileName);
    }

    [Test]
    [Arguments("RevitAPI")]
    public async Task Report_RevitApi_NotImplementedUtilsMethods(string assemblyName)
    {
        // Arrange
        var fileName = $"{assemblyName}-Utils-NotImplemented-{Application.VersionNumber}.md";

        // Act
        var entries = BuildEntries(assemblyName).Where(entry => entry.Descriptors.Count == 0);
        var report = ApiReportBuilder.GenerateMarkdown(entries);

        // Assert
        await AttachReportAsync(report, fileName);
    }
}