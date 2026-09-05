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

using System.Collections;
using System.Reflection;
using RevitLookup.Tests.Coverage.Models;

namespace RevitLookup.Tests.Coverage.Discovery;

/// <summary>
///     Discovers the enumerables an assembly exposes, and for each one the properties telling whether it contains any elements.
/// </summary>
/// <remarks>
///     The interop layer mirrors the native C++ containers: iteration runs through an iterator factory, an entry key lives
///     on the concrete iterator, and the collection contract stops at the non-generic <see cref="IEnumerable" />.
/// </remarks>
internal static class ApiEnumerableScanner
{
    private const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;

    private const string KeyPropertyName = nameof(IDictionaryEnumerator.Key);
    private const string CountPropertyName = nameof(ICollection.Count);

    /// <summary>
    ///     The name the CLR gives an indexer.
    /// </summary>
    private const string IndexerPropertyName = "Item";

    /// <summary>
    ///     The iteration entry point of the native C++ API, kept in the managed wrapper next to <c>GetEnumerator</c>.
    /// </summary>
    private const string IteratorFactoryName = "ForwardIterator";

    /// <summary>
    ///     The element count check of the native C++ API, kept in the managed wrapper next to <c>Size</c>.
    /// </summary>
    private const string IsEmptyPropertyName = "IsEmpty";

    /// <summary>
    ///     The descriptor whose switch reads <c>IsEmpty</c> and <c>Count</c> in place of creating an enumerator.
    /// </summary>
    private const string DescriptorFileName = "EnumerableDescriptor.cs";

    /// <summary>
    ///     The insertion methods of the native C++ API, kept in the managed wrapper in place of <c>Add</c>.
    /// </summary>
    private static readonly string[] InsertionMethodNames = ["Insert", "Append"];

    /// <summary>
    ///     The methods carrying the element type in their signature, in the order the element type is read from them.
    /// </summary>
    private static readonly string[] ElementTypeSourceMethodNames = [.. InsertionMethodNames, nameof(IList.Contains), "Erase"];

    /// <summary>
    ///     Builds one report row per enumerable the assembly exposes.
    /// </summary>
    /// <param name="assembly">The assembly to scan.</param>
    /// <param name="sourceFileIndex">The index reading the switch arms of the descriptor sources.</param>
    /// <returns>One <see cref="ApiEnumerableRow" /> per enumerable <paramref name="assembly" /> exposes.</returns>
    [Pure]
    public static IReadOnlyList<ApiEnumerableRow> ScanEnumerables(Assembly assembly, SourceFileIndex sourceFileIndex)
    {
        return
        [
            .. assembly
                .GetLoadableTypes()
                .Select(TryDescribeEnumerable)
                .OfType<ApiEnumerableShape>()
                .Select(shape => CreateRow(shape, sourceFileIndex))
        ];
    }

    /// <summary>
    ///     Reflects the shape of the type, skipping a type whose signatures name a type the process cannot load.
    /// </summary>
    private static ApiEnumerableShape? TryDescribeEnumerable(Type type)
    {
        try
        {
            return DescribeEnumerable(type);
        }
        catch (Exception exception) when (exception is FileNotFoundException or TypeLoadException)
        {
            return null;
        }
    }

    /// <summary>
    ///     Reflects the shape of the type, or returns <see langword="null" /> for a type that enumerates nothing.
    /// </summary>
    private static ApiEnumerableShape? DescribeEnumerable(Type type)
    {
        if (!type.IsVisible)
        {
            return null;
        }

        if (!type.IsClass)
        {
            return null;
        }

        if (!typeof(IEnumerable).IsAssignableFrom(type))
        {
            return null;
        }

        var iteratorType = FindIteratorType(type);
        var keyProperty = FindProperty(iteratorType, KeyPropertyName);
        var indexer = FindIndexer(type, typeof(int));

        return new ApiEnumerableShape
        {
            EnumerableType = type,
            Kind = FindKind(keyProperty, indexer),
            ElementType = keyProperty is null ? FindElementType(type, indexer) : FindValueType(type, keyProperty.PropertyType),
            HasIsEmpty = HasProperty(type, IsEmptyPropertyName, typeof(bool)),
            HasCount = HasProperty(type, CountPropertyName, typeof(int))
        };
    }

    private static ApiEnumerableKind FindKind(PropertyInfo? keyProperty, PropertyInfo? indexer)
    {
        if (keyProperty is not null)
        {
            return ApiEnumerableKind.Map;
        }

        if (indexer is not null)
        {
            return ApiEnumerableKind.IndexedSequence;
        }

        return ApiEnumerableKind.Sequence;
    }

    private static Type FindIteratorType(Type enumerableType)
    {
        var iteratorFactory = FindParameterlessMethod(enumerableType, IteratorFactoryName);
        if (iteratorFactory is not null)
        {
            return iteratorFactory.ReturnType;
        }

        var enumeratorFactory = FindParameterlessMethod(enumerableType, nameof(IEnumerable.GetEnumerator));

        return enumeratorFactory?.ReturnType ?? typeof(IEnumerator);
    }

