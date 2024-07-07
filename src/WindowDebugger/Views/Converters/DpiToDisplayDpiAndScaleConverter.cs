using System.Globalization;
using Avalonia.Data.Converters;

namespace WindowDebugger.Views.Converters;

public class DpiToDisplayDpiAndScaleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int dpi)
        {
            return $"{dpi} ({dpi / 96d:P0})";
        }

        return "0";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
