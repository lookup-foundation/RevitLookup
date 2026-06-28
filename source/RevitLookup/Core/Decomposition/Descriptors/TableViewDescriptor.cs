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

using Autodesk.Revit.DB.Electrical;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class TableViewDescriptor(TableView tableView) : ElementDescriptor(tableView)
{
    public override void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(TableView.Dispose)).Disable();
        configuration.Member(nameof(TableView.GetAvailableParameters)).Resolve(ResolveAvailableParameters);
        configuration.Member(nameof(TableView.GetCalculatedValueName)).Resolve(() => ResolveTableViewCells(tableView.GetCalculatedValueName));
        configuration.Member(nameof(TableView.GetCalculatedValueText)).Resolve(() => ResolveTableViewCells(tableView.GetCalculatedValueText));
        configuration.Member(nameof(TableView.IsValidSectionType)).Resolve(() => ResolveEnum<SectionType, bool>(tableView.IsValidSectionType));
        configuration.Member(nameof(TableView.GetCellText)).Resolve(() => ResolveTableViewCells(tableView.GetCellText));
        return;

        IVariant ResolveAvailableParameters()
        {
            var categories = tableView.Document.Settings.Categories;
            var variants = Variants.Values<IList<ElementId>>(categories.Size);
            foreach (Category category in categories)
            {
                variants.Add(TableView.GetAvailableParameters(tableView.Document, category.Id), category.Name);
            }

            return variants.Consume();
        }

        IVariant ResolveTableViewCells(Func<SectionType, int, int, string> selector)
        {
            var tableData = tableView switch
            {
                ViewSchedule viewSchedule => viewSchedule.GetTableData(),
                PanelScheduleView panelScheduleView => panelScheduleView.GetTableData(),
                _ => throw new NotSupportedException($"{tableView.GetType().FullName} is not supported in the current API version")
            };

            var sectionTypes = Enum.GetValues<SectionType>();
            var variants = Variants.Values<string>(sectionTypes.Length);
            foreach (var sectionType in sectionTypes)
            {
                var section = tableData.GetSectionData(sectionType);
                if (section is null) continue;

                for (var i = section.FirstRowNumber; i < section.LastRowNumber; i++)
                for (var j = section.FirstColumnNumber; j < section.LastColumnNumber; j++)
                {
                    var result = selector(sectionType, i, j);
                    variants.Add(result, $"{sectionType}, row {i}, column {j}: {result}");
                }
            }

            return variants.Consume();
        }
    }
}