    /// <summary>
    ///     Resolves the value type of a map from the indexer taking the key, falling back to the insertion method.
    /// </summary>
    private static Type FindValueType(Type enumerableType, Type keyType)
    {
        var indexer = FindIndexer(enumerableType, keyType);
        if (indexer is not null)
        {
            return indexer.PropertyType;
        }

        var insertMethod = enumerableType
            .GetMethods(PublicInstance)
            .Where(method => InsertionMethodNames.Contains(method.Name))
            .Where(method => method.GetParameters() is [var key, _] && key.ParameterType == keyType)
            .OrderBy(method => method.DeclaringType == enumerableType ? 0 : 1)
            .FirstOrDefault();

        if (insertMethod is not null)
        {
            return insertMethod.GetParameters()[1].ParameterType;
        }

        return typeof(object);
    }

    /// <summary>
    ///     Resolves the element type of a sequence from the generic contract, the indexer, or a single-element member.
    /// </summary>
    private static Type FindElementType(Type enumerableType, PropertyInfo? indexer)
    {
        var enumerableArgument = FindEnumerableArgument(enumerableType);
        if (enumerableArgument is not null)
        {
            return enumerableArgument;
        }

        if (indexer is not null)
        {
            return indexer.PropertyType;
        }

        foreach (var methodName in ElementTypeSourceMethodNames)
        {
            var method = enumerableType
                .GetMethods(PublicInstance)
                .Where(candidate => candidate.Name == methodName)
                .OrderBy(candidate => candidate.DeclaringType == enumerableType ? 0 : 1)
                .FirstOrDefault(candidate => candidate.GetParameters().Length == 1);

            if (method is not null)
            {
                return method.GetParameters()[0].ParameterType;
            }
        }

        return typeof(object);
    }

    private static Type? FindEnumerableArgument(Type enumerableType)
    {
        return enumerableType
            .GetInterfaces()
            .Where(candidate => candidate.IsGenericType)
            .Where(candidate => candidate.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .Select(candidate => candidate.GetGenericArguments()[0])
            .FirstOrDefault();
    }

    private static bool HasProperty(Type type, string propertyName, Type propertyType)
    {
        return FindProperty(type, propertyName)?.PropertyType == propertyType;
    }

    private static PropertyInfo? FindIndexer(Type type, Type indexType)
    {
        return type
            .GetProperties(PublicInstance)
            .Where(property => property.Name == IndexerPropertyName)
            .Where(property => property.GetIndexParameters() is [var index] && index.ParameterType == indexType)
            .OrderBy(property => property.DeclaringType == type ? 0 : 1)
            .FirstOrDefault();
    }

    /// <summary>
    ///     Finds the most derived declaration of a parameterless method.
    /// </summary>
    /// <remarks>
    ///     The interop layer declares the iterator factory on the base collection and narrows the return type on the derived one,
    ///     a pair of overloads <see cref="Type.GetMethod(string, BindingFlags)" /> reports as an ambiguous match.
    /// </remarks>
    private static MethodInfo? FindParameterlessMethod(Type type, string methodName)
    {
        return type
            .GetMethods(PublicInstance)
            .Where(method => method.Name == methodName)
            .Where(method => method.GetParameters().Length == 0)
            .OrderBy(method => method.DeclaringType == type ? 0 : 1)
            .FirstOrDefault();
    }

    /// <summary>
    ///     Finds the most derived declaration of a property taking no index.
    /// </summary>
    private static PropertyInfo? FindProperty(Type type, string propertyName)
    {
        return type
            .GetProperties(PublicInstance)
            .Where(property => property.Name == propertyName)
            .Where(property => property.GetIndexParameters().Length == 0)
            .OrderBy(property => property.DeclaringType == type ? 0 : 1)
            .FirstOrDefault();
    }

    /// <summary>
    ///     Finds the type named by the descriptor switch arm matching the type, or <see langword="null" /> when no arm matches it.
    /// </summary>
    /// <remarks>
    ///     A switch arm holds a declaration pattern, which matches every type deriving from the type it names.
    ///     An arm over a base class or an interface covers the whole hierarchy below it.
    /// </remarks>
    private static string? FindDescriptorArm(Type enumerableType, SourceFileIndex sourceFileIndex)
    {
        for (var candidate = enumerableType; candidate is not null; candidate = candidate.BaseType)
        {
            var candidateName = candidate.FormatDeclarationName();
            if (sourceFileIndex.HasSwitchArm(DescriptorFileName, candidateName))
            {
                return candidateName;
            }
        }

        foreach (var contract in enumerableType.GetInterfaces())
        {
            var contractName = contract.FormatDeclarationName();
            if (sourceFileIndex.HasSwitchArm(DescriptorFileName, contractName))
            {
                return contractName;
            }
        }

        return null;
    }

    private static ApiEnumerableRow CreateRow(ApiEnumerableShape shape, SourceFileIndex sourceFileIndex)
    {
        return new ApiEnumerableRow
        {
            Kind = shape.Kind,
            TypeName = shape.EnumerableType.FormatName(),
            Namespace = shape.EnumerableType.Namespace ?? string.Empty,
            ElementType = shape.ElementType.FormatName(),
            IsApiObject = typeof(APIObject).IsAssignableFrom(shape.EnumerableType),
            HasIsEmpty = shape.HasIsEmpty,
            HasCount = shape.HasCount,
            DescriptorArm = FindDescriptorArm(shape.EnumerableType, sourceFileIndex)
        };
    }
}
