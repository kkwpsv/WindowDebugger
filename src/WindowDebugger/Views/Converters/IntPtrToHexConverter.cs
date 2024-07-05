using System.Globalization;
using Avalonia.Data.Converters;

namespace WindowDebugger.Views.Converters;

public class IntPtrToHexConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is IntPtr pointer
            ? pointer.ToString("X8")
            : "";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is string text
            ? int.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result) ? result : IntPtr.Zero
            : IntPtr.Zero;
    }
}
