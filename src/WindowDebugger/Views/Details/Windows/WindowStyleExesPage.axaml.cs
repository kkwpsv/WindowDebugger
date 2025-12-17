using Avalonia.Controls;
using Avalonia.Interactivity;
using Lsj.Util.Win32.Enums;
using WindowDebugger.Services.NativeWindows.Windows;

namespace WindowDebugger.Views.Details.Windows;

public partial class WindowStyleExesPage : UserControl
{
    private readonly EnumPageManager<WindowStylesEx> _manager;

    public WindowStyleExesPage()
    {
        InitializeComponent();

        _manager = new EnumPageManager<WindowStylesEx>(WindowStyleListBox, v => (long)v, v => (WindowStylesEx)v);

        DataContextChanged += (sender, args) => UpdateView();
        Loaded += (sender, args) => UpdateView();
    }

    public IReadOnlyList<EnumNamedValue<WindowStylesEx>> AllWindowStyleExes => EnumPageManager<WindowStylesEx>.AllValues;

    private void UpdateView()
    {
        if (DataContext is WindowsNativeWindowModel { StylesEx: var value })
        {
            ValueTextBox.Text = ((uint)value).ToString("X8");
            _manager.UpdateValues(value);
        }
    }

    private void ChangeButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is WindowsNativeWindowModel vm)
        {
            if (uint.TryParse(ValueTextBox.Text, System.Globalization.NumberStyles.HexNumber, null, out var value))
            {
                vm.StylesEx = (WindowStylesEx)value;
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

        var value = _manager.CheckOrUncheckValues((CheckBox)sender!);
        if (DataContext is WindowsNativeWindowModel vm)
        {
            vm.StylesEx = value;
            UpdateView();
        }
    }
}
