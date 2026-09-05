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

using RevitLookup.Tests.Coverage.Models;

namespace RevitLookup.Tests.Coverage.Discovery;

/// <summary>
///     Represents the reflected shape of a Revit API enumerable.
/// </summary>
internal sealed record ApiEnumerableShape
{
    /// <summary>
    ///     Gets the enumerable type.
    /// </summary>
    public required Type EnumerableType { get; init; }

    /// <summary>
    ///     Gets the shape the enumerable exposes.
    /// </summary>
    public required ApiEnumerableKind Kind { get; init; }

    /// <summary>
    ///     Gets the element type the enumerable holds.
    /// </summary>
    public required Type ElementType { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the enumerable exposes a <c>bool IsEmpty</c> property.
    /// </summary>
    public required bool HasIsEmpty { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the enumerable exposes an <c>int Count</c> property.
    /// </summary>
    public required bool HasCount { get; init; }
}
