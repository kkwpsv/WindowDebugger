using Avalonia.Controls;
using Avalonia.Interactivity;
using WindowDebugger.Localizations;
using WindowDebugger.Views.Details;

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
    }

    private void InitializePlatformPages()
    {
        WindowDetailTabControl.Items.Add(new TabItem
        {
            Header = Lang.Current.App.UI.WindowDetail.Common.Info.Title,
            Content = new InfoPage(),
        });
        if (OperatingSystem.IsLinux())
        {
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Linux.Properties.Title,
                Content = new InfoPage(),
            });
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Linux.Operations.Title,
                Content = new InfoPage(),
            });
        }
        else if (OperatingSystem.IsWindows())
        {
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Windows.Styles.Title,
                Content = new InfoPage(),
            });
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Windows.StyleExes.Title,
                Content = new InfoPage(),
            });
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Windows.ClassStyles.Title,
                Content = new InfoPage(),
            });
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Windows.Operations.Title,
                Content = new InfoPage(),
            });
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Windows.Dwm.Title,
                Content = new InfoPage(),
            });
            WindowDetailTabControl.Items.Add(new TabItem
            {
                Header = Lang.Current.App.UI.WindowDetail.Windows.Others.Title,
                Content = new InfoPage(),
            });
        }
    }
}
