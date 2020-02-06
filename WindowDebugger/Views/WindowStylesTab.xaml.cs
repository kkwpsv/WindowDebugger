using Lsj.Util.Win32.Enums;
using Lsj.Util.Win32.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

namespace WindowDebugger.Views
{
    /// <summary>
    /// WindowStylesTab.xaml 的交互逻辑
    /// </summary>
    public partial class WindowStylesTab : TabItem
    {
        public WindowStylesTab()
        {
            InitializeComponent();
        }

        private void Grid_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is CheckBox checkBox)
            {
                var window = ViewModel.Instance.SelectedWindow;

                var style = (WindowStyles)Enum.Parse(typeof(WindowStyles), (string)checkBox.Tag);

                var styles = window.Styles;

                if (checkBox.IsChecked == true)
                {
                    styles |= style;
                }
                else
                {
                    styles &= ~style;
                }

                if (!window.SetStyles(styles, out var errorCode))
                {
                    MessageBox.Show(Window.GetWindow(this), ErrorMessageExtensions.GetSystemErrorMessageFromCode((uint)errorCode));
                }

                // Sometimes, style will be changed by system, so update IsChecked.
                BindingOperations.GetMultiBindingExpression(checkBox, CheckBox.IsCheckedProperty).UpdateTarget();
            }
        }

        private void UpdateStyles(object sender, RoutedEventArgs e)
        {
            TextBoxStyle.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void RefreshStyles(object sender, RoutedEventArgs e)
        {
            ViewModel.Instance.SelectedWindow.RefreshStyles();
        }
    }
}
