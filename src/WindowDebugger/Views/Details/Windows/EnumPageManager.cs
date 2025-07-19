using System.Collections.Immutable;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace WindowDebugger.Views.Details.Windows;

public class EnumPageManager<T>(ItemsControl listBox, Func<T, long> numberConverter, Func<long, T> reverseNumberConverter)
    where T : struct, Enum
{
    public static ImmutableArray<T> AllValues { get; } = [..Enum.GetValues<T>()];

    public bool IsReloading { get; private set; }

    public T CheckedValue
    {
        get
        {
            long value = default;
            if (listBox.FindDescendantOfType<UniformGrid>() is { } panel)
            {
                try
                {
                    IsReloading = true;
                    foreach (var checkBox in panel.Children.Select(x => x.FindDescendantOfType<CheckBox>()).OfType<CheckBox>())
                    {
                        var v = numberConverter((T)checkBox.DataContext!);
                        if (checkBox.IsChecked == true)
                        {
                            value |= v;
                        }
                    }
                }
                finally
                {
                    IsReloading = false;
                }
            }
            return reverseNumberConverter(value);
        }
        set
        {
            if (listBox.FindDescendantOfType<UniformGrid>() is { } panel)
            {
                try
                {
                    IsReloading = true;
                    foreach (var checkBox in panel.Children.Select(x => x.FindDescendantOfType<CheckBox>()).OfType<CheckBox>())
                    {
                        var v = (T)checkBox.DataContext!;
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
}
