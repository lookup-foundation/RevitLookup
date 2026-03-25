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

public sealed class TableDataDescriptor(TableData tableData) : Descriptor, IDescriptorResolver
{
    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(TableData.GetSectionData)
                when parameters.Length == 1 && parameters[0].ParameterType == typeof(SectionType) => () => VariantsResolver.ResolveEnum<SectionType, TableSectionData>(tableData.GetSectionData),
            nameof(TableData.GetSectionData)
                when parameters.Length == 1 && parameters[0].ParameterType == typeof(int) => ResolveSectionDataByIndex,
            nameof(TableData.IsValidZoomLevel) => ResolveZoomLevel,
            _ => null
        };

        IVariant ResolveSectionDataByIndex()
        {
            var variants = Variants.Values<TableSectionData>(tableData.NumberOfSections);
            for (var i = 0; i < tableData.NumberOfSections; i++)
            {
                variants.Add(tableData.GetSectionData(i), i.ToString());
            }

            return variants.Consume();
        }

        IVariant ResolveZoomLevel()
        {
            var variants = Variants.Values<bool>(512);

            var zoom = 0;
            var emptyIterations = 0;
            while (emptyIterations < 50)
            {
                var isValid = tableData.IsValidZoomLevel(zoom);
                if (isValid)
                {
                    variants.Add(true, $"{zoom}: valid");
                    emptyIterations = 0;
                }
                else
                {
                    emptyIterations++;
                }

                zoom++;
            }

            return variants.Consume();
        }
    }
}