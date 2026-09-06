using System.Collections;
using System.Globalization;
using System.Windows.Data;
using RevitLookup.Abstractions.Updater;
using Visibility = System.Windows.Visibility;

namespace RevitLookup.UI.Framework.Converters;

/// <summary>
///     Provides <see cref="IValueConverter" /> and <see cref="IMultiValueConverter" /> instances that map a value to a <see cref="Visibility" />.
/// </summary>
public static class VisibilityConverters
{
    /// <summary>
    ///     Gets a converter that returns <see cref="Visibility.Collapsed" /> for <see langword="true" />; otherwise, <see cref="Visibility.Visible" />.
    /// </summary>
    public static IValueConverter CollapsedWhenTrue { get; } = new CollapsedWhenTrueConverter();

    /// <summary>
    ///     Gets a converter that returns <see cref="Visibility.Hidden" /> for <see langword="true" />; otherwise, <see cref="Visibility.Visible" />.
    /// </summary>
    public static IValueConverter HiddenWhenTrue { get; } = new HiddenWhenTrueConverter();

    /// <summary>
    ///     Gets a converter that returns <see cref="Visibility.Collapsed" /> for <see langword="false" />; otherwise, <see cref="Visibility.Visible" />.
    /// </summary>
    public static IValueConverter CollapsedWhenFalse { get; } = new CollapsedWhenFalseConverter();

    /// <summary>
    ///     Gets a converter that returns <see cref="Visibility.Hidden" /> for <see langword="false" />; otherwise, <see cref="Visibility.Visible" />.
    /// </summary>
    public static IValueConverter HiddenWhenFalse { get; } = new HiddenWhenFalseConverter();

    /// <summary>
    ///     Gets a converter that returns <see cref="Visibility.Collapsed" /> for a <see langword="null" /> or empty <see cref="string" />; otherwise, <see cref="Visibility.Visible" />.
    /// </summary>
    public static IValueConverter CollapsedWhenNullOrEmpty { get; } = new CollapsedWhenNullOrEmptyConverter();

    /// <summary>
    ///     Gets a converter that returns <see cref="Visibility.Visible" /> for an empty <see cref="ICollection" />; otherwise, <see cref="Visibility.Collapsed" />.
    /// </summary>
    public static IValueConverter VisibleWhenEmpty { get; } = new VisibleWhenEmptyConverter();

    /// <summary>
    ///     Gets a converter that returns <see cref="Visibility.Visible" /> when every bound collection or count value is empty; otherwise, <see cref="Visibility.Collapsed" />.
    /// </summary>
    public static IMultiValueConverter VisibleWhenAllEmpty { get; } = new VisibleWhenAllEmptyConverter();

    /// <summary>
    ///     Gets a converter that returns <see cref="Visibility.Collapsed" /> for an empty <see cref="ICollection" />; otherwise, <see cref="Visibility.Visible" />.
    /// </summary>
    public static IValueConverter CollapsedWhenEmpty { get; } = new CollapsedWhenEmptyConverter();

    /// <summary>
    ///     Gets a converter that returns <see cref="Visibility.Visible" /> when any bound collection or count value is non-empty; otherwise, <see cref="Visibility.Collapsed" />.
    /// </summary>
    public static IMultiValueConverter CollapsedWhenAllEmpty { get; } = new CollapsedWhenAllEmptyConverter();

    /// <summary>
    ///     Gets a converter that returns <see cref="Visibility.Visible" /> when a <see cref="SoftwareUpdateState" /> value equals the converter parameter; otherwise, <see cref="Visibility.Hidden" />.
    /// </summary>
    public static IValueConverter HiddenWhenSoftwareUpdateStateEqual { get; } = new EnumHiddenVisibilityConverter<SoftwareUpdateState>();

    /// <summary>
    ///     Gets a converter that returns <see cref="Visibility.Visible" /> when a <see cref="SoftwareUpdateState" /> value equals the converter parameter; otherwise, <see cref="Visibility.Collapsed" />.
    /// </summary>
    public static IValueConverter CollapsedWhenSoftwareUpdateStateEqual { get; } = new EnumCollapsedVisibilityConverter<SoftwareUpdateState>();

    private sealed class CollapsedWhenTrueConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (bool)value! ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (Visibility)value! != Visibility.Visible;
        }
    }

    private sealed class HiddenWhenTrueConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (bool)value! ? Visibility.Hidden : Visibility.Visible;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (Visibility)value! != Visibility.Visible;
        }
    }

    private sealed class CollapsedWhenFalseConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (bool)value! ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (Visibility)value! == Visibility.Visible;
        }
    }

    private sealed class HiddenWhenFalseConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (bool)value! ? Visibility.Visible : Visibility.Hidden;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (Visibility)value! == Visibility.Visible;
        }
    }

    private sealed class CollapsedWhenNullOrEmptyConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is string text && !string.IsNullOrEmpty(text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class VisibleWhenEmptyConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var collection = (ICollection)value!;
            return collection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class VisibleWhenAllEmptyConverter : IMultiValueConverter
    {
        /// <inheritdoc />
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

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class CollapsedWhenEmptyConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var collection = (ICollection)value!;
            return collection.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class CollapsedWhenAllEmptyConverter : IMultiValueConverter
    {
        /// <inheritdoc />
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

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class EnumHiddenVisibilityConverter<TEnum> : IValueConverter where TEnum : Enum
    {
        /// <inheritdoc />
        /// <exception cref="ArgumentException"><paramref name="value" /> or <paramref name="parameter" /> is not a <typeparamref name="TEnum" />.</exception>
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

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    ///     Represents a converter that returns <see cref="Visibility.Visible" /> when a <typeparamref name="TEnum" /> value equals the converter parameter; otherwise, <see cref="Visibility.Collapsed" />.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type being compared.</typeparam>
    public class EnumCollapsedVisibilityConverter<TEnum> : IValueConverter where TEnum : Enum
    {
        /// <inheritdoc />
        /// <exception cref="ArgumentException"><paramref name="value" /> or <paramref name="parameter" /> is not a <typeparamref name="TEnum" />.</exception>
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

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
