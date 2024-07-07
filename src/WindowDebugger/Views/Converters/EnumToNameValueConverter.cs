using System.Globalization;
using Avalonia.Data.Converters;
using Lsj.Util.Win32.Enums;

namespace WindowDebugger.Views.Converters;

public abstract class EnumToNameValueConverter<T> : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is T enumValue)
        {
            return $"{enumValue.ToString()!.Replace("_", "__")} (0x{(uint)value:X8})";
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public sealed class WindowStyleToNameValueConverter : EnumToNameValueConverter<WindowStyles>;

public sealed class WindowStyleExToNameValueConverter : EnumToNameValueConverter<WindowStylesEx>;

public sealed class ClassStylesToNameValueConverter : EnumToNameValueConverter<ClassStyles>;
