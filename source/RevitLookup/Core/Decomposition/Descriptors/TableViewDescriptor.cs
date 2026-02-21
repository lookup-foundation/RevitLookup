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
using Autodesk.Revit.DB.Electrical;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class TableViewDescriptor(TableView tableView) : ElementDescriptor(tableView)
{
    public override Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            // nameof(TableView.GetAvailableParameterCategories) => ResolveAvailableParameterCategories, //TODO disabled, long computation time
            nameof(TableView.GetAvailableParameters) => ResolveAvailableParameters,
            nameof(TableView.GetCalculatedValueName) => ResolveCalculatedValueName,
            nameof(TableView.GetCalculatedValueText) => ResolveCalculatedValueText,
            nameof(TableView.IsValidSectionType) => ResolveIsValidSectionType,
            nameof(TableView.GetCellText) => ResolveCellText,
            _ => null
        };

        IVariant ResolveAvailableParameters()
        {
            var categories = tableView.Document.Settings.Categories;
            var variants = Variants.Values<IList<ElementId>>(categories.Size);
            foreach (Category category in categories)
            {
                var result = TableView.GetAvailableParameters(tableView.Document, category.Id);
                variants.Add(result, $"{category.Name}");
            }

            return variants.Consume();
        }

        IVariant ResolveCalculatedValueName()
        {
            var tableData = tableView switch
            {
                ViewSchedule viewSchedule => viewSchedule.GetTableData(),
                PanelScheduleView panelScheduleView => panelScheduleView.GetTableData(),
                _ => throw new NotSupportedException($"{tableView.GetType().FullName} is not supported in the current API version")
            };

            var sectionTypes = Enum.GetValues(typeof(SectionType));
            var variants = Variants.Values<string>(sectionTypes.Length);
            foreach (SectionType sectionType in sectionTypes)
            {
                var tableSectionData = tableData!.GetSectionData(sectionType);
                if (tableSectionData is null) continue;

                for (var i = tableSectionData.FirstRowNumber; i < tableSectionData.LastRowNumber; i++)
                for (var j = tableSectionData.FirstColumnNumber; j < tableSectionData.LastColumnNumber; j++)
                {
                    var result = tableView.GetCalculatedValueName(sectionType, i, j);
                    variants.Add(result, $"{sectionType}, row {i}, column {j}: {result}");
                }
            }

            return variants.Consume();
        }

        IVariant ResolveCalculatedValueText()
        {
            var tableData = tableView switch
            {
                ViewSchedule viewSchedule => viewSchedule.GetTableData(),
                PanelScheduleView panelScheduleView => panelScheduleView.GetTableData(),
                _ => throw new NotSupportedException($"{tableView.GetType().FullName} is not supported in the current API version")
            };

            var sectionTypes = Enum.GetValues(typeof(SectionType));
            var variants = Variants.Values<string>(sectionTypes.Length);
            foreach (SectionType sectionType in sectionTypes)
            {
                var tableSectionData = tableData!.GetSectionData(sectionType);
                if (tableSectionData is null) continue;

                for (var i = tableSectionData.FirstRowNumber; i < tableSectionData.LastRowNumber; i++)
                for (var j = tableSectionData.FirstColumnNumber; j < tableSectionData.LastColumnNumber; j++)
                {
                    var result = tableView.GetCalculatedValueText(sectionType, i, j);
                    variants.Add(result, $"{sectionType}, row {i}, column {j}: {result}");
                }
            }

            return variants.Consume();
        }

        IVariant ResolveCellText()
        {
            var tableData = tableView switch
            {
                ViewSchedule viewSchedule => viewSchedule.GetTableData(),
                PanelScheduleView panelScheduleView => panelScheduleView.GetTableData(),
                _ => throw new NotSupportedException($"{tableView.GetType().FullName} is not supported in the current API version")
            };

            var sectionTypes = Enum.GetValues(typeof(SectionType));
            var variants = Variants.Values<string>(sectionTypes.Length);
            foreach (SectionType sectionType in sectionTypes)
            {
                var tableSectionData = tableData!.GetSectionData(sectionType);
                if (tableSectionData is null) continue;
                for (var i = tableSectionData.FirstRowNumber; i < tableSectionData.LastRowNumber; i++)
                for (var j = tableSectionData.FirstColumnNumber; j < tableSectionData.LastColumnNumber; j++)
                {
                    var result = tableView.GetCellText(sectionType, i, j);
                    variants.Add(result, $"{sectionType}, row {i}, column {j}: {result}");
                }
            }

            return variants.Consume();
        }

        IVariant ResolveIsValidSectionType()
        {
            var sectionTypes = Enum.GetValues(typeof(SectionType));
            var variants = Variants.Values<bool>(sectionTypes.Length);
            foreach (SectionType sectionType in sectionTypes)
            {
                var result = tableView.IsValidSectionType(sectionType);
                variants.Add(result, $"{sectionType}: {result}");
            }

            return variants.Consume();
        }
    }
}