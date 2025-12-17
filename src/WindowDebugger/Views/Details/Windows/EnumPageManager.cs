using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace WindowDebugger.Views.Details.Windows;

public class EnumPageManager<T>(ItemsControl listBox, Func<T, long> numberConverter, Func<long, T> reverseNumberConverter)
    where T : unmanaged, Enum
{
    public static IReadOnlyList<EnumNamedValue<T>> AllValues { get; } = EnumNamedValue<T>.GetAll();

    public bool IsReloading { get; private set; }

    /// <summary>
    /// 当 <paramref name="changingCheckBox"/> 被选中或取消选中时，请调用此方法同步将同值不同名的其他 <see cref="CheckBox"/> 一并选中或取消选中。
    /// </summary>
    /// <param name="changingCheckBox">正在操作的 <see cref="CheckBox"/>。</param>
    /// <returns>整个 <see cref="CheckBox"/> 组所组成的共同枚举值。</returns>
    public T CheckOrUncheckValues(CheckBox changingCheckBox)
    {
        long result = 0;
        var changingValue = ((EnumNamedValue<T>)changingCheckBox.DataContext!).Value;
        if (listBox.FindDescendantOfType<UniformGrid>() is { } panel)
        {
            try
            {
                IsReloading = true;
                foreach (var checkBox in panel.Children.Select(x => x.FindDescendantOfType<CheckBox>()).OfType<CheckBox>())
                {
                    var value = ((EnumNamedValue<T>)checkBox.DataContext!).Value;
                    if (value.HasFlag(changingValue))
                    {
                        checkBox.IsChecked = changingCheckBox.IsChecked;
                    }
                    var v = numberConverter(value);
                    if (checkBox.IsChecked == true)
                    {
                        result |= v;
                    }
                }
            }
            finally
            {
                IsReloading = false;
            }
        }
        return reverseNumberConverter(result);
    }

    public void UpdateValues(T value)
    {
        if (listBox.FindDescendantOfType<UniformGrid>() is { } panel)
        {
            try
            {
                IsReloading = true;
                foreach (var checkBox in panel.Children.Select(x => x.FindDescendantOfType<CheckBox>()).OfType<CheckBox>())
                {
                    var v = ((EnumNamedValue<T>)checkBox.DataContext!).Value;
                    checkBox.IsChecked = value.HasFlag(v);
                }
            }
            finally
            {
                IsReloading = false;
            }
        }
    }
}

public interface IEnumNamedValue;

public record EnumNamedValue<T>(T Value, string Name) : IEnumNamedValue
    where T : unmanaged, Enum
{
    public bool IsFlagged => typeof(T).IsDefined(typeof(FlagsAttribute), false);

    public override string ToString()
    {
        return IsFlagged ? $"{Name} (0x{Convert.ToInt64(Value):X8})" : Name;
    }

    public static implicit operator T(EnumNamedValue<T> namedValue) => namedValue.Value;

    public static explicit operator EnumNamedValue<T>(T value) => new(value, value.ToString());

    public static IReadOnlyList<EnumNamedValue<T>> GetAll()
    {
        return Enum.GetNames<T>().Select(x => new EnumNamedValue<T>(Enum.Parse<T>(x), x)).ToList();
    }
}
