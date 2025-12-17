using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using WindowDebugger.Localizations;
using WindowDebugger.Native;
using WindowDebugger.Services.NativeWindows;
using WindowDebugger.Services.NativeWindows.Windows;
using WindowDebugger.Views.Details;
using WindowDebugger.Views.Details.Linux;
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

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        var vm = new MainViewModel();
        DataContext = vm;
        _ = ReloadAllAsync();
    }

    private void ReloadAllButton_Click(object? sender, RoutedEventArgs e)
    {
        _ = ReloadAllAsync();
    }

    private void CaptureButton_Click(object? sender, RoutedEventArgs e)
    {
    }

    private void ReloadButton_Click(object? sender, RoutedEventArgs e)
    {
        var oldSelection = WindowTreeView.SelectedItem;
        WindowTreeView.SelectedItem = null;
        WindowTreeView.SelectedItem = oldSelection;
    }

    private void RevealExecutableFileButton_Click(object? sender, RoutedEventArgs e)
    {
        var path = ProcessPathTextBox.Text;
        if (path is not null && File.Exists(path))
        {
            NativeFileManager.RevealFile(path);
        }
    }

    private void WindowTreeView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // 滚动到选中的项。
        _ = ScrollToItem(WindowTreeView.SelectedItem);
    }

    private async Task ReloadAllAsync()
    {
        var vm = (MainViewModel)DataContext!;
        var oldSelection = WindowTreeView.SelectedItem as WindowsNativeWindowModel;

        vm.ReloadWindows();

        var selfId = Environment.ProcessId;
        var newSelection = vm.NativeTree.EnumerableAllWindows().FirstOrDefault(x => x.Window.Id == oldSelection?.Id);
        var defaultSelection = newSelection ?? vm.NativeTree.EnumerableAllWindows().FirstOrDefault(x => x.Window.ProcessId == selfId);
        if (defaultSelection is not null)
        {
            if (newSelection is null)
            {
                // 初次选择，或者此前已取消选择。
                var index = vm.NativeTree.IndexOf(defaultSelection);
                await ScrollToItem(vm.NativeTree[^1]);
                await ScrollToItem(vm.NativeTree[Math.Max(0, index - 1)]);
                await Task.Delay(0);
                WindowTreeView.SelectedItem = defaultSelection;
            }
            else
            {
                // 曾经已选择，刷新后重新选择。
                await Task.Delay(0);
                var index = vm.NativeTree.IndexOf(defaultSelection);
                await ScrollToItem(vm.NativeTree[Math.Max(0, index)]);
                WindowTreeView.SelectedItem = defaultSelection;
            }
        }
    }

    private void SettingsButton_Click(object? sender, RoutedEventArgs e)
    {
        var w = (MainWindow)TopLevel.GetTopLevel(this)!;
        _ = w.ShowTransientViewAsync(new SettingsView());
    }

    private async Task ScrollToItem(object? item)
    {
        if (item is not null)
        {
            await Task.Delay(0);
            WindowTreeView.ScrollIntoView(item);
        }
    }

    private void InitializePlatformPages()
    {
        if (OperatingSystem.IsLinux())
        {
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Pages.Linux.Info.Title,
                Content = new WindowInfosPage(),
            });
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Pages.Linux.Properties.Title,
                Content = new WindowPropertiesPage(),
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
