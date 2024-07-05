using System.Diagnostics;
using System.Globalization;
using Avalonia.Data.Converters;
using Lsj.Util.Win32.Enums;
using WindowDebugger.Services.NativeWindows.Windows;

namespace WindowDebugger.Views.Converters;

public class WindowsNativeWindowModelToCanSetDisplayAffinityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is WindowsNativeWindowModel item &&
               item.ProcessId == Process.GetCurrentProcess().Id &&
               (item.Styles & WindowStyles.WS_CHILD) == 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
