using Avalonia.Controls;
using Avalonia.Interactivity;
using WindowDebugger.Localizations;
using WindowDebugger.Native;
using WindowDebugger.Views.Details;
using WindowDebugger.Views.Details.Windows;

#if NET6_0_OR_GREATER
using WindowDebugger.Views.Details.Linux;
#endif

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
        vm.SelectionChanged += ViewModel_SelectionChanged;
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

    private void ViewTrackingHistoryButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TrackButton.ContextFlyout is { } flyout)
        {
            flyout.Hide();
        }

        var w = (MainWindow)TopLevel.GetTopLevel(this)!;
        var view = new TrackingHistoryView
        {
            DataContext = DataContext,
        };
        _ = w.ShowTransientViewAsync(view);
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

    private async void ViewModel_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // 滚动到选中的项。
        try
        {
            var vm = (MainViewModel)sender!;
            if (e.IsReloading)
            {
                // 如果重新加载整个列表，那么滚动到选中项前后各有一些额外空间，提升视线舒适度。
                await ScrollToItem(vm.NativeTree[Math.Min(e.NewSelectionIndex + 3, vm.NativeTree.Count - 1)]);
                await ScrollToItem(vm.NativeTree[Math.Max(e.NewSelectionIndex - 3, 0)]);
            }
            await ScrollToItem(WindowTreeView.SelectedItem);
        }
        catch (Exception)
        {
            // async void 方法不允许抛出异常。
        }
    }

    private Task ReloadAllAsync()
    {
        var vm = (MainViewModel)DataContext!;
        vm.ReloadWindows();
        return Task.CompletedTask;
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
#if NET6_0_OR_GREATER
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
        else
#endif
        if (OperatingSystem.IsWindows())
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
