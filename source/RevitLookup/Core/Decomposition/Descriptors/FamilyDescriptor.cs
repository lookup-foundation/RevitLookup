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
        manager.Register(nameof(FamilySizeTableManager.GetFamilySizeTableManager), RegisterGetFamilySizeTableManager);
        manager.Register(nameof(FamilyUtils.FamilyCanConvertToFaceHostBased), RegisterFamilyCanConvertToFaceHostBased);
        manager.Register(nameof(FamilyUtils.GetProfileSymbols), RegisterProfileSymbols);
        manager.Register(nameof(FamilyUtils.ConvertFamilyToFaceHostBased), Variants.NotSupported);
        manager.Register(nameof(AdaptiveComponentFamilyUtils.GetNumberOfAdaptivePoints), () => Variants.Value(AdaptiveComponentFamilyUtils.GetNumberOfAdaptivePoints(family)));
        manager.Register(nameof(AdaptiveComponentFamilyUtils.GetNumberOfPlacementPoints), () => Variants.Value(AdaptiveComponentFamilyUtils.GetNumberOfPlacementPoints(family)));
        manager.Register(nameof(AdaptiveComponentFamilyUtils.GetNumberOfShapeHandlePoints), () => Variants.Value(AdaptiveComponentFamilyUtils.GetNumberOfShapeHandlePoints(family)));
        manager.Register(nameof(AdaptiveComponentFamilyUtils.IsAdaptiveComponentFamily), () => Variants.Value(AdaptiveComponentFamilyUtils.IsAdaptiveComponentFamily(family)));
        return;

        IVariant RegisterGetFamilySizeTableManager()
        {
            return Variants.Value(FamilySizeTableManager.GetFamilySizeTableManager(family.Document, family.Id));
        }

        IVariant RegisterFamilyCanConvertToFaceHostBased()
        {
            return Variants.Value(FamilyUtils.FamilyCanConvertToFaceHostBased(family.Document, family.Id));
        }

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