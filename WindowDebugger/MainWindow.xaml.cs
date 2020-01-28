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
        ViewModel Model => ViewModel.Instance;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshWindowList(null, null);
        }

        private void RefreshWindowList(object sender, RoutedEventArgs e)
        {
            var selectedNew = Model.SelectedWindow;
            var selected = default(WindowItem);

            var windows = WindowExtensions.GetAllTopLevelWindowHandle();
            Model.Windows.Clear();
            foreach (var handle in windows)
            {
                Model.Windows.Add(new WindowItem { WindowHandle = handle });
            }

            if (selectedNew != null)
            {
                selected = Model.Windows.FirstOrDefault(x => x.WindowHandle == selectedNew.WindowHandle);
            }

            Model.SelectedWindow = selected ?? Model.Windows.FirstOrDefault(x => x.WindowHandle == new WindowInteropHelper(this).Handle);
            WindowList.ScrollIntoView(Model.SelectedWindow);
        }
    }
}
