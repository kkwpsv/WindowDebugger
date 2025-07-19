using Avalonia.Controls;
using Avalonia.Interactivity;
using WindowDebugger.Services.NativeWindows.Windows;

namespace WindowDebugger.Views.Details.Windows;

public partial class WindowOperationsPage : UserControl
{
    public WindowOperationsPage()
    {
        InitializeComponent();
    }

    private WindowsNativeWindowModel? NativeWindow => DataContext as WindowsNativeWindowModel;

    private void SetForegroundButton_Click(object? sender, RoutedEventArgs e)
    {
        NativeWindow?.SetForeground();
    }

    private void CloseWindowButton_Click(object? sender, RoutedEventArgs e)
    {
        NativeWindow?.CloseWindow();
    }

    private void RedrawWindowButton_Click(object? sender, RoutedEventArgs e)
    {
        NativeWindow?.RedrawWindow();
    }

    private void FlashWindowButton_Click(object? sender, RoutedEventArgs e)
    {
        NativeWindow?.FlashWindow();
    }

    private void KillProcessButton_Click(object? sender, RoutedEventArgs e)
    {
        NativeWindow?.KillProcess();
    }

    private void ForceKillProcessButton_Click(object? sender, RoutedEventArgs e)
    {
        NativeWindow?.ForceKillProcess();
    }

    private void SuspendThreadButton_Click(object? sender, RoutedEventArgs e)
    {
        NativeWindow?.SuspendThread();
    }

    private void ResumeThreadButton_Click(object? sender, RoutedEventArgs e)
    {
        NativeWindow?.ResumeThread();
    }

    private void KillThreadButton_Click(object? sender, RoutedEventArgs e)
    {
        NativeWindow?.KillThread();
    }
}
