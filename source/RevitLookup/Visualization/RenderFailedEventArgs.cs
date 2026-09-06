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

namespace RevitLookup.Visualization;

/// <summary>
///     Provides data for the <see cref="DirectContext3DServer.RenderFailed" /> event.
/// </summary>
public sealed record RenderFailedEventArgs
{
    /// <summary>
    ///     Gets the exception that was thrown while rendering the scene.
    /// </summary>
    public required Exception ExceptionObject { get; init; }
}
