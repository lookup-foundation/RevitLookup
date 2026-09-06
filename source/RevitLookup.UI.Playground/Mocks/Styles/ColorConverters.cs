using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RevitLookup.UI.Playground.Mocks.Styles;

/// <summary>
///     Provides value converters for binding decomposed member values to WPF media colors.
/// </summary>
public static class ColorConverters
{
    /// <summary>
    ///     Gets the converter that passes a <see cref="Color" /> value through unchanged and does not support the reverse conversion.
    /// </summary>
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
