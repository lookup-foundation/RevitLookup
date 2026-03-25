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
using System.Text;
using Nice3point.TUnit.Revit;

namespace RevitLookup.Tests.Unit.Abstractions;

public abstract class RevitApiInheritanceReportTest : RevitApiTest
{
    private static readonly Dictionary<string, List<InheritanceEntry>> EntriesCache = [];

    protected List<InheritanceEntry> BuildEntries(string assemblyName, params Type[] baseTypes)
    {
        var cacheKey = $"{assemblyName}:{string.Join(",", baseTypes.Select(type => type.FullName))}";

        if (EntriesCache.TryGetValue(cacheKey, out var cached))
        {
            return cached;
        }

        var assembly = AppDomain.CurrentDomain.GetAssemblies().First(loadedAssembly => loadedAssembly.GetName().Name == assemblyName);

        var entries = InheritanceReportBuilder.GetDerivedTypes(assembly, baseTypes)
            .Select(type => InheritanceReportBuilder.CreateEntry(type, baseTypes))
            .ToList();

        EntriesCache[cacheKey] = entries;
        return entries;
    }

    protected static async Task AttachReportAsync(string content, string fileName)
    {
        await File.WriteAllTextAsync(fileName, content);

        TestContext.Current!.Output.AttachArtifact(
            fileName,
            displayName: fileName,
            description: "RevitAPI types inheritance report");
    }
}

public sealed record InheritanceEntry
{
    public required string TypeName { get; init; }
    public required string Namespace { get; init; }
    public required string BaseType { get; init; }
    public required IReadOnlyList<string> MatchedBases { get; init; }
}

public static class InheritanceReportBuilder
{
    public static IEnumerable<Type> GetDerivedTypes(Assembly assembly, IReadOnlyList<Type> baseTypes)
    {
        return assembly.GetTypes()
            .Where(type => type is { IsPublic: true, IsClass: true, IsAbstract: false})
            .Where(type => baseTypes.All(baseType => IsDerivedFrom(type, baseType)))
            .OrderBy(type => type.Name);
    }

    public static InheritanceEntry CreateEntry(Type type, IReadOnlyList<Type> baseTypes)
    {
        return new InheritanceEntry
        {
            TypeName = type.Name,
            Namespace = type.Namespace ?? string.Empty,
            BaseType = type.BaseType?.Name ?? string.Empty,
            MatchedBases = baseTypes
                .Where(baseType => IsDerivedFrom(type, baseType))
                .Select(baseType => baseType.Name)
                .ToList(),
        };
    }

    public static string GenerateMarkdown(IEnumerable<InheritanceEntry> entries)
    {
        var builder = new StringBuilder();
        builder.AppendLine("| Type | Namespace | Base Type | Matched Bases |");
        builder.AppendLine("| ---- | --------- | --------- | ------------- |");

        foreach (var entry in entries)
        {
            builder
                .Append("| ").Append(entry.TypeName)
                .Append(" | ").Append(entry.Namespace)
                .Append(" | ").Append(entry.BaseType)
                .Append(" | ").Append(string.Join(", ", entry.MatchedBases))
                .AppendLine(" |");
        }

        return builder.ToString();
    }

    private static bool IsDerivedFrom(Type type, Type baseType)
    {
        return baseType.IsInterface
            ? type.GetInterfaces().Contains(baseType)
            : IsSubclassOf(type, baseType);
    }

    private static bool IsSubclassOf(Type type, Type baseType)
    {
        var current = type.BaseType;

        while (current is not null)
        {
            if (current == baseType)
            {
                return true;
            }

            current = current.BaseType;
        }

        return false;
    }
}