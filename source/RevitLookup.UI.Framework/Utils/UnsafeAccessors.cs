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

#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace RevitLookup.UI.Framework.Utils;

/// <summary>
///     Provides zero-overhead access to private framework members without reflection.
/// </summary>
/// <remarks>
///     Available only on modern runtimes, see <see cref="UnsafeAccessorAttribute"/>.
/// </remarks>
internal static class UnsafeAccessors
{
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_internalScrollHost")]
    public static extern ref ScrollViewer DataGridInternalScrollHost(DataGrid dataGrid);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "OnViewportSizeChanged")]
    public static extern void DataGridOnViewportSizeChanged(DataGrid dataGrid, Size previousSize, Size newSize);
}
#endif
