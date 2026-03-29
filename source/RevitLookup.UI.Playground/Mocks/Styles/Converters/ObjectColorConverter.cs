using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RevitLookup.UI.Playground.Mocks.Styles.Converters;

public static class ColorConverters
{
    public static IValueConverter MediaColor { get; } = new ObjectColorConverter();
    
    private sealed class ObjectColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                Color color => color,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}