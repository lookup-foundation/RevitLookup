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
        manager.Define(nameof(LabelUtils.GetLabelForUnit)).Register(() => Variants.Value(LabelUtils.GetLabelForUnit(_typeId)));
        manager.Define(nameof(LabelUtils.GetLabelForSpec)).Register(() => Variants.Value(LabelUtils.GetLabelForSpec(_typeId)));
        manager.Define(nameof(LabelUtils.GetLabelForSymbol)).Register(() => Variants.Value(LabelUtils.GetLabelForSymbol(_typeId)));
        manager.Define(nameof(UnitUtils.IsUnit)).Register(() => Variants.Value(UnitUtils.IsUnit(_typeId)));
        manager.Define(nameof(UnitUtils.IsSymbol)).Register(() => Variants.Value(UnitUtils.IsSymbol(_typeId)));
        manager.Define(nameof(UnitUtils.GetAllUnits)).AsStatic().Register(() => Variants.Value(UnitUtils.GetAllUnits()));
        manager.Define(nameof(UnitUtils.GetTypeCatalogStringForSpec)).Register(() => Variants.Value(UnitUtils.GetTypeCatalogStringForSpec(_typeId)));
        manager.Define(nameof(UnitUtils.GetTypeCatalogStringForUnit)).Register(() => Variants.Value(UnitUtils.GetTypeCatalogStringForUnit(_typeId)));
        manager.Define(nameof(UnitUtils.GetValidUnits)).Register(() => Variants.Value(UnitUtils.GetValidUnits(_typeId)));
        manager.Define(nameof(UnitUtils.IsValidUnit)).Register(ResolveIsValidUnit);
        manager.Define(nameof(UnitUtils.Convert)).AsNotSupported();
        manager.Define(nameof(UnitUtils.ConvertFromInternalUnits)).AsNotSupported();
        manager.Define(nameof(UnitUtils.ConvertToInternalUnits)).AsNotSupported();
#if REVIT2022_OR_GREATER
        manager.Define(nameof(LabelUtils.GetLabelForGroup)).Register(() => Variants.Value(LabelUtils.GetLabelForGroup(_typeId)));
        manager.Define(nameof(LabelUtils.GetLabelForDiscipline)).Register(() => Variants.Value(LabelUtils.GetLabelForDiscipline(_typeId)));
        manager.Define(nameof(LabelUtils.GetLabelForBuiltInParameter)).Register(() => Variants.Value(LabelUtils.GetLabelForBuiltInParameter(_typeId)));
        manager.Define(nameof(ParameterUtils.IsBuiltInParameter)).Register(() => Variants.Value(ParameterUtils.IsBuiltInParameter(_typeId)));
        manager.Define(nameof(ParameterUtils.IsBuiltInGroup)).Register(() => Variants.Value(ParameterUtils.IsBuiltInGroup(_typeId)));
        manager.Define(nameof(ParameterUtils.GetBuiltInParameter)).Register(() => Variants.Value(ParameterUtils.GetBuiltInParameter(_typeId)));
        manager.Define(nameof(ParameterUtils.GetAllBuiltInParameters)).AsStatic().Register(() => Variants.Value(ParameterUtils.GetAllBuiltInParameters()));
        manager.Define(nameof(ParameterUtils.GetAllBuiltInGroups)).AsStatic().Register(() => Variants.Value(ParameterUtils.GetAllBuiltInGroups()));
        manager.Define(nameof(UnitUtils.IsMeasurableSpec)).Register(() => Variants.Value(UnitUtils.IsMeasurableSpec(_typeId)));
        manager.Define(nameof(UnitUtils.GetDiscipline)).Register(() => Variants.Value(UnitUtils.GetDiscipline(_typeId)));
        manager.Define(nameof(UnitUtils.GetAllDisciplines)).AsStatic().Register(() => Variants.Value(UnitUtils.GetAllDisciplines()));
        manager.Define(nameof(UnitUtils.GetAllMeasurableSpecs)).AsStatic().Register(() => Variants.Value(UnitUtils.GetAllMeasurableSpecs()));
        manager.Define(nameof(SpecUtils.IsSpec)).Register(() => Variants.Value(SpecUtils.IsSpec(_typeId)));
        manager.Define(nameof(SpecUtils.IsValidDataType)).Register(() => Variants.Value(SpecUtils.IsValidDataType(_typeId)));
        manager.Define(nameof(SpecUtils.GetAllSpecs)).AsStatic().Register(() => Variants.Value(SpecUtils.GetAllSpecs()));
#endif
#if REVIT2024_OR_GREATER
        manager.Define(nameof(ParameterUtils.DownloadParameter)).AsNotSupported(); //TODO slow
        manager.Define(nameof(ParameterUtils.DownloadCompanyName)).AsNotSupported(); //TODO slow
        manager.Define(nameof(ParameterUtils.DownloadParameterOptions)).AsNotSupported(); //TODO slow
        // manager.Define(nameof(ParameterUtils.DownloadParameter)).Register(context => Variants.Value(ParameterUtils.DownloadParameter(context, new ParameterDownloadOptions(), _typeId)));
        // manager.Define(nameof(ParameterUtils.DownloadCompanyName)).Register(context => Variants.Value(ParameterUtils.DownloadCompanyName(context, _typeId)));
        // manager.Define(nameof(ParameterUtils.DownloadParameterOptions)).Register(() => Variants.Value(ParameterUtils.DownloadParameterOptions(_typeId)));
#endif
#if REVIT2026_OR_GREATER
        manager.Define(nameof(ParameterUtils.GetBuiltInParameterGroupTypeId)).Register(() => Variants.Value(ParameterUtils.GetBuiltInParameterGroupTypeId(_typeId)));
#endif
        return;

        IVariant ResolveIsValidUnit()
        {
#if REVIT2022_OR_GREATER
            if (!SpecUtils.IsSpec(_typeId)) return Variants.Empty<bool>();
#else
            if (!UnitUtils.IsSpec(_typeId)) return Variants.Empty<bool>();
#endif

            var unitProperties = typeof(UnitTypeId).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            var values = Variants.Values<bool>(unitProperties.Length);

            foreach (var property in unitProperties)
            {
                var propertyValue = (ForgeTypeId) property.GetValue(null)!;
                var isValidUnit = UnitUtils.IsValidUnit(_typeId, propertyValue);
                values.Add(isValidUnit, $"{property.Name}: {isValidUnit}");
            }

            return values.Consume();
        }
    }
    
}