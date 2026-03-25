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
using System.Reflection;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class TableSectionDataDescriptor(TableSectionData tableSectionData) : Descriptor, IDescriptorResolver, IDescriptorResolver<Document>
{
    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(TableSectionData.AllowOverrideCellStyle) => () => VariantsResolver.ResolveTableCells(tableSectionData.NumberOfRows, tableSectionData.NumberOfColumns, tableSectionData.AllowOverrideCellStyle),
            nameof(TableSectionData.CanInsertColumn) => () => VariantsResolver.ResolveIndex(tableSectionData.NumberOfColumns, tableSectionData.CanInsertColumn),
            nameof(TableSectionData.CanInsertRow) => () => VariantsResolver.ResolveIndex(tableSectionData.NumberOfRows, tableSectionData.CanInsertRow),
            nameof(TableSectionData.CanRemoveColumn) => () => VariantsResolver.ResolveIndex(tableSectionData.NumberOfColumns, tableSectionData.CanRemoveColumn),
            nameof(TableSectionData.CanRemoveRow) => () => VariantsResolver.ResolveIndex(tableSectionData.NumberOfRows, tableSectionData.CanRemoveRow),
            nameof(TableSectionData.GetCellCalculatedValue) when parameters.Length == 1 => () => VariantsResolver.ResolveIndex(tableSectionData.NumberOfColumns, tableSectionData.GetCellCalculatedValue),
            nameof(TableSectionData.GetCellCalculatedValue) when parameters.Length == 2 => () => VariantsResolver.ResolveTableCells(tableSectionData.NumberOfRows, tableSectionData.NumberOfColumns, tableSectionData.GetCellCalculatedValue),
            nameof(TableSectionData.GetCellCombinedParameters) when parameters.Length == 1 => () => VariantsResolver.ResolveIndex(tableSectionData.NumberOfColumns, tableSectionData.GetCellCombinedParameters),
            nameof(TableSectionData.GetCellCombinedParameters) when parameters.Length == 2 => () => VariantsResolver.ResolveTableCells(tableSectionData.NumberOfRows, tableSectionData.NumberOfColumns, tableSectionData.GetCellCombinedParameters),
            nameof(TableSectionData.GetCellSpec) => ResolveCellSpec,
            nameof(TableSectionData.GetCellText) => () => VariantsResolver.ResolveTableCells(tableSectionData.NumberOfRows, tableSectionData.NumberOfColumns, tableSectionData.GetCellText),
            nameof(TableSectionData.GetCellType) when parameters.Length == 1 => () => VariantsResolver.ResolveIndex(tableSectionData.NumberOfColumns, tableSectionData.GetCellType),
            nameof(TableSectionData.GetCellType) when parameters.Length == 2 => () => VariantsResolver.ResolveTableCells(tableSectionData.NumberOfRows, tableSectionData.NumberOfColumns, tableSectionData.GetCellType),
            nameof(TableSectionData.GetColumnWidth) => ResolveColumnWidth,
            nameof(TableSectionData.GetColumnWidthInPixels) => ResolveColumnWidthInPixels,
            nameof(TableSectionData.GetMergedCell) => () => VariantsResolver.ResolveTableCells(tableSectionData.NumberOfRows, tableSectionData.NumberOfColumns, tableSectionData.GetMergedCell),
            nameof(TableSectionData.GetRowHeight) => ResolveRowHeight,
            nameof(TableSectionData.GetRowHeightInPixels) => ResolveRowHeightInPixels,
            nameof(TableSectionData.GetTableCellStyle) => () => VariantsResolver.ResolveTableCells(tableSectionData.NumberOfRows, tableSectionData.NumberOfColumns, tableSectionData.GetTableCellStyle),
            nameof(TableSectionData.IsCellFormattable) => () => VariantsResolver.ResolveTableCells(tableSectionData.NumberOfRows, tableSectionData.NumberOfColumns, tableSectionData.IsCellFormattable),
            nameof(TableSectionData.IsCellOverridden) when parameters.Length == 1 => () => VariantsResolver.ResolveIndex(tableSectionData.NumberOfColumns, tableSectionData.IsCellOverridden),
            nameof(TableSectionData.IsCellOverridden) when parameters.Length == 2 => () => VariantsResolver.ResolveTableCells(tableSectionData.NumberOfRows, tableSectionData.NumberOfColumns, tableSectionData.IsCellOverridden),
            nameof(TableSectionData.IsValidColumnNumber) => () => VariantsResolver.ResolveIndex(tableSectionData.NumberOfColumns, tableSectionData.IsValidColumnNumber),
            nameof(TableSectionData.IsValidRowNumber) => () => VariantsResolver.ResolveIndex(tableSectionData.NumberOfRows, tableSectionData.IsValidRowNumber),
            nameof(TableSectionData.RefreshData) => Variants.Disabled,
            _ => null
        };

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

    Func<Document, IVariant>? IDescriptorResolver<Document>.Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(TableSectionData.GetCellCategoryId) when parameters.Length == 1 => ResolveCellCategoryIdForColumns,
            nameof(TableSectionData.GetCellCategoryId) when parameters.Length == 2 => ResolveCellCategoryIdForTable,
            nameof(TableSectionData.GetCellFormatOptions) when parameters.Length == 2 => ResolveCellFormatOptionsForColumns,
            nameof(TableSectionData.GetCellFormatOptions) when parameters.Length == 3 => ResolveCellFormatOptionsForTable,
            nameof(TableSectionData.GetCellParamId) when parameters.Length == 1 => ResolveCellParamIdForColumns,
            nameof(TableSectionData.GetCellParamId) when parameters.Length == 2 => ResolveCellParamIdForTable,
            _ => null
        };

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
}