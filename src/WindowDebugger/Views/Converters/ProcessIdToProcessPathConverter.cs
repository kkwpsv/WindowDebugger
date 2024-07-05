using System.Diagnostics;
using System.Globalization;
using Avalonia.Data.Converters;

namespace WindowDebugger.Views.Converters;

public class ProcessIdToProcessPathConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int pid)
        {
            using var process = Process.GetProcessById(pid);
            try
            {
                return process.MainModule?.FileName;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
