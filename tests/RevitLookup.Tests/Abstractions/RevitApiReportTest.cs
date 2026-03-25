using System.Reflection;
using System.Text;
using Nice3point.TUnit.Revit;

namespace RevitLookup.Tests.Unit.Abstractions;

public abstract class RevitApiReportTest : RevitApiTest
{
    private static SourceIndex _sourceIndex = null!;
    private static readonly Dictionary<string, List<ApiMethodEntry>> EntriesCache = [];

    [Before(HookType.Assembly)]
    public static void SetupSourceIndex()
    {
        _sourceIndex = SourceIndex.Build(FindSourceDirectory());
    }

    protected List<ApiMethodEntry> BuildEntries(string assemblyName)
    {
        if (EntriesCache.TryGetValue(assemblyName, out var cached))
        {
            return cached;
        }

        var assembly = AppDomain.CurrentDomain.GetAssemblies().First(loadedAssembly => loadedAssembly.GetName().Name == assemblyName);

        var entries = ApiReportBuilder.GetUtilityTypes(assembly)
            .SelectMany(type => ApiReportBuilder.GetPublicStaticMethods(type)
                .Select(method => ApiReportBuilder.CreateEntry(type, method, _sourceIndex)))
            .ToList();

        EntriesCache[assemblyName] = entries;
        return entries;
    }

    protected static async Task AttachReportAsync(string content, string fileName)
    {
        await File.WriteAllTextAsync(fileName, content);

        TestContext.Current!.Output.AttachArtifact(
            fileName,
            displayName: fileName,
            description: "RevitAPI static methods with extension mapping");
    }

    private static string FindSourceDirectory()
    {
        var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".git")))
            {
                return Path.Combine(directory.FullName, "source", "RevitLookup", "Core", "Decomposition", "Descriptors");
            }

            directory = directory.Parent;
        }

        return string.Empty;
    }
}

public sealed record ApiMethodEntry
{
    public required string ReturnType { get; init; }
    public required string Method { get; init; }
    public required string Parameters { get; init; }
    public required IReadOnlyList<string> Descriptors { get; init; }
}

public sealed class SourceIndex
{
    private readonly List<(string FileName, string Content)> _entries;

    private SourceIndex(List<(string FileName, string Content)> entries)
    {
        _entries = entries;
    }

    public static SourceIndex Build(string directory)
    {
        if (!Directory.Exists(directory))
        {
            return new SourceIndex([]);
        }

        var entries = Directory
            .EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories)
            .Select(filePath => (FileName: Path.GetFileName(filePath), Content: File.ReadAllText(filePath)))
            .ToList();

        return new SourceIndex(entries);
    }

    public IReadOnlyList<string> FindDescriptors(string qualifiedName)
    {
        return _entries
            .Where(entry => entry.Content.Contains(qualifiedName, StringComparison.Ordinal))
            .Select(entry => entry.FileName)
            .ToList();
    }
}

public static class ApiReportBuilder
{
    public static IEnumerable<Type> GetUtilityTypes(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(IsUtilityType)
            .OrderBy(type => type.Name);
    }

    public static IEnumerable<MethodInfo> GetPublicStaticMethods(Type type)
    {
        return type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(method => !method.IsSpecialName)
            .OrderBy(method => method.Name);
    }

    public static ApiMethodEntry CreateEntry(Type type, MethodInfo method, SourceIndex sourceIndex)
    {
        var qualifiedName = $"{type.Name}.{method.Name}";
        var parameters = string.Join(", ", method.GetParameters().Select(parameter => $"{parameter.ParameterType.Name} {parameter.Name}"));

        return new ApiMethodEntry
        {
            ReturnType = method.ReturnType.Name,
            Method = qualifiedName,
            Parameters = parameters,
            Descriptors = sourceIndex.FindDescriptors(qualifiedName),
        };
    }

    public static string GenerateMarkdown(IEnumerable<ApiMethodEntry> entries)
    {
        var builder = new StringBuilder();
        builder.AppendLine("| Return type | Method | Parameters | Implementation |");
        builder.AppendLine("| ----------- | ------ | ---------- | -------------- |");

        foreach (var entry in entries)
        {
            builder
                .Append("| ").Append(entry.ReturnType)
                .Append(" | ").Append(entry.Method)
                .Append(" | ").Append(entry.Parameters)
                .Append(" | ").Append(string.Join(", ", entry.Descriptors))
                .AppendLine(" |");
        }

        return builder.ToString();
    }

    private static bool IsUtilityType(Type type)
    {
        if (!type.IsPublic || !type.IsClass)
        {
            return false;
        }

        var staticMethods = type
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(method => !method.IsSpecialName)
            .ToList();

        if (staticMethods.Count == 0)
        {
            return false;
        }

        return IsStaticClass(type) || IsGeneratedUtilityWrapper(type);
    }

    private static bool IsStaticClass(Type type)
    {
        return type is { IsAbstract: true, IsSealed: true };
    }

    private static bool IsGeneratedUtilityWrapper(Type type)
    {
        if (IsStaticClass(type))
        {
            return false;
        }

        var hasPublicConstructors = type
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Length > 0;

        if (hasPublicConstructors)
        {
            return false;
        }

        var hasUnexpectedInstanceMethods = type
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(method => !method.IsSpecialName)
            .Any(method => method.Name != nameof(IDisposable.Dispose));

        if (hasUnexpectedInstanceMethods)
        {
            return false;
        }

        var hasCreateMethod = type
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(method => !method.IsSpecialName)
            .Any(method => method.Name == "Create");

        if (hasCreateMethod)
        {
            return false;
        }

        var hasUnexpectedProperties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(property => property.GetIndexParameters().Length == 0)
            .Any(property => property.Name != "IsValidObject");

        if (hasUnexpectedProperties)
        {
            return false;
        }

        return true;
    }
}