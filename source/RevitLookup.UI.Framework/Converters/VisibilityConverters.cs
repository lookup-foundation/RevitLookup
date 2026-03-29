using System.Collections;
using System.Globalization;
using System.Windows.Data;
using RevitLookup.Abstractions.Services.Settings;
using Visibility = System.Windows.Visibility;

namespace RevitLookup.UI.Framework.Converters;

public static class VisibilityConverters
{
    public static IValueConverter CollapsedWhenTrue { get; } = new CollapsedWhenTrueConverter();
    public static IValueConverter HiddenWhenTrue { get; } = new HiddenWhenTrueConverter();
    public static IValueConverter CollapsedWhenFalse { get; } = new CollapsedWhenFalseConverter();
    public static IValueConverter HiddenWhenFalse { get; } = new HiddenWhenFalseConverter();

    public static IValueConverter CollapsedWhenNullOrEmpty { get; } = new CollapsedWhenNullOrEmptyConverter();

    public static IValueConverter VisibleWhenEmpty { get; } = new VisibleWhenEmptyConverter();
    public static IMultiValueConverter VisibleWhenAllEmpty { get; } = new VisibleWhenAllEmptyConverter();

    public static IValueConverter CollapsedWhenEmpty { get; } = new CollapsedWhenEmptyConverter();
    public static IMultiValueConverter CollapsedWhenAllEmpty { get; } = new CollapsedWhenAllEmptyConverter();
    
    public static IValueConverter HiddenWhenSoftwareUpdateStateEqual { get; } = new EnumHiddenVisibilityConverter<SoftwareUpdateState>();
    public static IValueConverter CollapsedWhenSoftwareUpdateStateEqual { get; } = new EnumCollapsedVisibilityConverter<SoftwareUpdateState>();

    private sealed class CollapsedWhenTrueConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (bool)value! ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (Visibility)value! != Visibility.Visible;
        }
    }

    private sealed class HiddenWhenTrueConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (bool)value! ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (Visibility)value! != Visibility.Visible;
        }
    }

    private sealed class CollapsedWhenFalseConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (bool)value! ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (Visibility)value! == Visibility.Visible;
        }
    }

    private sealed class HiddenWhenFalseConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (bool)value! ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (Visibility)value! == Visibility.Visible;
        }
    }

    private sealed class CollapsedWhenNullOrEmptyConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is string text && !string.IsNullOrEmpty(text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class VisibleWhenEmptyConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var collection = (ICollection)value!;
            return collection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class VisibleWhenAllEmptyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            foreach (var value in values)
            {
                switch (value)
                {
                    case ICollection { Count: > 0 }:
                    case > 0:
                        return Visibility.Collapsed;
                }
            }

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class CollapsedWhenEmptyConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var collection = (ICollection)value!;
            return collection.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class CollapsedWhenAllEmptyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            foreach (var value in values)
            {
                switch (value)
                {
                    case ICollection { Count: > 0 }:
                    case > 0:
                        return Visibility.Visible;
                }
            }

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    
    private sealed class EnumHiddenVisibilityConverter<TEnum> : IValueConverter where TEnum : Enum
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not TEnum valueEnum)
            {
                throw new ArgumentException($"{nameof(value)} is not type: {typeof(TEnum)}");
            }

            if (parameter is not TEnum parameterEnum)
            {
                throw new ArgumentException($"{nameof(parameter)} is not type: {typeof(TEnum)}");
            }

            return EqualityComparer<TEnum>.Default.Equals(valueEnum, parameterEnum) ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    
    public class EnumCollapsedVisibilityConverter<TEnum> : IValueConverter where TEnum : Enum
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not TEnum valueEnum)
            {
                throw new ArgumentException($"{nameof(value)} is not type: {typeof(TEnum)}");
            }

            if (parameter is not TEnum parameterEnum)
            {
                throw new ArgumentException($"{nameof(parameter)} is not type: {typeof(TEnum)}");
            }

            return EqualityComparer<TEnum>.Default.Equals(valueEnum, parameterEnum) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}