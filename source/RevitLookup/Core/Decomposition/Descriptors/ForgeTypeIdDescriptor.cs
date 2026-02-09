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

public sealed class ForgeTypeIdDescriptor : Descriptor, IDescriptorResolver, IDescriptorExtension
{
    private readonly ForgeTypeId _typeId;

    public ForgeTypeIdDescriptor(ForgeTypeId typeId)
    {
        _typeId = typeId;
        Name = typeId.TypeId;
    }

    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(ForgeTypeId.Clear) when parameters.Length == 0 => Variants.Disabled,
            _ => null
        };
    }

    public void RegisterExtensions(IExtensionManager manager)
    {
        manager.Register(nameof(LabelUtils.GetLabelForUnit), () => Variants.Value(LabelUtils.GetLabelForUnit(_typeId)));
        manager.Register(nameof(LabelUtils.GetLabelForSpec), () => Variants.Value(LabelUtils.GetLabelForSpec(_typeId)));
        manager.Register(nameof(LabelUtils.GetLabelForSymbol), () => Variants.Value(LabelUtils.GetLabelForSymbol(_typeId)));
#if REVIT2022_OR_GREATER
        manager.Register(nameof(LabelUtils.GetLabelForGroup), () => Variants.Value(LabelUtils.GetLabelForGroup(_typeId)));
        manager.Register(nameof(LabelUtils.GetLabelForDiscipline), () => Variants.Value(LabelUtils.GetLabelForDiscipline(_typeId)));
        manager.Register(nameof(LabelUtils.GetLabelForBuiltInParameter), () => Variants.Value(LabelUtils.GetLabelForBuiltInParameter(_typeId)));
#endif
        manager.Register(nameof(UnitUtils.IsUnit), () => Variants.Value(UnitUtils.IsUnit(_typeId)));
        manager.Register(nameof(UnitUtils.IsSymbol), () => Variants.Value(UnitUtils.IsSymbol(_typeId)));
#if REVIT2022_OR_GREATER
        manager.Register(nameof(SpecUtils.IsSpec), () => Variants.Value(SpecUtils.IsSpec(_typeId)));
        manager.Register(nameof(UnitUtils.IsMeasurableSpec), () => Variants.Value(UnitUtils.IsMeasurableSpec(_typeId)));
        manager.Register(nameof(ParameterUtils.IsBuiltInParameter), () => Variants.Value(ParameterUtils.IsBuiltInParameter(_typeId)));
        manager.Register(nameof(ParameterUtils.IsBuiltInGroup), () => Variants.Value(ParameterUtils.IsBuiltInGroup(_typeId)));
#endif
    }
}