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

using Autodesk.Revit.DB.Mechanical;
using LookupEngine.Abstractions.Configuration;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class DuctDescriptor(Duct duct) : ElementDescriptor(duct)
{
    public override void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(Duct.Dispose)).Disable();
        configuration.Extension(nameof(MechanicalUtils.BreakCurve)).NotSupported();
        configuration.Extension(nameof(MechanicalUtils.ConnectDuctPlaceholdersAtElbow)).NotSupported();
        configuration.Extension(nameof(MechanicalUtils.ConnectDuctPlaceholdersAtTee)).NotSupported();
        configuration.Extension(nameof(MechanicalUtils.ConnectDuctPlaceholdersAtCross)).NotSupported();
        configuration.Extension("ConnectAirTerminal").Map(nameof(MechanicalUtils.ConnectAirTerminalOnDuct)).NotSupported();
    }
}