using System.Globalization;
using Avalonia.Data.Converters;
using WindowDebugger.Services.NativeWindows;

namespace WindowDebugger.Views.Converters;

public class NativeTreeNodeToNativeWindowConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            NativeProcessNode pn => pn.Windows.FirstOrDefault()?.Window,
            WindowsNativeWindowNode wn => wn.Window,
            LinuxNativeWindowNode ln => ln.Window,
            _ => null,
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
