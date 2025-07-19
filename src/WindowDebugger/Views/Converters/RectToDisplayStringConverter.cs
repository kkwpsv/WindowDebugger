using System.Globalization;
using Avalonia.Data.Converters;
using Lsj.Util.Win32.Structs;

namespace WindowDebugger.Views.Converters;

public class RectToDisplayStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is RECT rect)
        {
            return $"{rect.left}, {rect.top}, {rect.right}, {rect.bottom}";
        }

        return "";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
