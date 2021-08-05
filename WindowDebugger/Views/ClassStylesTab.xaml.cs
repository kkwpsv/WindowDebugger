using Lsj.Util.Win32.Enums;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
            TextBoxClassStyle.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }
    }
}
