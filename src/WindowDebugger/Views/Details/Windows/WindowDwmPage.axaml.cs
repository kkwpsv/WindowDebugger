using Avalonia;
using Avalonia.Controls;
using Lsj.Util.Win32;

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

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        Dwmapi.DwmIsCompositionEnabled(out var isEnabled);
        DwmIsCompositionEnabled = isEnabled;
    }
}
