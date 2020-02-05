using Lsj.Util.Win32.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace WindowDebugger.Converters
{
    public class ProcessIDToDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int val)
            {
                try
                {
                    var process = Process.GetProcessById(val);
                    return $"{val} ({process.ProcessName})";
                }
                catch
                {
                    return val.ToString();
                }

            }
            return DependencyProperty.UnsetValue;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
