using System.Collections.Immutable;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Lsj.Util.Win32.Enums;
using WindowDebugger.Services.NativeWindows.Windows;

namespace WindowDebugger.Views.Details.Windows;

public partial class WindowsStylesPage : UserControl
{
    private bool _isReloading;

    public WindowsStylesPage()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Loaded += OnLoaded;
    }

    public ImmutableArray<WindowStyles> AllWindowStyles { get; } = [..Enum.GetValues<WindowStyles>()];

    public WindowStyles CheckedValue
    {
        get
        {
            WindowStyles value = 0;
            if (WindowStyleListBox.FindDescendantOfType<UniformGrid>() is { } panel)
            {
                try
                {
                    _isReloading = true;
                    foreach (var checkBox in panel.Children.Select(x => x.FindDescendantOfType<CheckBox>()).OfType<CheckBox>())
                    {
                        var v = (WindowStyles)checkBox.DataContext!;
                        if (checkBox.IsChecked == true)
                        {
                            value |= v;
                        }
                    }
                }
                finally
                {
                    _isReloading = false;
                }
            }
            return value;
        }
        set
        {
            if (WindowStyleListBox.FindDescendantOfType<UniformGrid>() is { } panel)
            {
                try
                {
                    _isReloading = true;
                    foreach (var checkBox in panel.Children.Select(x => x.FindDescendantOfType<CheckBox>()).OfType<CheckBox>())
                    {
                        var v = (WindowStyles)checkBox.DataContext!;
                        checkBox.IsChecked = value.HasFlag(v);
                    }
                }
                finally
                {
                    _isReloading = false;
                }
            }
        }
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        TryUpdateView();
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        TryUpdateView();
    }

    private void TryUpdateView()
    {
        if(DataContext is not WindowsNativeWindowModel vm)
        {
            return;
        }

        var value = vm.Styles;
        ValueTextBox.Text = ((uint)value).ToString("X8");
        CheckedValue = value;
    }

    private void ToggleButton_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (_isReloading)
        {
            return;
        }

        var value = CheckedValue;
        if (DataContext is WindowsNativeWindowModel vm)
        {
            vm.Styles = value;
            CheckedValue = vm.Styles;
        }
    }
}
