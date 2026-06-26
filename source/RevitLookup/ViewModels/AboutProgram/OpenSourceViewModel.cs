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

using RevitLookup.Abstractions.Models.AboutProgram;
using RevitLookup.Abstractions.ViewModels.AboutProgram;

namespace RevitLookup.ViewModels.AboutProgram;

[UsedImplicitly]
public sealed class OpenSourceViewModel : ObservableObject, IOpenSourceViewModel
{
    public List<OpenSourceSoftware> Software { get; } =
    [
        new()
        {
            SoftwareName = "CommunityToolkit.Mvvm",
            SoftwareUri = "https://github.com/CommunityToolkit/dotnet",
            LicenseName = "MIT License",
            LicenseUri = "https://github.com/CommunityToolkit/dotnet/blob/main/LICENSE.md"
        },
        new()
        {
            SoftwareName = "Microsoft.Extensions.Hosting",
            SoftwareUri = "https://github.com/dotnet/runtime",
            LicenseName = "MIT License",
            LicenseUri = "https://github.com/dotnet/runtime/blob/main/LICENSE.TXT"
        },
        new()
        {
            SoftwareName = "Scrutor",
            SoftwareUri = "https://github.com/khellang/Scrutor",
            LicenseName = "MIT License",
            LicenseUri = "https://github.com/khellang/Scrutor/blob/master/LICENSE"
        },
        new()
        {
            SoftwareName = "Nice3point.Revit.Api",
            SoftwareUri = "https://github.com/Nice3point/RevitApi",
            LicenseName = "MIT License",
            LicenseUri = "https://github.com/Nice3point/RevitApi/blob/main/LICENSE.md"
        },
        new()
        {
            SoftwareName = "Nice3point.Revit.Extensions",
            SoftwareUri = "https://github.com/Nice3point/RevitExtensions",
            LicenseName = "MIT License",
            LicenseUri = "https://github.com/Nice3point/RevitExtensions/blob/main/LICENSE.md"
        },
        new()
        {
            SoftwareName = "Nice3point.Revit.Toolkit",
            SoftwareUri = "https://github.com/Nice3point/RevitToolkit",
            LicenseName = "MIT License",
            LicenseUri = "https://github.com/Nice3point/RevitToolkit/blob/main/LICENSE.md"
        },
        new()
        {
            SoftwareName = "PolySharp",
            SoftwareUri = "https://github.com/Sergio0694/PolySharp",
            LicenseName = "MIT License",
            LicenseUri = "https://github.com/Sergio0694/PolySharp/blob/main/LICENSE"
        },
        new()
        {
            SoftwareName = "Serilog",
            SoftwareUri = "https://github.com/serilog/serilog",
            LicenseName = "Apache License 2.0",
            LicenseUri = "https://github.com/serilog/serilog/blob/dev/LICENSE"
        },
        new()
        {
            SoftwareName = "Serilog.Extensions.Hosting",
            SoftwareUri = "https://github.com/serilog/serilog-extensions-hosting",
            LicenseName = "Apache License 2.0",
            LicenseUri = "https://github.com/serilog/serilog-extensions-hosting/blob/dev/LICENSE"
        },
        new()
        {
            SoftwareName = "Serilog.Sinks.Autodesk.Revit",
            SoftwareUri = "https://github.com/dosymep/Serilog.Sinks.Autodesk.Revit",
            LicenseName = "MIT License",
            LicenseUri = "https://github.com/dosymep/Serilog.Sinks.Autodesk.Revit/blob/master/LICENSE.md"
        },
        new()
        {
            SoftwareName = "Serilog.Sinks.Console",
            SoftwareUri = "https://github.com/serilog/serilog-sinks-console",
            LicenseName = "Apache License 2.0",
            LicenseUri = "https://github.com/serilog/serilog-sinks-console/blob/dev/LICENSE"
        },
        new()
        {
            SoftwareName = "Serilog.Sinks.Debug",
            SoftwareUri = "https://github.com/serilog/serilog-sinks-debug",
            LicenseName = "Apache License 2.0",
            LicenseUri = "https://github.com/serilog/serilog-sinks-debug/blob/dev/LICENSE"
        },
        new()
        {
            SoftwareName = "Riok.Mapperly",
            SoftwareUri = "https://github.com/riok/mapperly",
            LicenseName = "Apache License 2.0",
            LicenseUri = "https://github.com/riok/mapperly/blob/main/LICENSE"
        },
        new()
        {
            SoftwareName = "WPF-UI",
            SoftwareUri = "https://github.com/lepoco/wpfui",
            LicenseName = "MIT License",
            LicenseUri = "https://github.com/lepoco/wpfui/blob/main/LICENSE"
        }
    ];
}