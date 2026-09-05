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

using System.Collections.Concurrent;
using System.Reflection;
using Nice3point.TUnit.Revit;
using RevitLookup.Tests.Coverage.Discovery;
using RevitLookup.Tests.Coverage.Models;

namespace RevitLookup.Tests.Abstractions;

/// <summary>
///     Supplies report tests with the reflected surface of an assembly, annotated with the descriptor source files naming each member.
/// </summary>
public abstract class RevitApiReportTest : RevitApiTest
{
    private const string DescriptorDirectory = @"source\RevitLookup\Core\Decomposition\Descriptors";

    private static readonly ConcurrentDictionary<Assembly, IReadOnlyList<ApiMethodRow>> UtilityMethodRowsByAssembly = new();
    private static readonly ConcurrentDictionary<Assembly, IReadOnlyList<ApiEnumerableRow>> EnumerableRowsByAssembly = new();

    private static SourceFileIndex _descriptorSourceIndex = null!;

    /// <summary>
    ///     Reads the descriptor sources once per test session.
    /// </summary>
    [Before(HookType.Assembly)]
    public static void BuildDescriptorSourceIndex()
    {
        _descriptorSourceIndex = SourceFileIndex.Build(FindDescriptorSourceDirectory());
    }

    /// <summary>
    ///     Scans the assembly once per test session and returns one row per public static utility method in discovery order.
    /// </summary>
    /// <param name="assembly">The assembly to report on.</param>
    protected static IReadOnlyList<ApiMethodRow> GetUtilityMethodRows(Assembly assembly)
    {
        return UtilityMethodRowsByAssembly.GetOrAdd(assembly, static target => ApiUtilityScanner.ScanUtilityMethods(target, _descriptorSourceIndex));
    }

    /// <summary>
    ///     Scans the assembly once per test session and returns one row per enumerable in discovery order.
    /// </summary>
    /// <param name="assembly">The assembly to report on.</param>
    protected static IReadOnlyList<ApiEnumerableRow> GetEnumerableRows(Assembly assembly)
    {
        return EnumerableRowsByAssembly.GetOrAdd(assembly, static target => ApiEnumerableScanner.ScanEnumerables(target, _descriptorSourceIndex));
    }

    /// <summary>
    ///     Walks up from the test output directory to the descriptor sources in the repository.
    /// </summary>
    /// <exception cref="DirectoryNotFoundException">No repository checkout encloses the test output directory.</exception>
    private static string FindDescriptorSourceDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var descriptorDirectory = Path.Combine(directory.FullName, DescriptorDirectory);
            if (Directory.Exists(descriptorDirectory))
            {
                return descriptorDirectory;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException($"No '{DescriptorDirectory}' directory was found above '{AppContext.BaseDirectory}'.");
    }
}
