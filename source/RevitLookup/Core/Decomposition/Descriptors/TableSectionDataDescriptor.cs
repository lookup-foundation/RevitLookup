// Copyright (column) Lookup Foundation and Contributors
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

using System.Globalization;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using RevitLookup.Common.Extensions;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class TableSectionDataDescriptor(TableSectionData tableSectionData) : ResolvingDescriptor, IDescriptorConfigurator, IDescriptorConfigurator<Document>
{
    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(TableSectionData.AllowOverrideCellStyle)).Resolve(() => ResolveTableCells(tableSectionData.NumberOfRows, tableSectionData.NumberOfColumns, tableSectionData.AllowOverrideCellStyle));
        configuration.Member(nameof(TableSectionData.CanInsertColumn)).Resolve(() => ResolveRange(tableSectionData.NumberOfColumns, tableSectionData.CanInsertColumn));
        configuration.Member(nameof(TableSectionData.CanInsertRow)).Resolve(() => ResolveRange(tableSectionData.NumberOfRows, tableSectionData.CanInsertRow));
        configuration.Member(nameof(TableSectionData.CanRemoveColumn)).Resolve(() => ResolveRange(tableSectionData.NumberOfColumns, tableSectionData.CanRemoveColumn));
        configuration.Member(nameof(TableSectionData.CanRemoveRow)).Resolve(() => ResolveRange(tableSectionData.NumberOfRows, tableSectionData.CanRemoveRow));
        configuration.Member(nameof(TableSectionData.GetCellCalculatedValue)).When(parameters => parameters.Length == 1).Resolve(() => ResolveRange(tableSectionData.NumberOfColumns, tableSectionData.GetCellCalculatedValue));
        configuration.Member(nameof(TableSectionData.GetCellCalculatedValue)).When(parameters => parameters.Length == 2).Resolve(() => ResolveTableCells(tableSectionData.NumberOfRows, tableSectionData.NumberOfColumns, tableSectionData.GetCellCalculatedValue));
        configuration.Member(nameof(TableSectionData.GetCellCombinedParameters)).When(parameters => parameters.Length == 1).Resolve(() => ResolveRange(tableSectionData.NumberOfColumns, tableSectionData.GetCellCombinedParameters));
        configuration.Member(nameof(TableSectionData.GetCellCombinedParameters)).When(parameters => parameters.Length == 2).Resolve(() => ResolveTableCells(tableSectionData.NumberOfRows, tableSectionData.NumberOfColumns, tableSectionData.GetCellCombinedParameters));
        configuration.Member(nameof(TableSectionData.GetCellSpec)).Resolve(ResolveCellSpec);
        configuration.Member(nameof(TableSectionData.GetCellText)).Resolve(() => ResolveTableCells(tableSectionData.NumberOfRows, tableSectionData.NumberOfColumns, tableSectionData.GetCellText));
        configuration.Member(nameof(TableSectionData.GetCellType)).When(parameters => parameters.Length == 1).Resolve(() => ResolveRange(tableSectionData.NumberOfColumns, tableSectionData.GetCellType));
        configuration.Member(nameof(TableSectionData.GetCellType)).When(parameters => parameters.Length == 2).Resolve(() => ResolveTableCells(tableSectionData.NumberOfRows, tableSectionData.NumberOfColumns, tableSectionData.GetCellType));
        configuration.Member(nameof(TableSectionData.GetColumnWidth)).Resolve(ResolveColumnWidth);
        configuration.Member(nameof(TableSectionData.GetColumnWidthInPixels)).Resolve(ResolveColumnWidthInPixels);
        configuration.Member(nameof(TableSectionData.GetMergedCell)).Resolve(() => ResolveTableCells(tableSectionData.NumberOfRows, tableSectionData.NumberOfColumns, tableSectionData.GetMergedCell));
        configuration.Member(nameof(TableSectionData.GetRowHeight)).Resolve(ResolveRowHeight);
        configuration.Member(nameof(TableSectionData.GetRowHeightInPixels)).Resolve(ResolveRowHeightInPixels);
        configuration.Member(nameof(TableSectionData.GetTableCellStyle)).Resolve(() => ResolveTableCells(tableSectionData.NumberOfRows, tableSectionData.NumberOfColumns, tableSectionData.GetTableCellStyle));
        configuration.Member(nameof(TableSectionData.IsCellFormattable)).Resolve(() => ResolveTableCells(tableSectionData.NumberOfRows, tableSectionData.NumberOfColumns, tableSectionData.IsCellFormattable));
        configuration.Member(nameof(TableSectionData.IsCellOverridden)).When(parameters => parameters.Length == 1).Resolve(() => ResolveRange(tableSectionData.NumberOfColumns, tableSectionData.IsCellOverridden));
        configuration.Member(nameof(TableSectionData.IsCellOverridden)).When(parameters => parameters.Length == 2).Resolve(() => ResolveTableCells(tableSectionData.NumberOfRows, tableSectionData.NumberOfColumns, tableSectionData.IsCellOverridden));
        configuration.Member(nameof(TableSectionData.IsValidColumnNumber)).Resolve(() => ResolveRange(tableSectionData.NumberOfColumns, tableSectionData.IsValidColumnNumber));
        configuration.Member(nameof(TableSectionData.IsValidRowNumber)).Resolve(() => ResolveRange(tableSectionData.NumberOfRows, tableSectionData.IsValidRowNumber));
        configuration.Member(nameof(TableSectionData.RefreshData)).Defer();
        return;

        IVariant ResolveCellSpec()
        {
            var rowsNumber = tableSectionData.NumberOfRows;
            var columnsNumber = tableSectionData.NumberOfColumns;
            var variants = Variants.Values<ForgeTypeId>(rowsNumber * columnsNumber);
            for (var i = 0; i < columnsNumber; i++)
            {
                for (var j = 0; j < rowsNumber; j++)
                {
                    var result = tableSectionData.GetCellSpec(j, i);
                    if (result.Empty()) continue;

                    variants.Add(result, $"Row {j}, Column {i}: {result.ToSpecLabel()}");
                }
            }

            return variants.Consume();
        }

        IVariant ResolveColumnWidth()
        {
            var count = tableSectionData.NumberOfColumns;
            var variants = Variants.Values<double>(count);
            for (var i = 0; i < count; i++)
            {
                var result = tableSectionData.GetColumnWidth(i);
                variants.Add(result, $"{i}: {result.ToString(CultureInfo.InvariantCulture)}");
            }

            return variants.Consume();
        }

        IVariant ResolveColumnWidthInPixels()
        {
            var count = tableSectionData.NumberOfColumns;
            var variants = Variants.Values<int>(count);
            for (var i = 0; i < count; i++)
            {
                var result = tableSectionData.GetColumnWidthInPixels(i);
                variants.Add(result, $"{i}: {result.ToString(CultureInfo.InvariantCulture)}");
            }

            return variants.Consume();
        }

        IVariant ResolveRowHeight()
        {
            var count = tableSectionData.NumberOfRows;
            var variants = Variants.Values<double>(count);
            for (var i = 0; i < count; i++)
            {
                var result = tableSectionData.GetRowHeight(i);
                variants.Add(result, $"{i}: {result.ToString(CultureInfo.InvariantCulture)}");
            }

            return variants.Consume();
        }

        IVariant ResolveRowHeightInPixels()
        {
            var count = tableSectionData.NumberOfRows;
            var variants = Variants.Values<int>(count);
            for (var i = 0; i < count; i++)
            {
                var result = tableSectionData.GetRowHeightInPixels(i);
                variants.Add(result, $"{i}: {result.ToString(CultureInfo.InvariantCulture)}");
            }

            return variants.Consume();
        }
    }

    void IDescriptorConfigurator<Document>.Configure(IMemberConfigurator<Document> configuration)
    {
        configuration.Member(nameof(TableSectionData.GetCellCategoryId)).When(parameters => parameters.Length == 1).Resolve(ResolveCellCategoryIdForColumns);
        configuration.Member(nameof(TableSectionData.GetCellCategoryId)).When(parameters => parameters.Length == 2).Resolve(ResolveCellCategoryIdForTable);
        configuration.Member(nameof(TableSectionData.GetCellFormatOptions)).When(parameters => parameters.Length == 2).Resolve(ResolveCellFormatOptionsForColumns);
        configuration.Member(nameof(TableSectionData.GetCellFormatOptions)).When(parameters => parameters.Length == 3).Resolve(ResolveCellFormatOptionsForTable);
        configuration.Member(nameof(TableSectionData.GetCellParamId)).When(parameters => parameters.Length == 1).Resolve(ResolveCellParamIdForColumns);
        configuration.Member(nameof(TableSectionData.GetCellParamId)).When(parameters => parameters.Length == 2).Resolve(ResolveCellParamIdForTable);

        IVariant ResolveCellCategoryIdForColumns(Document context)
        {
            var columnsNumber = tableSectionData.NumberOfColumns;
            var variants = Variants.Values<ElementId>(columnsNumber);
            for (var i = 0; i < columnsNumber; i++)
            {
                var result = tableSectionData.GetCellCategoryId(i);
                if (result == ElementId.InvalidElementId) continue;

                var category = Category.GetCategory(context, result);
                if (category is null) continue;

                variants.Add(result, $"Column {i}: {category.Name}");
            }

            return variants.Consume();
        }

        IVariant ResolveCellCategoryIdForTable(Document context)
        {
            var rowsNumber = tableSectionData.NumberOfRows;
            var columnsNumber = tableSectionData.NumberOfColumns;
            var variants = Variants.Values<ElementId>(rowsNumber * columnsNumber);
            for (var i = 0; i < columnsNumber; i++)
            {
                for (var j = 0; j < rowsNumber; j++)
                {
                    var result = tableSectionData.GetCellCategoryId(j, i);
                    if (result == ElementId.InvalidElementId) continue;

                    var category = Category.GetCategory(context, result);
                    if (category is null) continue;

                    variants.Add(result, $"Row {j}, Column {i}: {category.Name}");
                }
            }

            return variants.Consume();
        }

        IVariant ResolveCellFormatOptionsForColumns(Document context)
        {
            var columnsNumber = tableSectionData.NumberOfColumns;
            var variants = Variants.Values<FormatOptions>(columnsNumber);
            for (var i = 0; i < columnsNumber; i++)
            {
                var result = tableSectionData.GetCellFormatOptions(i, context);
                variants.Add(result, $"Column {i}");
            }

            return variants.Consume();
        }

        IVariant ResolveCellFormatOptionsForTable(Document context)
        {
            var rowsNumber = tableSectionData.NumberOfRows;
            var columnsNumber = tableSectionData.NumberOfColumns;
            var variants = Variants.Values<FormatOptions>(rowsNumber * columnsNumber);
            for (var i = 0; i < columnsNumber; i++)
            {
                for (var j = 0; j < rowsNumber; j++)
                {
                    var result = tableSectionData.GetCellFormatOptions(j, i, context);
                    variants.Add(result, $"Row {j}, Column {i}");
                }
            }

            return variants.Consume();
        }

        IVariant ResolveCellParamIdForColumns(Document context)
        {
            var columnsNumber = tableSectionData.NumberOfColumns;
            var variants = Variants.Values<ElementId>(columnsNumber);
            for (var i = 0; i < columnsNumber; i++)
            {
                var result = tableSectionData.GetCellParamId(i);
                if (result != ElementId.InvalidElementId)
                {
                    var parameter = result.ToElement(context);
                    variants.Add(result, $"Column {i}: {parameter!.Name}");
                }
            }

            return variants.Consume();
        }

        IVariant ResolveCellParamIdForTable(Document context)
        {
            var rowsNumber = tableSectionData.NumberOfRows;
            var columnsNumber = tableSectionData.NumberOfColumns;
            var variants = Variants.Values<ElementId>(rowsNumber * columnsNumber);
            for (var i = 0; i < columnsNumber; i++)
            {
                for (var j = 0; j < rowsNumber; j++)
                {
                    var result = tableSectionData.GetCellParamId(j, i);
                    if (result == ElementId.InvalidElementId) continue;

                    var parameter = result.ToElement(context);
                    if (parameter is null) continue;

                    variants.Add(result, $"Row {j}, Column {i}: {parameter.Name}");
                }
            }

            return variants.Consume();
        }
    }
    
    private static IVariant ResolveTableCells<TResult>(int rows, int columns, Func<int, int, TResult> selector)
    {
        var variants = Variants.Values<TResult>(rows * columns);
        var simple = typeof(TResult).IsPrimitiveType();
        for (var i = 0; i < columns; i++)
        {
            for (var j = 0; j < rows; j++)
            {
                var result = selector(j, i);
                variants.Add(result, simple ? $"Row {j}, Column {i}: {result}" : $"Row {j}, Column {i}");
            }
        }

        return variants.Consume();
    }
}