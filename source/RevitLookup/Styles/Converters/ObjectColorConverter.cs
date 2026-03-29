using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Color = Autodesk.Revit.DB.Color;

namespace RevitLookup.Styles.Converters;

public static class ColorConverters
{
    public static IValueConverter MediaColor { get; } = new ObjectColorConverter();
    
    private sealed class ObjectColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                Color {IsValid: false} => Colors.Transparent,
                Color color => System.Windows.Media.Color.FromRgb(color.Red, color.Green, color.Blue),
                System.Windows.Media.Color color => color,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}