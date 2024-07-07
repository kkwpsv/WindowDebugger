using Avalonia.Controls;
using Avalonia.Interactivity;
using WindowDebugger.Localizations;
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

        await vm.ReloadWindows();

        var selfId = Environment.ProcessId;
        WindowListBox.SelectedItem = vm.NativeWindows.FirstOrDefault(x => x.ProcessId == selfId);
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
