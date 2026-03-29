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

using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RevitLookup.UI.Framework.Converters;

public static class SettingsVisibilityConverters
{
    public static IMultiValueConverter VisibleWhenEmptySearchResults { get; } = new VisibleWhenEmptySearchResultsConverter();
    public static IMultiValueConverter VisibleWhenEmptyAfterInitialization { get; } = new VisibleWhenEmptyAfterInitializationConverter();

    private sealed class VisibleWhenEmptySearchResultsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 3) throw new ArgumentException("Invalid parameters");

            var items = (ICollection)values[0]!;
            if (items.Count > 0) return Visibility.Collapsed;

            if (values[1] is > 0) return Visibility.Collapsed;
            if (values[2] is false) return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class VisibleWhenEmptyAfterInitializationConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is not int collectionSize) return Visibility.Collapsed;
            if (values[1] is not bool isInitialized) return Visibility.Collapsed;

            if (!isInitialized) return Visibility.Collapsed;
            if (collectionSize > 0) return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}