using System.Globalization;
using Avalonia.Data.Converters;
using Lsj.Util.Win32.Enums;

namespace WindowDebugger.Views.Converters;

public abstract class Int32EnumToDisplayStringConverter<T> : IValueConverter
    where T : struct, Enum
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is T enumValue)
        {
            return $"{enumValue.ToString()!.Replace("_", "__")} (0x{(int)value:X8})";
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public abstract class UInt32EnumToDisplayStringConverter<T> : IValueConverter
    where T : struct, Enum
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

public sealed class WindowStyleToDisplayStringConverter : UInt32EnumToDisplayStringConverter<WindowStyles>;

public sealed class WindowStyleExToDisplayStringConverter : UInt32EnumToDisplayStringConverter<WindowStylesEx>;

public sealed class ClassStylesToDisplayStringConverter : UInt32EnumToDisplayStringConverter<ClassStyles>;

public sealed class DwmCloakedToDisplayStringConverter : Int32EnumToDisplayStringConverter<DWM_CLOAKED>;
