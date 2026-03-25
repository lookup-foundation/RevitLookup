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

public sealed class ForgeTypeIdDescriptor : Descriptor, IDescriptorResolver, IDescriptorExtension, IDescriptorExtension<Document>
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
        manager.Register(nameof(UnitUtils.IsUnit), () => Variants.Value(UnitUtils.IsUnit(_typeId)));
        manager.Register(nameof(UnitUtils.IsSymbol), () => Variants.Value(UnitUtils.IsSymbol(_typeId)));
        manager.Register(nameof(UnitUtils.GetAllUnits), () => Variants.Value(UnitUtils.GetAllUnits()));
        manager.Register(nameof(UnitUtils.GetTypeCatalogStringForSpec), () => Variants.Value(UnitUtils.GetTypeCatalogStringForSpec(_typeId)));
        manager.Register(nameof(UnitUtils.GetTypeCatalogStringForUnit), () => Variants.Value(UnitUtils.GetTypeCatalogStringForUnit(_typeId)));
        manager.Register(nameof(UnitUtils.GetValidUnits), () => Variants.Value(UnitUtils.GetValidUnits(_typeId)));
        manager.Register(nameof(UnitUtils.IsValidUnit), ResolveIsValidUnit);
#if REVIT2022_OR_GREATER
        manager.Register(nameof(LabelUtils.GetLabelForGroup), () => Variants.Value(LabelUtils.GetLabelForGroup(_typeId)));
        manager.Register(nameof(LabelUtils.GetLabelForDiscipline), () => Variants.Value(LabelUtils.GetLabelForDiscipline(_typeId)));
        manager.Register(nameof(LabelUtils.GetLabelForBuiltInParameter), () => Variants.Value(LabelUtils.GetLabelForBuiltInParameter(_typeId)));
        manager.Register(nameof(ParameterUtils.IsBuiltInParameter), () => Variants.Value(ParameterUtils.IsBuiltInParameter(_typeId)));
        manager.Register(nameof(ParameterUtils.IsBuiltInGroup), () => Variants.Value(ParameterUtils.IsBuiltInGroup(_typeId)));
        manager.Register(nameof(ParameterUtils.GetBuiltInParameter), () => Variants.Value(ParameterUtils.GetBuiltInParameter(_typeId)));
        manager.Register(nameof(UnitUtils.IsMeasurableSpec), () => Variants.Value(UnitUtils.IsMeasurableSpec(_typeId)));
        manager.Register(nameof(UnitUtils.GetDiscipline), () => Variants.Value(UnitUtils.GetDiscipline(_typeId)));
        manager.Register(nameof(SpecUtils.IsSpec), () => Variants.Value(SpecUtils.IsSpec(_typeId)));
        manager.Register(nameof(SpecUtils.IsValidDataType), () => Variants.Value(SpecUtils.IsValidDataType(_typeId)));
#endif
#if REVIT2024_OR_GREATER
        manager.Register(nameof(ParameterUtils.DownloadParameterOptions), () => Variants.Value(ParameterUtils.DownloadParameterOptions(_typeId)));
#endif
#if REVIT2026_OR_GREATER
        manager.Register(nameof(ParameterUtils.GetBuiltInParameterGroupTypeId), () => Variants.Value(ParameterUtils.GetBuiltInParameterGroupTypeId(_typeId)));
#endif

        RegisterNotSupportedExtensions();
        return;

        // Indicates API methods that exist but cannot produce a read-only value in RevitLookup
        void RegisterNotSupportedExtensions()
        {
            manager.Register(nameof(UnitUtils.Convert), Variants.NotSupported);
            manager.Register(nameof(UnitUtils.ConvertFromInternalUnits), Variants.NotSupported);
            manager.Register(nameof(UnitUtils.ConvertToInternalUnits), Variants.NotSupported);
        }

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

    public void RegisterExtensions(IExtensionManager<Document> manager)
    {
#if REVIT2024_OR_GREATER
        manager.Register(nameof(ParameterUtils.DownloadParameter), context => Variants.Value(ParameterUtils.DownloadParameter(context, new ParameterDownloadOptions(), _typeId)));
        manager.Register(nameof(ParameterUtils.DownloadCompanyName), context => Variants.Value(ParameterUtils.DownloadCompanyName(context, _typeId)));
#endif
    }
}