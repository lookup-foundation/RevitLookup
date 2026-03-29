using System.Globalization;
using System.Windows.Data;
using RevitLookup.Abstractions.Services.Settings;

namespace RevitLookup.UI.Framework.Converters;

public static class BooleanConverters
{
    public static IValueConverter Not { get; } = new NotConverter();
    
    public static IValueConverter SoftwareUpdateStateEqual { get; } = new EnumBoolConverter<SoftwareUpdateState>();

    private sealed class NotConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !(bool)value!;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !(bool)value!;
        }
    }
    
    private sealed class EnumBoolConverter<TEnum> : IValueConverter where TEnum : Enum
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