using System.Globalization;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using ReactiveUI;
using WindowDebugger.Services.NativeWindows;

namespace WindowDebugger.Views.Converters;

public sealed class WindowGroupingToBooleanConverter : NullableEnumToRadioButtonCheckedConverter<WindowGrouping>;

public sealed class WindowSortingToBooleanConverter : NullableEnumToRadioButtonCheckedConverter<WindowSorting>;

/// <summary>
/// 从可空枚举值转换为适用于 <see cref="RadioButton.IsChecked"/> 控件值的转换器。<br/>
/// 正向转换时，在枚举值与指定的转换器参数相等时返回 <see langword="true"/>，否则返回 <see langword="false"/>。<br/>
/// 反向转换时，在布尔值为 <see langword="true"/> 时返回转换器参数值，否则返回 <see langword="null"/>。
/// </summary>
/// <typeparam name="T">枚举类型。</typeparam>
public abstract class NullableEnumToRadioButtonCheckedConverter<T> : IValueConverter
    where T : struct, Enum
{
    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is T unit && parameter is T t)
        {
            return unit.Equals(t);
        }

        return false;
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolean && parameter is T @true)
        {
            return boolean ? @true : null;
        }

        return null;
    }
}

/// <summary>
/// 配合 <see cref="NullableEnumToRadioButtonCheckedConverter{T}"/> 使用的扩展方法，使得 XAML 的 <see cref="RadioButton.IsChecked"/> 属性能够正确地设置枚举值。
/// </summary>
public static class NullableEnumReactiveRecordExtensions
{
    /// <summary>
    /// 设置枚举字段的值并引发属性更改事件，这将使得 <see cref="NullableEnumToRadioButtonCheckedConverter{T}"/> 转换器能够正确地将枚举值与界面上的 <see cref="RadioButton"/> 等控件绑定。
    /// </summary>
    /// <param name="obj">要设置字段的对象。</param>
    /// <param name="backingField">要设置的字段的引用。</param>
    /// <param name="value">要设置的值。</param>
    /// <param name="changedValue">如果字段的值发生了有效更改，则为更改后的值；否则为默认值。</param>
    /// <param name="propertyName">属性名称。</param>
    /// <typeparam name="T">字段的类型。</typeparam>
    /// <returns>
    /// 如果字段的值发生了有效更改，则为 <see langword="true"/>；否则为 <see langword="false"/>。<br/>
    /// 有效更改指的是在配合了转换器正常工作的前提下，值确实更改了（而不是专门为了同步转换器）。
    /// </returns>
    public static bool SetCheckedField<T>(this ReactiveRecord obj, ref T backingField, in T? value, out T changedValue, [CallerMemberName] string? propertyName = null)
        where T : struct, Enum
    {
        if (value is { } v)
        {
            changedValue = v;

            var oldValue = backingField;
            var newValue = obj.RaiseAndSetIfChanged(ref backingField, v, propertyName);
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                return false;
            }

            backingField = v;
            changedValue = v;
            return true;
        }

        changedValue = default;
        return false;
    }
}
