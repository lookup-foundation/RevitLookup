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

namespace RevitLookup.Decomposition.Extensions;

/// <summary>
///     Provides extension methods for <see cref="Autodesk.Revit.ApplicationServices.Application" /> to retrieve extended version information.
/// </summary>
[PublicAPI]
public static class ApplicationExtensions
{
    private static Version? _version;

    /// <param name="application">The Revit application to inspect.</param>
    extension(Autodesk.Revit.ApplicationServices.Application application)
    {
        /// <summary>
        ///     Gets the parsed sub-version number of the Revit application.
        /// </summary>
        /// <remarks>
        ///     The parsed value is cached after the first call and reused for every subsequent call, regardless of the receiver instance.
        /// </remarks>
        public Version Version => _version ??= Version.Parse(application.SubVersionNumber);
    }
}
