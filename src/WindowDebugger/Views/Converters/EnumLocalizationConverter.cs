using System.Globalization;
using Avalonia.Data.Converters;
using dotnetCampus.Localizations;
using Lsj.Util.Win32.Enums;
using WindowDebugger.Localizations;

namespace WindowDebugger.Views.Converters;

public abstract class EnumLocalizationConverter<T> : IValueConverter
    where T : struct, Enum
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is T enumValue)
        {
            return Lang.Current.Get0($"Dynamic.EnumValues.{typeof(T).Name}.{enumValue}").ToString();
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public sealed class DpiAwarenessLocalizationConverter : EnumLocalizationConverter<DPI_AWARENESS>;
