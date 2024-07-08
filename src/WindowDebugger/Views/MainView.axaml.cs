using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using WindowDebugger.Localizations;
using WindowDebugger.Native;
using WindowDebugger.Services.NativeWindows.Windows;
using WindowDebugger.Views.Details;
using WindowDebugger.Views.Details.Windows;

namespace WindowDebugger.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        InitializePlatformPages();

        Loaded += OnLoaded;
    }

    private void TopMostToggleButton_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (TopLevel.GetTopLevel(this) is Window window)
        {
            window.Topmost = ((ToggleButton)sender!).IsChecked is true;
        }
    }

    private void UacButton_Click(object? sender, RoutedEventArgs e)
    {
        if (Environment.ProcessPath is { } path && File.Exists(path))
        {
            Process.Start(new ProcessStartInfo(path)
            {
                UseShellExecute = true,
                Verb = "runas",
            });
            if (TopLevel.GetTopLevel(this) is Window window)
            {
                window.Close();
            }
        }
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        var vm = new MainViewModel();
        DataContext = vm;
        await ReloadAllAsync();
    }

    private async void ReloadAllButton_Click(object? sender, RoutedEventArgs e)
    {
        await ReloadAllAsync();
    }

    private void ReloadButton_Click(object? sender, RoutedEventArgs e)
    {
        var oldSelection = WindowListBox.SelectedItem as WindowsNativeWindowModel;
        WindowListBox.SelectedItem = null;
        WindowListBox.SelectedItem = oldSelection;
    }

    private void RevealExecutableFileButton_Click(object? sender, RoutedEventArgs e)
    {
        var path = ProcessPathTextBox.Text;
        if (path is not null && File.Exists(path))
        {
            NativeFileManager.RevealFile(path);
        }
    }

    private async Task ReloadAllAsync()
    {
        var vm = (MainViewModel)DataContext!;
        var oldSelection = WindowListBox.SelectedItem as WindowsNativeWindowModel;

        vm.ReloadWindows();

        var selfId = Environment.ProcessId;
        var newSelection = vm.NativeWindows.FirstOrDefault(x => x.Id == oldSelection?.Id);
        var defaultSelection = newSelection ?? vm.NativeWindows.FirstOrDefault(x => x.ProcessId == selfId);
        if (defaultSelection is not null)
        {
            if (newSelection is null)
            {
                // 初次选择，或者此前已取消选择。
                var index = vm.NativeWindows.IndexOf(defaultSelection);
                await Task.Delay(0);
                WindowListBox.ScrollIntoView(vm.NativeWindows[^1]);
                await Task.Delay(0);
                WindowListBox.ScrollIntoView(vm.NativeWindows[Math.Max(0, index - 1)]);
                await Task.Delay(0);
                WindowListBox.SelectedItem = defaultSelection;
            }
            else
            {
                // 曾经已选择，刷新后重新选择。
                await Task.Delay(0);
                var index = vm.NativeWindows.IndexOf(defaultSelection);
                WindowListBox.ScrollIntoView(vm.NativeWindows[Math.Max(0, index)]);
                WindowListBox.SelectedItem = defaultSelection;
            }
        }
    }

    private void CaptureButton_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
    }

    private void InitializePlatformPages()
    {
        if (OperatingSystem.IsLinux())
        {
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Pages.Linux.Info.Title,
                Content = new WipPage(),
            });
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Pages.Linux.Properties.Title,
                Content = new WipPage(),
            });
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Pages.Linux.Operations.Title,
                Content = new WipPage(),
            });
        }
        else if (OperatingSystem.IsWindows())
        {
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Pages.Windows.Info.Title,
                Content = new WindowsInfoPage(),
            });
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Pages.Windows.Styles.Title,
                Content = new WindowsStylesPage(),
            });
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Pages.Windows.StyleExes.Title,
                Content = new WindowStyleExesPage(),
            });
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Pages.Windows.ClassStyles.Title,
                Content = new WindowClassStylesPage(),
            });
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Pages.Windows.Operations.Title,
                Content = new WindowOperationsPage(),
            });
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Pages.Windows.Dwm.Title,
                Content = new WindowDwmPage(),
            });
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Pages.Windows.Others.Title,
                Content = new WindowOthersPage(),
            });
        }
    }
}
