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

namespace RevitLookup.Abstractions.Tools;

/// <summary>
///     Represents information about the Revit unit.
/// </summary>
public sealed class UnitInfo
{
    /// <summary>
    ///     Gets or sets the unit name.
    /// </summary>
    public required string Unit { get; init; }

    /// <summary>
    ///     Gets or sets the unit's display label.
    /// </summary>
    public required string Label { get; init; }

    /// <summary>
    ///     Gets or sets the underlying Revit unit value.
    /// </summary>
    public required object Value { get; init; }

    /// <summary>
    ///     Gets or sets the unit class, or <see langword="null" /> when the unit has none.
    /// </summary>
    public string? Class { get; init; }
}
