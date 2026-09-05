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
using RevitLookup.Tests.Coverage.Models;

namespace RevitLookup.Tests.Coverage.Discovery;

/// <summary>
///     Discovers the public static utility methods an assembly declares.
/// </summary>
internal static class ApiUtilityScanner
{
    private const BindingFlags DeclaredStatic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredInstance = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private const string FactoryMethodName = "Create";
    private const string InteropValidityPropertyName = "IsValidObject";

    /// <summary>
    ///     Builds one report row per public static method of every utility type in the assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan.</param>
    /// <param name="sourceFileIndex">The index resolving which descriptor source files resolve each method.</param>
    /// <returns>One <see cref="ApiMethodRow" /> per public static method of every utility type in <paramref name="assembly" />.</returns>
    public static IReadOnlyList<ApiMethodRow> ScanUtilityMethods(Assembly assembly, SourceFileIndex sourceFileIndex)
    {
        return
        [
            .. assembly
                .GetLoadableTypes()
                .Where(IsUtilityType)
                .SelectMany(type => GetUtilityMethods(type).Select(method => CreateRow(type, method, sourceFileIndex)))
        ];
    }

    /// <summary>
    ///     Reports whether the type carries utility methods: a static class, or an interop class whose only
    ///     reachable members are static methods.
    /// </summary>
    private static bool IsUtilityType(Type type)
    {
        if (!type.IsVisible)
        {
            return false;
        }

        if (!type.IsClass)
        {
            return false;
        }

        if (!GetUtilityMethods(type).Any())
        {
            return false;
        }

        return IsStaticClass(type) || IsStaticOnlyInteropClass(type);
    }

    /// <summary>
    ///     Reports whether the type is a non-static class that cannot be constructed or used as an instance,
    ///     the shape the Revit API interop layer produces for its utility classes.
    /// </summary>
    private static bool IsStaticOnlyInteropClass(Type type)
    {
        if (IsStaticClass(type))
        {
            return false;
        }

        var hasPublicConstructors = type
            .GetConstructors(DeclaredInstance)
            .Length > 0;

        if (hasPublicConstructors)
        {
            return false;
        }

        var hasUnexpectedInstanceMethods = type
            .GetMethods(DeclaredInstance)
            .Where(method => !method.IsSpecialName)
            .Any(method => method.Name != nameof(IDisposable.Dispose));

        if (hasUnexpectedInstanceMethods)
        {
            return false;
        }

        var hasFactoryMethod = GetUtilityMethods(type)
            .Any(method => method.Name == FactoryMethodName);

        if (hasFactoryMethod)
        {
            return false;
        }

        var hasUnexpectedProperties = type
            .GetProperties(DeclaredInstance)
            .Where(property => property.GetIndexParameters().Length == 0)
            .Any(property => property.Name != InteropValidityPropertyName);

        if (hasUnexpectedProperties)
        {
            return false;
        }

        return true;
    }

    private static IEnumerable<MethodInfo> GetUtilityMethods(Type type)
    {
        return type
            .GetMethods(DeclaredStatic)
            .Where(method => !method.IsSpecialName);
    }

    private static bool IsStaticClass(Type type)
    {
        return type is { IsAbstract: true, IsSealed: true };
    }

    private static ApiMethodRow CreateRow(Type type, MethodInfo method, SourceFileIndex sourceFileIndex)
    {
        var qualifiedName = $"{type.FormatDeclarationName()}.{method.Name}";
        var parameters = string.Join(", ", method.GetParameters().Select(parameter => $"{parameter.ParameterType.FormatName()} {parameter.Name}"));

        return new ApiMethodRow
        {
            ReturnType = method.ReturnType.FormatName(),
            QualifiedName = qualifiedName,
            Parameters = parameters,
            DescriptorFiles = sourceFileIndex.FindReferencingFiles(qualifiedName)
        };
    }
}
