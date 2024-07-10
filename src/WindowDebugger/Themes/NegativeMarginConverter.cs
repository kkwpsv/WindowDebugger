using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace WindowDebugger.Themes;

public class NegativeMarginConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            Thickness thickness => new Thickness(-thickness.Left, -thickness.Top, -thickness.Right, -thickness.Bottom),
            _ => default,
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            Thickness thickness => new Thickness(-thickness.Left, -thickness.Top, -thickness.Right, -thickness.Bottom),
            _ => default,
        };
    }
}
