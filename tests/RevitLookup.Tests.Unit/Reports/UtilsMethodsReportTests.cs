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
using Nice3point.TUnit.Revit.Executors;
using TUnit.Core.Executors;

namespace RevitLookup.Tests.Unit.Reports;

public sealed class UtilsMethodsReportTests : RevitApiTest
{
    [Test]
    [TestExecutor<RevitThreadExecutor>]
    public async Task Report_RevitAPI_UtilsMethods()
    {
        var sourceIndex = BuildSourceIndex();
        var types = GetRevitTypes();
        var methods = new List<StaticMethodEntry>();

        foreach (var type in types)
        {
            var staticMethods = GetQualifyingMethods(type);
            if (staticMethods.Count == 0) continue;

            foreach (var method in staticMethods)
            {
                var entry = CreateMethodEntry(type, method, sourceIndex);
                methods.Add(entry);
            }
        }

        var report = GenerateMarkdownReport(methods);
        await SaveAndAttachReportAsync(report);
    }

    private async Task SaveAndAttachReportAsync(string content)
    {
        var reportPath = $"RevitAPI-Utils-{Application.VersionNumber}.md";
        await File.WriteAllTextAsync(reportPath, content);

        TestContext.Current!.Output.AttachArtifact(
            reportPath,
            displayName: "Static Methods Report",
            description: "RevitAPI static methods with extension mapping"
        );
    }

    private static SourceIndex BuildSourceIndex()
    {
        var descriptorsDirectory = FindDescriptorsDirectory();
        if (descriptorsDirectory is null) return new SourceIndex([]);

        var entries = new List<SourceFileEntry>();

        foreach (var filePath in Directory.EnumerateFiles(descriptorsDirectory, "*.cs", SearchOption.AllDirectories))
        {
            var content = File.ReadAllText(filePath);
            var fileName = Path.GetFileName(filePath);

            entries.Add(new SourceFileEntry
            {
                FileName = fileName,
                Content = content,
            });
        }

        return new SourceIndex(entries);
    }

    private static string? FindDescriptorsDirectory()
    {
        var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        while (directory is not null)
        {
            var rootFolder = Path.Combine(directory.FullName, ".git");
            if (Directory.Exists(rootFolder))
            {
                return Path.Combine(directory.FullName, "source", "RevitLookup", "Core", "Decomposition", "Descriptors");
            }

            directory = directory.Parent;
        }

        return null;
    }

    private static IEnumerable<Type> GetRevitTypes()
    {
        var revitApiAssembly = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "RevitAPI");

        return revitApiAssembly.GetTypes()
            .Where(IsUtilityRevitType)
            .OrderBy(type => type.Name);
    }

    private static bool IsUtilityRevitType(Type type)
    {
        if (!IsPublicClass(type)) return false;
        if (!HasPublicDeclaredStaticMethods(type)) return false;

        return IsStaticClass(type) || IsGeneratedUtilityWrapper(type);
    }

    private static bool IsPublicClass(Type type) => type is {IsPublic: true, IsClass: true};

    private static bool IsStaticClass(Type type) => type is {IsAbstract: true, IsSealed: true};

    private static bool HasPublicDeclaredStaticMethods(Type type)
    {
        return type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Any(method => !method.IsSpecialName);
    }

    private static bool IsGeneratedUtilityWrapper(Type type)
    {
        if (IsStaticClass(type)) return false;
        if (HasPublicDeclaredInstanceConstructors(type)) return false;
        if (HasUnexpectedPublicInstanceMethods(type)) return false;
        if (HasUnexpectedPublicStaticMethods(type)) return false;
        if (HasUnexpectedPublicInstanceProperties(type)) return false;

        return true;
    }

    private static bool HasPublicDeclaredInstanceConstructors(Type type)
    {
        return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length > 0;
    }

    private static bool HasUnexpectedPublicInstanceMethods(Type type)
    {
        return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(method => !method.IsSpecialName)
            .Any(method => method.Name != nameof(IDisposable.Dispose));
    }

    private static bool HasUnexpectedPublicStaticMethods(Type type)
    {
        return type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(method => !method.IsSpecialName)
            .Any(method => method.Name == "Create");
    }

    private static bool HasUnexpectedPublicInstanceProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(property => property.GetIndexParameters().Length == 0)
            .Any(property => property.Name != "IsValidObject");
    }

    private static List<MethodInfo> GetQualifyingMethods(Type type)
    {
        return type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(method => !method.IsSpecialName)
            .OrderBy(method => method.Name)
            .ToList();
    }

    private static StaticMethodEntry CreateMethodEntry(Type type, MethodInfo method, SourceIndex sourceIndex)
    {
        var parameters = method.GetParameters();

        var returnType = method.ReturnType.Name;
        var qualifiedName = $"{type.Name}.{method.Name}";
        var descriptors = sourceIndex.FindDescriptors(qualifiedName);
        var parameterSignature = string.Join(", ", parameters.Select(parameter => $"{parameter.ParameterType.Name} {parameter.Name}"));

        return new StaticMethodEntry
        {
            ReturnType = returnType,
            Method = qualifiedName,
            Parameters = parameterSignature,
            Descriptors = descriptors,
        };
    }

    private static string GenerateMarkdownReport(List<StaticMethodEntry> methods)
    {
        var outputBuilder = new StringBuilder();
        outputBuilder.AppendLine("| Return type | Method | Parameters | Implementation |");
        outputBuilder.AppendLine("| ----------- | ------ | ---------- | -------------- |");

        foreach (var entry in methods)
        {
            outputBuilder
                .Append("| ")
                .Append(entry.ReturnType)
                .Append(" | ")
                .Append(entry.Method)
                .Append(" | ")
                .Append(entry.Parameters)
                .Append(" | ")
                .Append(string.Join(", ", entry.Descriptors))
                .AppendLine(" |");
        }

        return outputBuilder.ToString();
    }

    private sealed record StaticMethodEntry
    {
        public required string ReturnType { get; init; }
        public required string Method { get; init; }
        public required string Parameters { get; init; }
        public required List<string> Descriptors { get; init; }
    }

    private sealed record SourceFileEntry
    {
        public required string FileName { get; init; }
        public required string Content { get; init; }
    }

    private sealed class SourceIndex(List<SourceFileEntry> entries)
    {
        public List<string> FindDescriptors(string qualifiedName)
        {
            var results = new List<string>();

            foreach (var entry in entries)
            {
                if (entry.Content.Contains(qualifiedName, StringComparison.Ordinal))
                {
                    results.Add(entry.FileName);
                }
            }

            return results;
        }
    }
}