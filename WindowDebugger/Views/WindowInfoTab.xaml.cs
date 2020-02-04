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

namespace WindowDebugger.Views
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
            if (!ViewModel.Instance.SelectedWindow.SetText(TextBoxText.Text, out var errorCode))
            {
                MessageBox.Show(Window.GetWindow(this), new Win32Exception(errorCode).Message);
            }
        }
    }
}
