using Lsj.Util.Win32.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace WindowDebugger.Converters
{
    public class EnumToDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (str.StartsWith("WS_EX_"))
                {
                    var result = (WindowStylesEx)Enum.Parse(typeof(WindowStylesEx), str);
                    return $"{str.Replace("_", "__")} (0X{((uint)result).ToString("X8")})";
                }
                else if (str.StartsWith("WS_"))
                {
                    var result = (WindowStyles)Enum.Parse(typeof(WindowStyles), str);
                    return $"{str.Replace("_", "__")} (0X{((uint)result).ToString("X8")})";
                }
            }
            return DependencyProperty.UnsetValue;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
