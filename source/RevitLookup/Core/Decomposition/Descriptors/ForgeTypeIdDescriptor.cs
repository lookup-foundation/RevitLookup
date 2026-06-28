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

public sealed class ForgeTypeIdDescriptor : Descriptor, IDescriptorConfigurator
#if REVIT2024_OR_GREATER
    , IDescriptorConfigurator<Document>
#endif
{
    private readonly ForgeTypeId _typeId;

    public ForgeTypeIdDescriptor(ForgeTypeId typeId)
    {
        _typeId = typeId;
        Name = typeId.TypeId;
    }

    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(ForgeTypeId.Dispose)).Disable();
        configuration.Member(nameof(ForgeTypeId.Clear)).When(parameters => parameters.Length == 0).Disable();

        configuration.Extension(nameof(LabelUtils.GetLabelForUnit)).Register(() => LabelUtils.GetLabelForUnit(_typeId));
        configuration.Extension(nameof(LabelUtils.GetLabelForSpec)).Register(() => LabelUtils.GetLabelForSpec(_typeId));
        configuration.Extension(nameof(LabelUtils.GetLabelForSymbol)).Register(() => LabelUtils.GetLabelForSymbol(_typeId));
        configuration.Extension(nameof(UnitUtils.IsUnit)).Register(() => UnitUtils.IsUnit(_typeId));
        configuration.Extension(nameof(UnitUtils.IsSymbol)).Register(() => UnitUtils.IsSymbol(_typeId));
        configuration.Extension(nameof(UnitUtils.GetAllUnits)).AsStatic().Register(UnitUtils.GetAllUnits);
        configuration.Extension(nameof(UnitUtils.GetTypeCatalogStringForSpec)).Register(() => UnitUtils.GetTypeCatalogStringForSpec(_typeId));
        configuration.Extension(nameof(UnitUtils.GetTypeCatalogStringForUnit)).Register(() => UnitUtils.GetTypeCatalogStringForUnit(_typeId));
        configuration.Extension(nameof(UnitUtils.GetValidUnits)).Register(() => UnitUtils.GetValidUnits(_typeId));
        configuration.Extension(nameof(UnitUtils.IsValidUnit)).Register(ResolveIsValidUnit);
        configuration.Extension(nameof(UnitUtils.Convert)).NotSupported();
        configuration.Extension(nameof(UnitUtils.ConvertFromInternalUnits)).NotSupported();
        configuration.Extension(nameof(UnitUtils.ConvertToInternalUnits)).NotSupported();
#if REVIT2022_OR_GREATER
        configuration.Extension(nameof(LabelUtils.GetLabelForGroup)).Register(() => LabelUtils.GetLabelForGroup(_typeId));
        configuration.Extension(nameof(LabelUtils.GetLabelForDiscipline)).Register(() => LabelUtils.GetLabelForDiscipline(_typeId));
        configuration.Extension(nameof(LabelUtils.GetLabelForBuiltInParameter)).Register(() => LabelUtils.GetLabelForBuiltInParameter(_typeId));
        configuration.Extension(nameof(ParameterUtils.IsBuiltInParameter)).Register(() => ParameterUtils.IsBuiltInParameter(_typeId));
        configuration.Extension(nameof(ParameterUtils.IsBuiltInGroup)).Register(() => ParameterUtils.IsBuiltInGroup(_typeId));
        configuration.Extension(nameof(ParameterUtils.GetBuiltInParameter)).Register(() => ParameterUtils.GetBuiltInParameter(_typeId));
        configuration.Extension(nameof(ParameterUtils.GetAllBuiltInParameters)).AsStatic().Register(ParameterUtils.GetAllBuiltInParameters);
        configuration.Extension(nameof(ParameterUtils.GetAllBuiltInGroups)).AsStatic().Register(ParameterUtils.GetAllBuiltInGroups);
        configuration.Extension(nameof(UnitUtils.IsMeasurableSpec)).Register(() => UnitUtils.IsMeasurableSpec(_typeId));
        configuration.Extension(nameof(UnitUtils.GetDiscipline)).Register(() => UnitUtils.GetDiscipline(_typeId));
        configuration.Extension(nameof(UnitUtils.GetAllDisciplines)).AsStatic().Register(UnitUtils.GetAllDisciplines);
        configuration.Extension(nameof(UnitUtils.GetAllMeasurableSpecs)).AsStatic().Register(UnitUtils.GetAllMeasurableSpecs);
        configuration.Extension(nameof(SpecUtils.IsSpec)).Register(() => SpecUtils.IsSpec(_typeId));
        configuration.Extension(nameof(SpecUtils.IsValidDataType)).Register(() => SpecUtils.IsValidDataType(_typeId));
        configuration.Extension(nameof(SpecUtils.GetAllSpecs)).AsStatic().Register(SpecUtils.GetAllSpecs);
#endif
#if REVIT2024_OR_GREATER
#if REVIT2027_OR_GREATER
        configuration.Extension(nameof(ParameterUtils.DownloadParameterOptions)).Register(ResolveDownloadParameterOptions);
#else
        configuration.Extension(nameof(ParameterUtils.DownloadParameterOptions)).Register(() => ParameterUtils.DownloadParameterOptions(_typeId));
#endif
#endif
#if REVIT2026_OR_GREATER
        configuration.Extension(nameof(ParameterUtils.GetBuiltInParameterGroupTypeId)).Register(() => ParameterUtils.GetBuiltInParameterGroupTypeId(_typeId));
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

#if REVIT2027_OR_GREATER
        IVariant ResolveDownloadParameterOptions()
        {
            var regions = RevitApiContext.Application.GetAllCloudRegions();
            var variants = Variants.Values<ParameterDownloadOptions>(regions.Count);

            foreach (var region in regions)
            {
                variants.Add(ParameterUtils.DownloadParameterOptions(_typeId, region), region);
            }

            return variants.Consume();
        }
#endif
    }

#if REVIT2024_OR_GREATER
    void IDescriptorConfigurator<Document>.Configure(IMemberConfigurator<Document> configuration)
    {
#if REVIT2027_OR_GREATER
        configuration.Extension(nameof(ParameterUtils.DownloadParameter)).Register(ResolveDownloadParameter);
        configuration.Extension(nameof(ParameterUtils.DownloadCompanyName)).Register(ResolveDownloadCompanyName);
#else
        configuration.Extension(nameof(ParameterUtils.DownloadParameter)).Register(context => ParameterUtils.DownloadParameter(context, new ParameterDownloadOptions(), _typeId));
        configuration.Extension(nameof(ParameterUtils.DownloadCompanyName)).Register(context => ParameterUtils.DownloadCompanyName(context, _typeId));
#endif
#if REVIT2027_OR_GREATER
        return;

        IVariant ResolveDownloadParameter(Document context)
        {
            var regions = RevitApiContext.Application.GetAllCloudRegions();
            var options = new ParameterDownloadOptions();
            var variants = Variants.Values<SharedParameterElement>(regions.Count);

            foreach (var region in regions)
            {
                variants.Add(ParameterUtils.DownloadParameter(context, options, _typeId, region), region);
            }

            return variants.Consume();
        }

        IVariant ResolveDownloadCompanyName(Document context)
        {
            var regions = RevitApiContext.Application.GetAllCloudRegions();
            var variants = Variants.Values<string>(regions.Count);

            foreach (var region in regions)
            {
                variants.Add(ParameterUtils.DownloadCompanyName(context, _typeId, region), region);
            }

            return variants.Consume();
        }
#endif
    }
#endif
}