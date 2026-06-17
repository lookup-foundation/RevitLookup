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

// ReSharper disable once CheckNamespace
namespace RevitLookup.UI.Framework.Controls;

/// <summary>
///     Specifies the semantic palette of a <see cref="Chip" />.
/// </summary>
public enum ChipSeverity
{
    /// <summary>Neutral. The default.</summary>
    Neutral,

    /// <summary>Highlights an actionable or pending state.</summary>
    Info,

    /// <summary>Indicates a positive or completed state.</summary>
    Success,

    /// <summary>Indicates a warning or intentionally restricted state.</summary>
    Caution,

    /// <summary>Indicates an error or failure.</summary>
    Critical
}