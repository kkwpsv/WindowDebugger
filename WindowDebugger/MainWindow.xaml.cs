using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using Lsj.Util.Win32.Extensions;
using WindowDebugger.ViewModels;

namespace WindowDebugger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            WPFUI.Background.Manager.Apply(this);
            InitializeComponent();
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            WPFUI.Background.Manager.Apply(WPFUI.Background.BackgroundType.Mica, hwnd);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            RootNavigation.Navigate("windows", false);
        }
    }
}
