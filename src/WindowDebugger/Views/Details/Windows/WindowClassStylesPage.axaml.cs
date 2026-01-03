using Avalonia.Controls;
using Avalonia.Interactivity;
using Lsj.Util.Win32.Enums;
using WindowDebugger.Services.NativeWindows.Windows;

namespace WindowDebugger.Views.Details.Windows;

public partial class WindowClassStylesPage : UserControl
{
    private readonly EnumPageManager<WindowClassStyles> _manager;

    public WindowClassStylesPage()
    {
        InitializeComponent();

        _manager = new EnumPageManager<WindowClassStyles>(WindowStyleListBox, v => (long)v, v => (WindowClassStyles)v);

        DataContextChanged += (sender, args) => UpdateView();
        Loaded += (sender, args) => UpdateView();
    }

    public IReadOnlyList<EnumNamedValue<WindowClassStyles>> AllClassStyles => EnumPageManager<WindowClassStyles>.AllValues;

    private void UpdateView()
    {
        if (DataContext is WindowsNativeWindowModel { ClassStyles: var value })
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
                vm.ClassStyles = (WindowClassStyles)value;
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
            vm.ClassStyles = value;
            UpdateView();
        }
    }
}
