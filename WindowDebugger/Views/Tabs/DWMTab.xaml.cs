using Lsj.Util.Win32.Enums;
using Lsj.Util.Win32.NativeUI;
using System.Windows;
using System.Windows.Controls;
using WindowDebugger.ViewModels;

namespace WindowDebugger.Views.Tabs
{
    /// <summary>
    /// DWMTab.xaml 的交互逻辑
    /// </summary>
    public partial class DWMTab : TabItem
    {
        public DWMTab()
        {
            InitializeComponent();
        }

        private void NonClientRenderingPolicyButton_Click(object sender, RoutedEventArgs e)
        {
            if (NonClientRenderingPolicyComboBox.SelectedItem != null)
            {
                var dwmInfo = ((WindowItem)((Button)sender).DataContext).DWMInfo;
                dwmInfo.NonClientRenderingPolicy = (DWMNCRENDERINGPOLICY)((ComboBoxItem)NonClientRenderingPolicyComboBox.SelectedItem).Tag;
            }
        }

        private void IsDWMTransitionsEnabledButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsDWMTransitionsEnabledComboBox.SelectedItem != null)
            {
                var dwmInfo = ((WindowItem)((Button)sender).DataContext).DWMInfo;
                dwmInfo.IsDWMTransitionsEnabled = (bool)((ComboBoxItem)IsDWMTransitionsEnabledComboBox.SelectedItem).Tag;
            }
        }

        private void IsNonClientContentRightToLeftLayoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsNonClientContentRightToLeftLayoutComboBox.SelectedItem != null)
            {
                var dwmInfo = ((WindowItem)((Button)sender).DataContext).DWMInfo;
                dwmInfo.IsNonClientContentRightToLeftLayout = (bool)((ComboBoxItem)IsNonClientContentRightToLeftLayoutComboBox.SelectedItem).Tag;
            }
        }

        private void IsForceIconicRepresentationButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsNonClientContentRightToLeftLayoutComboBox.SelectedItem != null)
            {
                var dwmInfo = ((WindowItem)((Button)sender).DataContext).DWMInfo;
                dwmInfo.IsForceIconicRepresentation = (bool)((ComboBoxItem)IsForceIconicRepresentationComboBox.SelectedItem).Tag;
            }
        }

        private void Flip3DPolicyButton_Click(object sender, RoutedEventArgs e)
        {
            if (Flip3DPolicyComboBox.SelectedItem != null)
            {
                var dwmInfo = ((WindowItem)((Button)sender).DataContext).DWMInfo;
                dwmInfo.Flip3DPolicy = (DWMFLIP3DWINDOWPOLICY)((ComboBoxItem)Flip3DPolicyComboBox.SelectedItem).Tag;
            }
        }

        private void HasIconicBitmapButton_Click(object sender, RoutedEventArgs e)
        {
            if (HasIconicBitmapComboBox.SelectedItem != null)
            {
                var dwmInfo = ((WindowItem)((Button)sender).DataContext).DWMInfo;
                dwmInfo.HasIconicBitmap = (bool)((ComboBoxItem)HasIconicBitmapComboBox.SelectedItem).Tag;
            }
        }

        private void IsDisallowPeekButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsDisallowPeekComboBox.SelectedItem != null)
            {
                var dwmInfo = ((WindowItem)((Button)sender).DataContext).DWMInfo;
                dwmInfo.IsDisallowPeek = (bool)((ComboBoxItem)IsDisallowPeekComboBox.SelectedItem).Tag;
            }
        }

        private void IsExcludedFromPeekButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsExcludedFromPeekComboBox.SelectedItem != null)
            {
                var dwmInfo = ((WindowItem)((Button)sender).DataContext).DWMInfo;
                dwmInfo.IsExcludedFromPeek = (bool)((ComboBoxItem)IsExcludedFromPeekComboBox.SelectedItem).Tag;
            }
        }

        private void IsCloakButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsCloakComboBox.SelectedItem != null)
            {
                var dwmInfo = ((WindowItem)((Button)sender).DataContext).DWMInfo;
                dwmInfo.IsCloak = (bool)((ComboBoxItem)IsCloakComboBox.SelectedItem).Tag;
            }
        }

        private void IsFreezeRepresentationButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsFreezeRepresentationComboBox.SelectedItem != null)
            {
                var dwmInfo = ((WindowItem)((Button)sender).DataContext).DWMInfo;
                dwmInfo.IsFreezeRepresentation = (bool)((ComboBoxItem)IsFreezeRepresentationComboBox.SelectedItem).Tag;
            }
        }
    }
}
