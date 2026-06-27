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
using System.Windows;
using LookupEngine.Abstractions.Enums;
using RevitLookup.Abstractions.ObservableModels.Decomposition;

namespace RevitLookup.UI.Framework.Views.Decomposition;

public partial class SummaryViewBase
{
    /// <summary>
    ///     Create tree view tooltips
    /// </summary>
    private static void CreateTreeTooltip(ObservableDecomposedObject decomposedObject, FrameworkElement row)
    {
        var builder = new StringBuilder()
            .Append("Name: ")
            .AppendLine(decomposedObject.Name)
            .Append("Type: ")
            .AppendLine(decomposedObject.TypeName)
            .Append("Full type: ")
            .Append(decomposedObject.TypeFullName);

        if (decomposedObject.Description is not null)
        {
            builder.AppendLine()
                .Append("Description: ")
                .Append(decomposedObject.Description);
        }

        row.ToolTip = builder.ToString();
    }

    /// <summary>
    ///     Create tree view tooltips
    /// </summary>
    private static void CreateTreeTooltip(ObservableDecomposedObjectsGroup decomposedGroup, FrameworkElement row)
    {
        row.ToolTip = new StringBuilder()
            .Append("Type: ")
            .AppendLine(decomposedGroup.GroupName)
            .Append("Items: ")
            .Append(decomposedGroup.GroupItems.Count)
            .ToString();
    }

    /// <summary>
    ///     Create data grid tooltips
    /// </summary>
    private static void CreateGridRowTooltip(ObservableDecomposedMember member, FrameworkElement row)
    {
        var builder = new StringBuilder();

        if ((member.MemberAttributes & MemberAttributes.Private) != 0) builder.Append("Private ");
        if ((member.MemberAttributes & MemberAttributes.Static) != 0) builder.Append("Static ");
        if ((member.MemberAttributes & MemberAttributes.Property) != 0) builder.Append("Property: ");
        if ((member.MemberAttributes & MemberAttributes.Extension) != 0) builder.Append("Extension: ");
        if ((member.MemberAttributes & MemberAttributes.Method) != 0) builder.Append("Method: ");
        if ((member.MemberAttributes & MemberAttributes.Event) != 0) builder.Append("Event: ");
        if ((member.MemberAttributes & MemberAttributes.Field) != 0) builder.Append("Field: ");

        builder.Append(member.Name);

        if (member.EvaluationPolicy != MemberEvaluationPolicy.Evaluated)
        {
            row.ToolTip = builder.ToString();
            return;
        }

        builder.AppendLine()
            .Append("Type: ")
            .AppendLine(member.Value.TypeName)
            .Append("Full type: ")
            .Append(member.Value.TypeFullName);

        if (member.Value.RawValue is not null)
        {
            builder.AppendLine()
                .Append("Value: ")
                .Append(member.Value.Name);
        }

        if (member.Value.Description is not null)
        {
            builder.AppendLine()
                .Append("Description: ")
                .Append(member.Value.Description);
        }

        if (member.ComputationTime > 0)
        {
            builder.AppendLine()
                .Append("Time: ")
                .Append(member.ComputationTime)
                .Append(" ms");
        }

        if (member.AllocatedBytes > 0)
        {
            builder.AppendLine()
                .Append("Allocated: ")
                .Append(member.AllocatedBytes)
                .Append(" bytes");
        }

        row.ToolTip = builder.ToString();
    }
}