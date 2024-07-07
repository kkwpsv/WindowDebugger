using Avalonia.Controls;
using Avalonia.Interactivity;
using WindowDebugger.Localizations;
using WindowDebugger.Services.NativeWindows.Windows;
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

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        var vm = new MainViewModel();
        DataContext = vm;

        vm.ReloadWindows();

        var selfId = Environment.ProcessId;
        var defaultSelection = vm.NativeWindows.FirstOrDefault(x => x.ProcessId == selfId);
        if (defaultSelection is not null)
        {
            await Task.Delay(1);
            var index = vm.NativeWindows.IndexOf(defaultSelection);
            WindowListBox.ScrollIntoView(vm.NativeWindows[^1]);
            WindowListBox.ScrollIntoView(vm.NativeWindows[Math.Max(0, index - 1)]);
            WindowListBox.SelectedItem = defaultSelection;
        }
    }

    private void ReloadButton_Click(object? sender, RoutedEventArgs e)
    {
        var oldSelection = WindowListBox.SelectedItem as WindowsNativeWindowModel;
        WindowListBox.SelectedItem = null;
        WindowListBox.SelectedItem = oldSelection;
    }

    private void InitializePlatformPages()
    {
        if (OperatingSystem.IsLinux())
        {
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Pages.Linux.Info.Title,
                Content = new WindowsInfoPage(),
            });
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Pages.Linux.Properties.Title,
                Content = new WindowsInfoPage(),
            });
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Pages.Linux.Operations.Title,
                Content = new WindowsInfoPage(),
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
