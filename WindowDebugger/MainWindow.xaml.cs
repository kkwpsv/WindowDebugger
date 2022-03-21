using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using Lsj.Util.Win32.Extensions;
using WindowDebugger.ViewModels;
using WPFUI.Appearance;

namespace WindowDebugger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            Watcher.Watch(this, BackgroundType.Mica, true);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            RootNavigation.Navigate("windows", false);
        }
    }
}
