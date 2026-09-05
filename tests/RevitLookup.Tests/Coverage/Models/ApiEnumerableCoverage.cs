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
///     Determines how <c>EnumerableDescriptor</c> finds out whether a Revit API collection contains any elements.
/// </summary>
/// <remarks>
///     The descriptor asks that question of every collection it decomposes.
///     A type carrying <c>IsEmpty</c> or <c>Count</c> answers it through a single property read.
///     Any other type leaves the descriptor its fallback: creating an enumerator and moving it to the first element.
///     The members are declared in report order, the state to act on first.
/// </remarks>
public enum ApiEnumerableCoverage
{
    /// <summary>
    ///     The type carries <c>IsEmpty</c> or <c>Count</c>, and no switch arm of the descriptor reads it.
    /// </summary>
    /// <remarks>
    ///     The descriptor creates an enumerator a new arm would spare it.
    /// </remarks>
    Missing,

    /// <summary>
    ///     The type carries neither <c>IsEmpty</c> nor <c>Count</c>, and only an enumerator tells the descriptor whether elements follow.
    /// </summary>
    Iterated,

    /// <summary>
    ///     A switch arm of the descriptor reads <c>IsEmpty</c> or <c>Count</c> of the type, and no enumerator is created.
    /// </summary>
    Covered
}
