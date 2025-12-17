using System.Globalization;
using Avalonia.Data.Converters;

namespace WindowDebugger.Views.Converters;

public class ZeroToStringConverter : IValueConverter
{
    public string? Zero { get; set; }

    public string? NonZero { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int number)
        {
            return number == 0 ? Zero : NonZero;
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public class ZeroToBooleanConverter : IValueConverter
{
    public bool Zero { get; set; }

    public bool NonZero { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int number)
        {
            return number == 0 ? Zero : NonZero;
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
