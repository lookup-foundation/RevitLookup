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

using System.Reflection;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class FamilyDescriptor(Family family) : ElementDescriptor(family)
{
    public override Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return null;
    }

    public override void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define(nameof(FamilySizeTableManager.GetFamilySizeTableManager)).Register(() => Variants.Value(FamilySizeTableManager.GetFamilySizeTableManager(family.Document, family.Id)));
        manager.Define(nameof(FamilyUtils.GetProfileSymbols)).Register(RegisterProfileSymbols);
        manager.Define(nameof(LoadedFamilyIntegrityCheck.CheckFamily)).Register(() => Variants.Value(LoadedFamilyIntegrityCheck.CheckFamily(family.Document, family.Id)));
        manager.Define("CanBeConvertedToFaceHostBased").Register(() => Variants.Value(FamilyUtils.FamilyCanConvertToFaceHostBased(family.Document, family.Id)));
        manager.Define("ConvertToFaceHostBased").Map(nameof(FamilyUtils.ConvertFamilyToFaceHostBased)).AsNotSupported();
        
        if (manager.Define(nameof(AdaptiveComponentFamilyUtils.IsAdaptiveComponentFamily)).TryRegister(() => Variants.Value(AdaptiveComponentFamilyUtils.IsAdaptiveComponentFamily(family))))
        {
            manager.Define(nameof(AdaptiveComponentFamilyUtils.GetNumberOfAdaptivePoints)).Register(() => Variants.Value(AdaptiveComponentFamilyUtils.GetNumberOfAdaptivePoints(family)));
            manager.Define(nameof(AdaptiveComponentFamilyUtils.GetNumberOfPlacementPoints)).Register(() => Variants.Value(AdaptiveComponentFamilyUtils.GetNumberOfPlacementPoints(family)));
            manager.Define(nameof(AdaptiveComponentFamilyUtils.GetNumberOfShapeHandlePoints)).Register(() => Variants.Value(AdaptiveComponentFamilyUtils.GetNumberOfShapeHandlePoints(family)));
        }

        return;
        
        IVariant RegisterProfileSymbols()
        {
            var values = Enum.GetValues(typeof(ProfileFamilyUsage));
            var capacity = values.Length * 2;
            var variants = Variants.Values<ICollection<ElementId>>(capacity);
            foreach (ProfileFamilyUsage value in values)
            {
                variants.Add(FamilyUtils.GetProfileSymbols(family.Document, value, false), $"{value}, with multiple curve loops");
                variants.Add(FamilyUtils.GetProfileSymbols(family.Document, value, true), $"{value}, with single curve loop");
            }

            return variants.Consume();
        }
    }

}