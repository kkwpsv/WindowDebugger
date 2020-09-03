using System.Windows.Controls;
using WindowDebugger.ViewModels;

namespace WindowDebugger.Views
{
    /// <summary>
    /// OperationsTab.xaml 的交互逻辑
    /// </summary>
    public partial class OperationsTab : TabItem
    {
        public OperationsTab()
        {
            InitializeComponent();
        }

        private void ButtonSetForeground_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.Instance.SelectedWindow.SetForeground();
        }

        private void ButtonCloseWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.Instance.SelectedWindow.CloseWindow();
        }

        private void ButtonRedrawWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.Instance.SelectedWindow.RedrawWindow();
        }

        private void ButtonFlashWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.Instance.SelectedWindow.FlashWindow();
        }

        private void ButtonKillProcess_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.Instance.SelectedWindow.KillProcess();
        }

        private void ButtonForceKillProcess_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.Instance.SelectedWindow.ForceKillProcess();
        }

        private void ButtonKillThread_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.Instance.SelectedWindow.KillThread();
        }
    }
}
