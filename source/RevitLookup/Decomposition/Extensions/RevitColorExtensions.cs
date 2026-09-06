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

using Color = System.Drawing.Color;

namespace RevitLookup.Decomposition.Extensions;

/// <summary>
///     Provides extension methods for <see cref="Autodesk.Revit.DB.Color" /> to convert to other color representations.
/// </summary>
[PublicAPI]
public static class RevitColorExtensions
{
    /// <param name="color">The Revit color to convert.</param>
    extension(Autodesk.Revit.DB.Color color)
    {
        /// <summary>
        ///     Converts the color to an equivalent <see cref="System.Drawing.Color" />.
        /// </summary>
        /// <returns>A <see cref="System.Drawing.Color" /> with the same red, green, and blue channels and an alpha channel value of 255.</returns>
        public Color GetDrawingColor()
        {
            return Color.FromArgb(255, color.Red, color.Green, color.Blue);
        }
    }
}
