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

using Autodesk.Revit.DB.Structure.StructuralSections;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class FamilySymbolDescriptor(FamilySymbol familySymbol) : ElementDescriptor(familySymbol)
{
    public override void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define("SetStructuralSection").Map(nameof(StructuralSectionUtils.SetStructuralSection)).AsNotSupported();
        if (manager.Define(nameof(AdaptiveComponentInstanceUtils.IsAdaptiveFamilySymbol)).TryRegister(() => Variants.Value(AdaptiveComponentInstanceUtils.IsAdaptiveFamilySymbol(familySymbol))))
        {
            manager.Define("CreateAdaptiveComponentInstance").Map(nameof(AdaptiveComponentInstanceUtils.CreateAdaptiveComponentInstance)).AsNotSupported();
        }
    }
}