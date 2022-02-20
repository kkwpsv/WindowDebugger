using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace WindowDebugger.Views.Pages;
/// <summary>
/// AboutPage.xaml 的交互逻辑
/// </summary>
public partial class AboutPage : Page
{
    public AboutPage()
    {
        InitializeComponent();
    }

    private void AboutPage_Loaded(object sender, RoutedEventArgs e)
    {
        VersionRun.Text = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? "0.0.0";
    }
}
