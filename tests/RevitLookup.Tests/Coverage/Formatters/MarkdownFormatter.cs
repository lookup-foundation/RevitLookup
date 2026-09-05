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

using System.Text;
using RevitLookup.Tests.Coverage.Models;

namespace RevitLookup.Tests.Coverage.Formatters;

/// <summary>
///     Provides extension methods for sequences of <see cref="ApiMethodRow" /> and <see cref="ApiEnumerableRow" /> to render them as Markdown.
/// </summary>
internal static class MarkdownFormatter
{
    private const string PresentMark = "yes";
    private const string AbsentMark = "no";

    private const string CoveredMark = "[x]";
    private const string MissingMark = "[ ]";
    private const string IteratedMark = "[-]";

    private const string CoverageLegend =
        $"""
         `EnumerableDescriptor` reports whether a collection contains any elements. It reads `IsEmpty` or `Count` on the types its switch matches, and creates an enumerator for every other type.

         Descriptor arm:

         - `{CoveredMark}` an arm matches the type
         - `{MissingMark}` no arm matches the type, and the type carries `IsEmpty` or `Count`: an arm reading it spares the enumerator
         - `{IteratedMark}` no arm matches the type, and the type carries neither property

         Matched type: the type named by the matching arm. A base type or an interface there marks an arm reaching the row through the hierarchy.
         """;

    /// <param name="rows">The report rows in presentation order.</param>
    extension(IEnumerable<ApiMethodRow> rows)
    {
        /// <summary>
        ///     Renders the rows as a Markdown table.
        /// </summary>
        /// <returns>The rows rendered as a Markdown table.</returns>
        [Pure]
        public string ToMarkdownTable()
        {
            var builder = new StringBuilder();
            builder.AppendLine("| Return type | Method | Parameters | Descriptors |");
            builder.AppendLine("| ----------- | ------ | ---------- | ----------- |");

            foreach (var row in rows)
            {
                builder
                    .Append("| ").Append(row.ReturnType)
                    .Append(" | ").Append(row.QualifiedName)
                    .Append(" | ").Append(row.Parameters)
                    .Append(" | ").Append(string.Join(", ", row.DescriptorFiles))
                    .AppendLine(" |");
            }

            return builder.ToString();
        }
    }

    /// <param name="rows">The report rows in presentation order.</param>
    extension(IEnumerable<ApiEnumerableRow> rows)
    {
        /// <summary>
        ///     Renders the rows as a Markdown table, led by a legend of the column marks.
        /// </summary>
        /// <returns>The rows rendered as a Markdown table, preceded by a legend of the column marks.</returns>
        [Pure]
        public string ToMarkdownTable()
        {
            var builder = new StringBuilder();
            builder.AppendLine(CoverageLegend);
            builder.AppendLine();
            builder.AppendLine("| Descriptor arm | Kind | Enumerable | Namespace | Element | APIObject | IsEmpty | Count | Matched type |");
            builder.AppendLine("| -------------- | ---- | ---------- | --------- | ------- | --------- | ------- | ----- | ------------ |");

            foreach (var row in rows)
            {
                builder
                    .Append("| ").Append(FormatCoverage(row.Coverage))
                    .Append(" | ").Append(row.Kind)
                    .Append(" | ").Append(row.TypeName)
                    .Append(" | ").Append(row.Namespace)
                    .Append(" | ").Append(row.ElementType)
                    .Append(" | ").Append(FormatFlag(row.IsApiObject))
                    .Append(" | ").Append(FormatFlag(row.HasIsEmpty))
                    .Append(" | ").Append(FormatFlag(row.HasCount))
                    .Append(" | ").Append(row.DescriptorArm)
                    .AppendLine(" |");
            }

            return builder.ToString();
        }
    }

    private static string FormatFlag(bool value)
    {
        return value ? PresentMark : AbsentMark;
    }

    private static string FormatCoverage(ApiEnumerableCoverage coverage)
    {
        return coverage switch
        {
            ApiEnumerableCoverage.Covered => CoveredMark,
            ApiEnumerableCoverage.Missing => MissingMark,
            _ => IteratedMark
        };
    }
}
