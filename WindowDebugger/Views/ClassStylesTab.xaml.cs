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
    public partial class ClassStylesTab : TabItem
    {
        public ClassStylesTab()
        {
            InitializeComponent();
        }

        private void Grid_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is CheckBox checkBox)
            {
                var window = ViewModel.Instance.SelectedWindow;

                var style = (ClassStyles)Enum.Parse(typeof(ClassStyles), (string)checkBox.Tag);

                var styles = window.ClassStyles;

                if (checkBox.IsChecked == true)
                {
                    styles |= style;
                }
                else
                {
                    styles &= ~style;
                }

                window.ClassStyles = styles;

                BindingOperations.GetMultiBindingExpression(checkBox, CheckBox.IsCheckedProperty).UpdateTarget();
            }
        }

        private void UpdateClassStyles(object sender, RoutedEventArgs e)
        {
            TextBoxClassStyle.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void RefreshClassStyles(object sender, RoutedEventArgs e)
        {
            ViewModel.Instance.SelectedWindow.RefreshClassStyles();
        }
    }
}
