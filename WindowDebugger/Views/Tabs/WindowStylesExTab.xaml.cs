﻿using Lsj.Util.Win32.Enums;
using Lsj.Util.Win32.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
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
    /// WindowStylesTab.xaml 的交互逻辑
    /// </summary>
    public partial class WindowStylesExTab : TabItem
    {
        public WindowStylesExTab()
        {
            InitializeComponent();
        }

        private void Grid_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is CheckBox checkBox)
            {
                var window = ViewModel.Instance.SelectedWindow;

                var style = (WindowStylesEx)Enum.Parse(typeof(WindowStylesEx), (string)checkBox.Tag);

                if (style == WindowStylesEx.WS_EX_TOPMOST)
                {
                    window.SetTopMost(checkBox.IsChecked == true);
                }
                else
                {
                    var styles = window.StylesEx;

                    if (checkBox.IsChecked == true)
                    {
                        styles |= style;
                    }
                    else
                    {
                        styles &= ~style;
                    }

                    window.StylesEx = styles;
                }

                BindingOperations.GetMultiBindingExpression(checkBox, CheckBox.IsCheckedProperty).UpdateTarget();
            }
        }

        private void UpdateStylesEx(object sender, RoutedEventArgs e)
        {
            TextBoxStyleEx.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void RefreshStylesEx(object sender, RoutedEventArgs e)
        {
            TextBoxStyleEx.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }
    }
}
