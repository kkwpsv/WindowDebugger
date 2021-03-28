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
        ViewModel Model => ViewModel.Instance;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _ = UpdateHelper.CheckUpdate();
            RefreshWindowList(null, null);
        }

        private void RefreshWindowList(object sender, RoutedEventArgs e)
        {
            var selectedNew = Model.SelectedWindow;
            var selected = default(WindowItem);

            Model.RefreshWindowList();

            if (selectedNew != null)
            {
                selected = Model.Windows.FirstOrDefault(x => x.WindowHandle == selectedNew.WindowHandle);
            }

            Model.SelectedWindow = selected ?? Model.Windows.FirstOrDefault(x => x.WindowHandle == new WindowInteropHelper(this).Handle);
            if (Model.SelectedWindow != null)
            {
                WindowList.ScrollIntoView(WindowList.Items[WindowList.Items.Count - 1]);
                WindowList.ScrollIntoView(Model.SelectedWindow);
            }
        }

        private void RefreshCurrent(object sender, RoutedEventArgs e)
        {
            Model.SelectedWindow?.RefreshItem();
            Model.SelectedWindow?.RefreshScreenShot();
        }

        private void TabControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is WindowItem olditem)
            {
                olditem.PropertyChanged -= WindowItem_PropertyChanged;
            }
            if (e.NewValue is WindowItem newitem)
            {
                newitem.PropertyChanged += WindowItem_PropertyChanged;
            }
        }

        private void WindowItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is WindowItem item && e.PropertyName == nameof(WindowItem.LastError))
            {
                if (item.LastError != null)
                {
                    MessageBox.Show(this, item.LastError);
                    item.LastError = null;
                }
            }
        }
    }
}
