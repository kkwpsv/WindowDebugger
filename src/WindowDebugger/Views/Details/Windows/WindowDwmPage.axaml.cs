using System.Collections.Immutable;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Lsj.Util.Win32;
using Lsj.Util.Win32.Enums;
using WindowDebugger.Services.NativeWindows.Windows;

namespace WindowDebugger.Views.Details.Windows;

public partial class WindowDwmPage : UserControl
{
    private bool _dwmIsCompositionEnabled;

    public WindowDwmPage()
    {
        InitializeComponent();

        DataContextChanged += OnDataContextChanged;
    }

    public static readonly DirectProperty<WindowDwmPage, bool> DwmIsCompositionEnabledProperty = AvaloniaProperty.RegisterDirect<WindowDwmPage, bool>(
        "DwmIsCompositionEnabled", o => o.DwmIsCompositionEnabled, (o, v) => o.DwmIsCompositionEnabled = v);

    public bool DwmIsCompositionEnabled
    {
        get => _dwmIsCompositionEnabled;
        set => SetAndRaise(DwmIsCompositionEnabledProperty, ref _dwmIsCompositionEnabled, value);
    }

    public ImmutableArray<BooleanValue> AllBooleans { get; } = [..Enum.GetValues<BooleanValue>()];

    public ImmutableArray<DWMNCRENDERINGPOLICY> AllDwmNCRenderingPolicy { get; } = [..Enum.GetValues<DWMNCRENDERINGPOLICY>()];

    public ImmutableArray<DWMFLIP3DWINDOWPOLICY> AllDwmFlip3DWindowPolicy { get; } = [..Enum.GetValues<DWMFLIP3DWINDOWPOLICY>()];

    private WindowsNativeWindowModel WindowModel => (WindowsNativeWindowModel)DataContext!;

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        Dwmapi.DwmIsCompositionEnabled(out var isEnabled);
        DwmIsCompositionEnabled = isEnabled;
    }

    private void NonClientRenderingPolicyButton_Click(object sender, RoutedEventArgs e)
    {
        if (NonClientRenderingPolicyComboBox.GetSelectedEnum<DWMNCRENDERINGPOLICY>() is { } value)
        {
            WindowModel.DWMInfo.NonClientRenderingPolicy = value;
        }
    }

    private void IsDWMTransitionsEnabledButton_Click(object sender, RoutedEventArgs e)
    {
        if (IsDWMTransitionsEnabledComboBox.GetSelectedBoolean() is { } value)
        {
            WindowModel.DWMInfo.IsDWMTransitionsEnabled = value;
        }
    }

    private void IsNonClientContentRightToLeftLayoutButton_Click(object sender, RoutedEventArgs e)
    {
        if (IsNonClientContentRightToLeftLayoutComboBox.GetSelectedBoolean() is { } value)
        {
            WindowModel.DWMInfo.IsNonClientContentRightToLeftLayout = value;
        }
    }

    private void IsForceIconicRepresentationButton_Click(object sender, RoutedEventArgs e)
    {
        if (IsForceIconicRepresentationComboBox.GetSelectedBoolean() is { } value)
        {
            WindowModel.DWMInfo.IsForceIconicRepresentation = value;
        }
    }

    private void Flip3DPolicyButton_Click(object sender, RoutedEventArgs e)
    {
        if (Flip3DPolicyComboBox.GetSelectedEnum<DWMFLIP3DWINDOWPOLICY>() is { } value)
        {
            WindowModel.DWMInfo.Flip3DPolicy = value;
        }
    }

    private void HasIconicBitmapButton_Click(object sender, RoutedEventArgs e)
    {
        if (HasIconicBitmapComboBox.GetSelectedBoolean() is { } value)
        {
            WindowModel.DWMInfo.HasIconicBitmap = value;
        }
    }

    private void IsDisallowPeekButton_Click(object sender, RoutedEventArgs e)
    {
        if (IsDisallowPeekComboBox.GetSelectedBoolean() is { } value)
        {
            WindowModel.DWMInfo.IsDisallowPeek = value;
        }
    }

    private void IsExcludedFromPeekButton_Click(object sender, RoutedEventArgs e)
    {
        if (IsExcludedFromPeekComboBox.GetSelectedBoolean() is { } value)
        {
            WindowModel.DWMInfo.IsExcludedFromPeek = value;
        }
    }

    private void IsCloakButton_Click(object sender, RoutedEventArgs e)
    {
        if (IsCloakComboBox.GetSelectedBoolean() is { } value)
        {
            WindowModel.DWMInfo.IsCloak = value;
        }
    }

    private void IsFreezeRepresentationButton_Click(object sender, RoutedEventArgs e)
    {
        if (IsFreezeRepresentationComboBox.GetSelectedBoolean() is { } value)
        {
            WindowModel.DWMInfo.IsFreezeRepresentation = value;
        }
    }
}

public enum BooleanValue : byte
{
    False,
    True,
}

file static class ComboBoxExtensions
{
    public static bool? GetSelectedBoolean(this ComboBox comboBox)
    {
        return comboBox.SelectedItem switch
        {
            BooleanValue.False => false,
            false => false,
            BooleanValue.True => true,
            true => true,
            _ => null,
        };
    }

    public static T? GetSelectedEnum<T>(this ComboBox comboBox) where T : struct, Enum
    {
        return comboBox.SelectedItem is T value ? value : null;
    }
}
