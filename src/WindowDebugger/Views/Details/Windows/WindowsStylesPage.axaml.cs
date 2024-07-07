using System.Collections.Immutable;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Lsj.Util.Win32.Enums;
using WindowDebugger.Services.NativeWindows.Windows;

namespace WindowDebugger.Views.Details.Windows;

public partial class WindowsStylesPage : UserControl
{
    private readonly EnumPageManager<WindowStyles> _manager;

    public WindowsStylesPage()
    {
        InitializeComponent();

        _manager = new EnumPageManager<WindowStyles>(WindowStyleListBox, v => (long)v, v => (WindowStyles)v);

        DataContextChanged += (sender, args) => UpdateView();
        Loaded += (sender, args) => UpdateView();
    }

    public ImmutableArray<WindowStyles> AllWindowStyles => EnumPageManager<WindowStyles>.AllValues;

    private void UpdateView()
    {
        if (DataContext is WindowsNativeWindowModel { Styles: var value })
        {
            ValueTextBox.Text = ((uint)value).ToString("X8");
            _manager.CheckedValue = value;
        }
    }

    private void ChangeButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is WindowsNativeWindowModel vm)
        {
            if (uint.TryParse(ValueTextBox.Text, System.Globalization.NumberStyles.HexNumber, null, out var value))
            {
                vm.Styles = (WindowStyles)value;
                UpdateView();
            }
        }
    }

    private void ToggleButton_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (_manager.IsReloading)
        {
            return;
        }

        var value = _manager.CheckedValue;
        if (DataContext is WindowsNativeWindowModel vm)
        {
            vm.Styles = value;
            UpdateView();
        }
    }
}
