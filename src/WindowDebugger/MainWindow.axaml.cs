using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Rendering.Composition;
using Vector2 = Avalonia.Vector;
using Vector3 = System.Numerics.Vector3;

namespace WindowDebugger;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        InitializeWindowSize();

        AddHandler(InputElement.PointerPressedEvent, OnGlobalPointerPressed, RoutingStrategies.Tunnel);
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

    private void OnGlobalPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(this);
        if (point.Properties.IsXButton1Pressed)
        {
            _ = CloseTransientViewAsync();
        }
    }

    private void BackButton_Click(object? sender, RoutedEventArgs e)
    {
        _ = CloseTransientViewAsync();
    }

    public async Task ShowTransientViewAsync(Control view)
    {
        TitleTextBlock.Classes.Add("HasBack");
        BackButton.Classes.Add("HasBack");

        TransientBorder.Child = view;

        var task1 = SlideToAnimatedly(MainView, new Vector2(-ContentPanel.Bounds.Width, 0));
        var task2 = SlideFromAnimatedly(TransientBorder, new Vector2(ContentPanel.Bounds.Width, 0));

        await Task.WhenAll(task1, task2);
    }

    public async Task CloseTransientViewAsync()
    {
        TitleTextBlock.Classes.Remove("HasBack");
        BackButton.Classes.Remove("HasBack");

        var task1 = SlideFromAnimatedly(MainView, new Vector2(-ContentPanel.Bounds.Width, 0));
        var task2 = SlideToAnimatedly(TransientBorder, new Vector2(ContentPanel.Bounds.Width, 0));

        await Task.WhenAll(task1, task2);
        TransientBorder.Child = null;
    }

    private Task SlideFromAnimatedly(Visual visual, Vector2 offset)
    {
        return SlideAnimatedly(visual, offset, default, TimeSpan.FromSeconds(0.6));
    }

    private Task SlideToAnimatedly(Visual visual, Vector2 offset)
    {
        return SlideAnimatedly(visual, default, offset, TimeSpan.FromSeconds(0.6));
    }

    private async Task SlideAnimatedly(Visual visual, Vector2 fromOffset, Vector2 toOffset, TimeSpan duration)
    {
        var compositionVisual = ElementComposition.GetElementVisual(visual)!;
        var compositor = compositionVisual.Compositor;

        var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
        offsetAnimation.Target = "Offset";
        offsetAnimation.InsertKeyFrame(0f, new Vector3((float)fromOffset.X, (float)fromOffset.Y, 0));
        offsetAnimation.InsertKeyFrame(1f, new Vector3((float)toOffset.X, (float)toOffset.Y, 0));
        offsetAnimation.Duration = duration;

        compositionVisual.StartAnimation("Offset", offsetAnimation);
        await Task.Delay(duration);
    }
}
