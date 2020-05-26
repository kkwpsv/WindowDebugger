using System;
using System.Globalization;
using System.Windows.Data;

namespace WindowDebugger.Converters
{
    [ValueConversion(typeof(IntPtr), typeof(int))]
    public class IntPtrToInt32Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is IntPtr pointer ? pointer.ToInt32() : (object)0;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is int number ? new IntPtr(number) : (object)IntPtr.Zero;
    }
}
