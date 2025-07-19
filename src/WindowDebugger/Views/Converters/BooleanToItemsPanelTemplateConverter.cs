using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml.Templates;

namespace WindowDebugger.Views.Converters;

public class BooleanToItemsPanelTemplateConverter : IValueConverter
{
    public ItemsPanelTemplate? True { get; set; }

    public ItemsPanelTemplate? False { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool booleanValue)
        {
            return booleanValue ? True : False;
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
