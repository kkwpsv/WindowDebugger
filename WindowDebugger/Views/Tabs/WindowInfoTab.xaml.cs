using Lsj.Util.Win32.Extensions;
using Lsj.Util.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowDebugger.ViewModels;

namespace WindowDebugger.Views.Tabs
{
    /// <summary>
    /// WindowInfoTab.xaml 的交互逻辑
    /// </summary>
    public partial class WindowInfoTab : TabItem
    {
        public WindowInfoTab()
        {
            InitializeComponent();
        }

        private void UpdateText(object sender, RoutedEventArgs e)
        {
            TextBoxText.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void UpdateRect(object sender, RoutedEventArgs e)
        {
            if (!ValidationHelper.GetHasErrorWithChildren(GridRect))
            {
                (DataContext as WindowItem).UpdateWindowRect(int.Parse(TextBoxRectLeft.Text), int.Parse(TextBoxRectTop.Text), int.Parse(TextBoxRectWidth.Text), int.Parse(TextBoxRectHeight.Text));
            }
        }

        private void UpdateParentWindowHandle(object sender, RoutedEventArgs e)
        {
            if (!Validation.GetHasError(TextBoxParentWindowHandle))
            {
                TextBoxParentWindowHandle.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }

        private void UpdateOwnerWindowHandle(object sender, RoutedEventArgs e)
        {
            if (!Validation.GetHasError(TextBoxOwnerWindowHandle))
            {
                TextBoxOwnerWindowHandle.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
    }
}
