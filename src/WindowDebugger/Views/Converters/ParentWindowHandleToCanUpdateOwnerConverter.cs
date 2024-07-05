using System.Globalization;
using Avalonia.Data.Converters;
using static Lsj.Util.Win32.User32;

namespace WindowDebugger.Views.Converters;

public class ParentWindowHandleToCanUpdateOwnerConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is IntPtr parentWindowHandle && parentWindowHandle == GetDesktopWindow();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
