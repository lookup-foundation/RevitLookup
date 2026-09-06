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
///     Represents the metadata of an assembly runtime module.
/// </summary>
public sealed class ModuleInfo
{
    /// <summary>
    ///     Gets or sets the module name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     Gets or sets the module file path.
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    ///     Gets or sets the module load order.
    /// </summary>
    public required int Order { get; init; }

    /// <summary>
    ///     Gets or sets the module version.
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    ///     Gets or sets the name of the isolation context or application domain that hosts the module.
    /// </summary>
    public required string Container { get; init; }
}
