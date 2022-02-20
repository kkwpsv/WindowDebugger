using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using WindowDebugger.ViewModels;

namespace WindowDebugger.Views.Pages;
/// <summary>
/// WindowsPage.xaml 的交互逻辑
/// </summary>
public partial class WindowsPage : Page
{
    public WindowsPage()
    {
        InitializeComponent();
    }

    ViewModel Model => ViewModel.Instance;

    private void Page_Loaded(object sender, RoutedEventArgs e)
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

        var hwnd = ((HwndSource)HwndSource.FromVisual(this)).Handle;

        Model.SelectedWindow = selected ?? Model.Windows.FirstOrDefault(x => x.WindowHandle == hwnd);
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
        if (sender is WindowItem item && e.PropertyName == nameof(WindowItem.ErrorString))
        {
            if (item.ErrorString != null)
            {
                MessageBox.Show(Window.GetWindow(this), item.ErrorString);
                item.ErrorString = null;
            }
        }
    }
}
