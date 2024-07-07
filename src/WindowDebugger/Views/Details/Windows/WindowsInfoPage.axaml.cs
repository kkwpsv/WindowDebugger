using System.Collections.Immutable;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Lsj.Util.Win32.Enums;

namespace WindowDebugger.Views.Details.Windows;

public partial class WindowsInfoPage : UserControl
{
    public WindowsInfoPage()
    {
        InitializeComponent();
    }

    public ImmutableArray<ShowWindowCommands> AllShowWindowCommands { get; } = [..Enum.GetValues<ShowWindowCommands>()];

    public ImmutableArray<WindowDisplayAffinities> AllWindowDisplayAffinities { get; } = [..Enum.GetValues<WindowDisplayAffinities>()];

    private void UpdateText(object sender, RoutedEventArgs e)
    {
    }

    private void UpdateRect(object sender, RoutedEventArgs e)
    {
    }

    private void UpdateParentWindowHandle(object sender, RoutedEventArgs e)
    {
    }

    private void UpdateOwnerWindowHandle(object sender, RoutedEventArgs e)
    {
    }
}
