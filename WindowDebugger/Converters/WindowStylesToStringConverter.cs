﻿using Lsj.Util.Win32.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WindowDebugger.Converters
{
    public class WindowStylesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WindowStyles)
            {
                return ((uint)value).ToString("X8");
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && uint.TryParse(str, NumberStyles.HexNumber, culture, out var result))
            {
                return (WindowStyles)result;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
