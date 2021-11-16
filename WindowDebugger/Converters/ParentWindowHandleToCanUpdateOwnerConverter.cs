using System;
using System.Globalization;
using System.Windows.Data;
using static Lsj.Util.Win32.User32;

namespace WindowDebugger.Converters
{
    public class ParentWindowHandleToCanUpdateOwnerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is IntPtr parentWindowHandle && parentWindowHandle == GetDesktopWindow();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
