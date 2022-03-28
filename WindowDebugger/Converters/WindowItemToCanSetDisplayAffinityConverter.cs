using Lsj.Util.Win32.Enums;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using WindowDebugger.ViewModels;

namespace WindowDebugger.Converters
{
    public class WindowItemToCanSetDisplayAffinityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is WindowItem item && 
            item.ProcessID == Process.GetCurrentProcess().Id &&
            (item.Styles & WindowStyles.WS_CHILD) == 0;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
