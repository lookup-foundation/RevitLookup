using System.Globalization;
using System.Windows.Data;
using RevitLookup.Abstractions.Updater;

namespace RevitLookup.UI.Framework.Converters;

/// <summary>
///     Provides <see cref="IValueConverter" /> instances that convert or compare boolean values.
/// </summary>
public static class BooleanConverters
{
    /// <summary>
    ///     Gets a converter that negates a <see cref="bool" /> value.
    /// </summary>
    public static IValueConverter Not { get; } = new NotConverter();

    /// <summary>
    ///     Gets a converter that compares a <see cref="SoftwareUpdateState" /> value against the converter parameter for equality.
    /// </summary>
    public static IValueConverter SoftwareUpdateStateEqual { get; } = new EnumBoolConverter<SoftwareUpdateState>();

    private sealed class NotConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !(bool)value!;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !(bool)value!;
        }
    }

    private sealed class EnumBoolConverter<TEnum> : IValueConverter where TEnum : Enum
    {
        /// <inheritdoc />
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

            return EqualityComparer<TEnum>.Default.Equals(valueEnum, parameterEnum);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (parameter is not TEnum parameterEnum)
            {
                throw new ArgumentException($"{nameof(parameter)} is not type: {typeof(TEnum)}");
            }

            return parameterEnum;
        }
    }
}
