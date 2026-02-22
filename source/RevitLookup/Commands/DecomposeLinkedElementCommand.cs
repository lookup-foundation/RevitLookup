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

using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Services.Application;
using RevitLookup.UI.Framework.Views.Decomposition;

namespace RevitLookup.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class DecomposeLinkedElementCommand : ExternalCommand
{
    public override void Execute()
    {
        Host.GetService<IUiOrchestratorService>()
            .Decompose(KnownDecompositionObject.LinkedElement)
            .Show<DecompositionSummaryPage>();
    }
}