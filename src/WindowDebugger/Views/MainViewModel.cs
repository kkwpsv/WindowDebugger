using Avalonia.Collections;
using Avalonia.Threading;
using Lsj.Util.Win32.BaseTypes;
using ReactiveUI;
using WindowDebugger.Services.NativeWindows;
using WindowDebugger.Utils;

namespace WindowDebugger.Views;

public class MainViewModel : ReactiveObject
{
    private readonly ForegroundWindowTracker _tracker = new();

    public MainViewModel()
    {
        WindowList = new WindowListViewModel(this);
        _tracker.ForegroundWindowChanged += Tracker_ForegroundWindowChanged;
    }

    public WindowListViewModel WindowList { get; }

    public AvaloniaList<NativeTreeNode> NativeTree { get; } = [];

    public AvaloniaList<TrackedForegroundWindowModel> TrackedWindowsHistory { get; } = [];

    public NativeTreeNode? SelectedNode
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool IsForegroundWindowTracking
    {
        get => _tracker.IsEnabled;
        set
        {
            if (value)
            {
                TrackedWindowsHistory.Clear();
            }
            _tracker.IsEnabled = value;
            this.RaisePropertyChanged();
        }
    }

    public bool AllowsTrackSelf
    {
        get => _tracker.AllowsTrackSelf;
        set
        {
            IsForegroundWindowTracking = false;
            _tracker.AllowsTrackSelf = value;
            this.RaisePropertyChanged();
        }
    }

    public void ReloadWindows()
    {
        var tree = WindowList.ReloadWindows();
        NativeTree.Clear();
        NativeTree.AddRange(tree);
    }

    private void Tracker_ForegroundWindowChanged(object? sender, HWND hwnd)
    {
        var time = DateTime.Now;

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var node = NativeTree.EnumerableAllWindows().FirstOrDefault(windowNode => windowNode.Window.Id == hwnd);
            SelectedNode = node;
            if (node?.Window is { } window)
            {
                TrackedWindowsHistory.Add(new TrackedForegroundWindowModel
                {
                    TrackedTime = time,
                    Window = window,
                });
            }
        });
    }
}

public record TrackedForegroundWindowModel
{
    public required DateTime TrackedTime { get; init; }

    public required NativeWindowModel Window { get; init; }
}
