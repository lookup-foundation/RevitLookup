using Installer;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

const string outputName = "RevitLookup";
const string projectName = "RevitLookup";

var guidMap = new Dictionary<int, string>
{
    {2021, "9B7FD05D-C782-4538-A5D3-04B64AE81FE4"},
    {2022, "207733B1-1BEA-4603-99EA-EA1E87077F60"},
    {2023, "2179ECCB-0ED3-4FFF-907D-01C9D57AD20D"},
    {2024, "2E347D52-D08D-4624-8909-3679D75B9C1D"},
    {2025, "5182B068-1BE3-42A9-B91D-BF4BEEE81680"},
    {2026, "468ECD03-68D6-4D99-B04C-13D72AB47CBC"},
    {2027, "8CCB4872-7F50-43C9-B3FE-50FEF55F96F4"},
};

var versioning = Versioning.CreateFromVersionStringAsync(args[0]);
if (!guidMap.TryGetValue(versioning.VersionPrefix.Major, out var guid))
{
    throw new ArgumentOutOfRangeException($"Version GUID mapping missing for the specified version: {versioning.VersionPrefix.Major}");
}

var project = new Project
{
    OutDir = "output",
    Name = projectName,
    GUID = new Guid(guid),
    Platform = Platform.x64,
    UI = WUI.WixUI_InstallDir,
    Version = versioning.MsiVersion,
    MajorUpgrade = MajorUpgrade.Default,
    BackgroundImage = @"install\Resources\Icons\BackgroundImage.png",
    BannerImage = @"install\Resources\Icons\BannerImage.png",
    ControlPanelInfo =
    {
        Manufacturer = "Lookup Foundation",
        HelpLink = "https://github.com/lookup-foundation/RevitLookup/issues",
        ProductIcon = @"install\Resources\Icons\ShellIcon.ico"
    }
};

var wixEntities = Generator.GenerateWixEntities(versioning, args[1]);
project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);

BuildSingleUserMsi();
BuildMultiUserUserMsi();

void BuildSingleUserMsi()
{
    project.Scope = InstallScope.perUser;
    project.OutFileName = $"{outputName}-{versioning.Version}-SingleUser";
    project.Dirs =
    [
        new InstallDir($@"%AppDataFolder%\Autodesk\Revit\Addins", wixEntities)
    ];
    project.BuildMsi();
}

void BuildMultiUserUserMsi()
{
    project.Scope = InstallScope.perMachine;
    project.OutFileName = $"{outputName}-{versioning.Version}-MultiUser";
    project.Dirs =
    [
        new InstallDir(versioning.VersionPrefix.Major >= 2027 ? @"%ProgramFiles%\Autodesk\Revit\Addins" : @"%CommonAppDataFolder%\Autodesk\Revit\Addins", wixEntities)
    ];
    project.BuildMsi();
}