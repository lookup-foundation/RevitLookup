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

namespace RevitLookup.Tests.Coverage.Models;

/// <summary>
///     Represents a single row of the enumerable report.
/// </summary>
public sealed record ApiEnumerableRow
{
    /// <summary>
    ///     Gets the shape the enumerable exposes.
    /// </summary>
    public required ApiEnumerableKind Kind { get; init; }

    /// <summary>
    ///     Gets the short name of the enumerable type.
    /// </summary>
    public required string TypeName { get; init; }

    /// <summary>
    ///     Gets the namespace declaring the enumerable type.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    ///     Gets the short name of the element type the enumerable holds.
    /// </summary>
    public required string ElementType { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the type derives from <see cref="Autodesk.Revit.DB.APIObject" />, the interop base holding a native handle.
    /// </summary>
    public required bool IsApiObject { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the type exposes a <c>bool IsEmpty</c> property.
    /// </summary>
    public required bool HasIsEmpty { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the type exposes an <c>int Count</c> property.
    /// </summary>
    public required bool HasCount { get; init; }

    /// <summary>
    ///     Gets the type named by the <c>EnumerableDescriptor</c> switch arm reading <c>IsEmpty</c> or <c>Count</c> of this type.
    /// </summary>
    /// <remarks>
    ///     <see langword="null" /> marks a type no arm matches. A base type or an interface here marks a type an arm reaches through the hierarchy.
    /// </remarks>
    public required string? DescriptorArm { get; init; }

    /// <summary>
    ///     Gets how the descriptor finds out whether an instance of this type contains any elements.
    /// </summary>
    public ApiEnumerableCoverage Coverage
    {
        get
        {
            if (DescriptorArm is not null)
            {
                return ApiEnumerableCoverage.Covered;
            }

            if (HasIsEmpty || HasCount)
            {
                return ApiEnumerableCoverage.Missing;
            }

            return ApiEnumerableCoverage.Iterated;
        }
    }
}
