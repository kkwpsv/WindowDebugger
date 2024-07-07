using System.Collections.Immutable;
using System.ComponentModel;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Lsj.Util.Win32.Enums;
using WindowDebugger.Services.NativeWindows.Windows;

namespace WindowDebugger.Views.Details.Windows;

public partial class WindowsInfoPage : UserControl
{
    private WindowNotificationManager? _notification;
    private WindowsNativeWindowModel? _oldWindow;

    public WindowsInfoPage()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        _notification = new WindowNotificationManager(TopLevel.GetTopLevel(this)!);
    }

    public ImmutableArray<ShowWindowCommands> AllShowWindowCommands { get; } = [..Enum.GetValues<ShowWindowCommands>()];

    public ImmutableArray<WindowDisplayAffinities> AllWindowDisplayAffinities { get; } = [..Enum.GetValues<WindowDisplayAffinities>()];

    private void UpdateText(object sender, RoutedEventArgs e)
    {
        var vm = EnsureViewModel<WindowsNativeWindowModel>();
        vm.Text = WindowTitleTextBox.Text ?? "";
    }

    private void UpdateRect(object sender, RoutedEventArgs e)
    {
        var vm = EnsureViewModel<WindowsNativeWindowModel>();
        if (RectLeftTextBox.Text is { } leftText && int.TryParse(leftText, out var left)
            && RectTopTextBox.Text is { } topText && int.TryParse(topText, out var top)
            && RectWidthTextBox.Text is { } widthText && int.TryParse(widthText, out var width)
            && RectHeightTextBox.Text is { } heightText && int.TryParse(heightText, out var height))
        {
            vm.UpdateWindowRect(left, top, width, height);
        }
    }

    private void UpdateParentWindowHandle(object sender, RoutedEventArgs e)
    {
        var vm = EnsureViewModel<WindowsNativeWindowModel>();
        if (ParentWindowHandleTextBox.Text is { } handleText
            && int.TryParse(handleText, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var handle))
        {
            vm.ParentWindowHandle = handle;
        }
    }

    private void UpdateOwnerWindowHandle(object sender, RoutedEventArgs e)
    {
        var vm = EnsureViewModel<WindowsNativeWindowModel>();
        if (OwnerWindowHandleTextBox.Text is { } handleText
            && int.TryParse(handleText, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var handle))
        {
            vm.OwnerWindowHandle = handle;
        }
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (_oldWindow is {} oldWindow)
        {
            oldWindow.PropertyChanged -= OnWindowPropertyChanged;
        }
        if (DataContext is WindowsNativeWindowModel newWindow)
        {
            newWindow.PropertyChanged += OnWindowPropertyChanged;
            _oldWindow = newWindow;
        }
        else
        {
            _oldWindow = null;
        }
    }

    private void OnWindowPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WindowsNativeWindowModel.ErrorString))
        {
            var vm = (WindowsNativeWindowModel)sender!;
            var error = vm.ErrorString;
            if (!string.IsNullOrWhiteSpace(error))
            {
                vm.ErrorString = null;
                _notification?.Show(error, NotificationType.Error);
            }
        }
    }

    private T EnsureViewModel<T>() where T : class
    {
        if (DataContext is T vm)
        {
            return vm;
        }

        throw new InvalidOperationException("DataContext is not the expected type");
    }
}
