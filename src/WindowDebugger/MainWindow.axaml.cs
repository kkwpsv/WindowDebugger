using Avalonia;
using Avalonia.Controls;

namespace WindowDebugger;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        InitializeWindowSize();
    }

    private void InitializeWindowSize()
    {
        var designWidth = Width;
        var designHeight = Height;

        var screen = Screens.ScreenFromWindow(this);
        if (screen is null)
        {
            Width = designWidth;
            Height = designHeight;
            return;
        }

        var desiredHeight = screen.WorkingArea.Height / screen.Scaling * 0.75;
        var height = Math.Min(designHeight, desiredHeight);
        var width = height * 4 / 3;

        Width = width;
        Height = height;
    }
}
