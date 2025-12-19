using System.Runtime.Versioning;
using Avalonia.Collections;
using Avalonia.Threading;
using Lsj.Util.Win32.BaseTypes;
using ReactiveUI;
using WindowDebugger.Services.NativeWindows;
using WindowDebugger.Services.NativeWindows.Windows;

namespace WindowDebugger.Views;

public class MainViewModel : ReactiveObject
{
    private readonly IForegroundWindowTracker _tracker;

    public MainViewModel()
    {
        WindowList = new WindowListViewModel(this);

        if (OperatingSystem.IsWindows())
        {
            var tracker = new WindowsForegroundWindowTracker();
            tracker.ForegroundWindowChanged += Tracker_ForegroundWindowChanged;
            _tracker = tracker;
        }
        else
        {
            _tracker = new EmptyForegroundWindowTracker();
        }
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

    [SupportedOSPlatform("windows")]
    private void Tracker_ForegroundWindowChanged(object? sender, HWND hwnd)
    {
        var time = DateTime.Now;
        var node = NativeTree.EnumerableAllWindows().FirstOrDefault(windowNode => windowNode.Window.Id == hwnd);
        if (node?.Window is { } window)
        {
            // 现有窗口，直接添加记录。
            var model = new TrackedForegroundWindowModel(time, node.Window);
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                TrackedWindowsHistory.Insert(0, model);
                SelectedNode = node;
            });
        }
        else
        {
            // 崭新窗口，创建一个新的窗口模型。
            // 等用户点了「刷新」后，这个窗口自然而然就消失了，不影响使用。
            node = new WindowsNativeWindowNode(new WindowsNativeWindowModel(hwnd))
            {
                ChildWindows = [],
            };
            var model = new TrackedForegroundWindowModel(time, node.Window);
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                NativeTree.Add(node);
                TrackedWindowsHistory.Insert(0, model);
                SelectedNode = node;
            });
        }
    }
}

public record TrackedForegroundWindowModel(DateTime TrackedTime, NativeWindowModel Window)
{
    public long Id { get; } = Window.Id;

    public string? Title { get; } = Window.Title;

    public int ProcessId { get; } = Window.ProcessId;

    public string ProcessName { get; } = Window is WindowsNativeWindowModel winModel
        ? winModel.ProcessName
        : "";
}
