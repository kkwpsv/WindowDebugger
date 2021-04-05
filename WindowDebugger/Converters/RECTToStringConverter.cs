using Lsj.Util.Win32.Structs;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WindowDebugger.Converters
{
    public class RECTToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RECT rect)
            {
                return $"({rect.left}, {rect.top}, {rect.right}, {rect.bottom})";
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
