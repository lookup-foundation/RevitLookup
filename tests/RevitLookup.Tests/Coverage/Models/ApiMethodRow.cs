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
///     Represents a single row of the utility method report.
/// </summary>
public sealed record ApiMethodRow
{
    /// <summary>
    ///     Gets the short name of the method return type.
    /// </summary>
    public required string ReturnType { get; init; }

    /// <summary>
    ///     Gets the <c>Type.Method</c> name of the reported method.
    /// </summary>
    public required string QualifiedName { get; init; }

    /// <summary>
    ///     Gets the method parameters rendered as a comma separated <c>Type name</c> list.
    /// </summary>
    public required string Parameters { get; init; }

    /// <summary>
    ///     Gets the names of the descriptor source files mentioning <see cref="QualifiedName" />.
    /// </summary>
    /// <remarks>
    ///     An empty list marks a method no descriptor resolves yet.
    /// </remarks>
    public required IReadOnlyList<string> DescriptorFiles { get; init; }
}
