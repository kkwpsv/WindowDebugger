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
    private bool _isReloading;

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
        set
        {
            if (field == value)
            {
                return;
            }

            if (_isReloading)
            {
                // 在重新加载窗口列表时，因为这里无法获得正确的 Index，所以不触发 SelectionChanged 事件；
                // 在重新加载的方法内，会主动触发具有正确 Index 的 SelectionChanged 事件。
                this.RaiseAndSetIfChanged(ref field, value);
                return;
            }

            var oldValue = field;
            var oldIndex = oldValue is null ? -1 : NativeTree.IndexOf(oldValue);
            var newIndex = value is null ? -1 : NativeTree.IndexOf(value);
            this.RaiseAndSetIfChanged(ref field, value);

            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs
            {
                IsReloading = false,
                OldSelection = oldValue,
                OldSelectionIndex = oldIndex,
                NewSelection = value,
                NewSelectionIndex = newIndex,
            });
        }
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

    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    public void ReloadWindows()
    {
        if (_isReloading)
        {
            return;
        }

        _isReloading = true;

        try
        {
            var oldSelectedNode = SelectedNode;
            var oldIndex = oldSelectedNode is null ? -1 : NativeTree.IndexOf(oldSelectedNode);
            var oldSelectedWindowId = oldSelectedNode switch
            {
                WindowsNativeWindowNode node => node.Window.Id,
                _ => 0,
            };

            var tree = WindowList.ReloadWindows();
            NativeTree.Clear();
            NativeTree.AddRange(tree);

            var selfId = Environment.ProcessId;
            var newSelection = NativeTree.EnumerableAllWindows().FirstOrDefault(x => x.Window.Id == oldSelectedWindowId);
            var defaultSelection = newSelection ?? NativeTree.EnumerableAllWindows().FirstOrDefault(x => x.Window.ProcessId == selfId);
            var newIndex = defaultSelection is null ? -1 : NativeTree.IndexOf(defaultSelection);
            SelectedNode = defaultSelection;

            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs
            {
                IsReloading = true,
                OldSelection = oldSelectedNode,
                OldSelectionIndex = oldIndex,
                NewSelection = defaultSelection,
                NewSelectionIndex = newIndex,
            });
        }
        finally
        {
            _isReloading = false;
        }
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

public class SelectionChangedEventArgs : EventArgs
{
    public required bool IsReloading { get; init; }
    public required NativeTreeNode? OldSelection { get; init; }
    public required int OldSelectionIndex { get; init; }
    public required NativeTreeNode? NewSelection { get; init; }
    public required int NewSelectionIndex { get; init; }
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
