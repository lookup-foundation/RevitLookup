using RevitLookup.Abstractions.Application;
using RevitLookup.ServiceDefaults.Application;

namespace RevitLookup.Tests.Unit;

public sealed class InstallationScopeResolverTests
{
    [Test]
    [Arguments(Environment.SpecialFolder.ProgramFiles, InstallationScope.PerMachine)]
    [Arguments(Environment.SpecialFolder.ProgramFilesX86, InstallationScope.PerMachine)]
    [Arguments(Environment.SpecialFolder.CommonApplicationData, InstallationScope.PerMachine)]
    [Arguments(Environment.SpecialFolder.ApplicationData, InstallationScope.PerUser)]
    [Arguments(Environment.SpecialFolder.LocalApplicationData, InstallationScope.PerUser)]
    public async Task Resolve_InstallerTargetFolder_ReportsScope(Environment.SpecialFolder folder, InstallationScope expected)
    {
        // Arrange
        var location = Path.Combine(Environment.GetFolderPath(folder), "Autodesk", "Revit", "Addins", "2027", "RevitLookup.dll");

        // Act
        var scope = InstallationScopeResolver.Resolve(location);

        // Assert
        await Assert.That(scope).IsEqualTo(expected);
    }

    [Test]
    [Arguments(Environment.SpecialFolder.ProgramFiles)]
    [Arguments(Environment.SpecialFolder.ProgramFilesX86)]
    [Arguments(Environment.SpecialFolder.CommonApplicationData)]
    public async Task Resolve_MachineWideFolderItself_ReportsPerMachine(Environment.SpecialFolder folder)
    {
        // Arrange
        var location = Environment.GetFolderPath(folder);

        // Act
        var scope = InstallationScopeResolver.Resolve(location);

        // Assert
        await Assert.That(scope).IsEqualTo(InstallationScope.PerMachine);
    }

    [Test]
    [Arguments(Environment.SpecialFolder.ProgramFiles, " Portable")]
    [Arguments(Environment.SpecialFolder.ProgramFilesX86, "Backup")]
    [Arguments(Environment.SpecialFolder.CommonApplicationData, ".Old")]
    public async Task Resolve_FolderSharingMachineWidePrefix_ReportsPerUser(Environment.SpecialFolder folder, string suffix)
    {
        // Arrange
        var location = Path.Combine($"{Environment.GetFolderPath(folder)}{suffix}", "RevitLookup.dll");

        // Act
        var scope = InstallationScopeResolver.Resolve(location);

        // Assert
        await Assert.That(scope).IsEqualTo(InstallationScope.PerUser);
    }

    [Test]
    [Arguments(@"D:\Repositories\RevitLookup\output\RevitLookup.dll")]
    [Arguments(@"C:\Autodesk\Revit\Addins\2027\RevitLookup.dll")]
    [Arguments(@"\\Server\Deployment\RevitLookup\RevitLookup.dll")]
    [Arguments("")]
    public async Task Resolve_LocationOutsideInstallerTargets_ReportsPerUser(string location)
    {
        // Act
        var scope = InstallationScopeResolver.Resolve(location);

        // Assert
        await Assert.That(scope).IsEqualTo(InstallationScope.PerUser);
    }
}
