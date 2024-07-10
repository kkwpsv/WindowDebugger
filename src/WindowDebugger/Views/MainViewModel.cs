using System.Collections.Immutable;
using System.Diagnostics;
using Avalonia.Collections;
using ReactiveUI;
using WindowDebugger.Services.NativeWindows;
using WindowDebugger.Services.NativeWindows.Linux;
using WindowDebugger.Services.NativeWindows.Windows;
using WindowDebugger.Utils;

namespace WindowDebugger.Views;

public record MainViewModel : ReactiveRecord
{
    private readonly NativeWindowCollectionManager _nativeWindowCollectionManager;
    private string? _searchText;
    private bool _includingInvisibleWindow;
    private bool _includingEmptyTitleWindow;
    private bool _includingChildWindow;
    private bool _includingMessageOnlyWindow;

    public MainViewModel()
    {
        _nativeWindowCollectionManager = new();
    }

    public AvaloniaList<NativeTreeNode> NativeTree { get; } = [];

    public string? SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetWithAction(ref _searchText, value, ReloadWindows);
    }

    public bool IncludingInvisibleWindow
    {
        get => _includingInvisibleWindow;
        set => this.RaiseAndSetWithAction(ref _includingInvisibleWindow, value, ReloadWindows);
    }

    public bool IncludingEmptyTitleWindow
    {
        get => _includingEmptyTitleWindow;
        set => this.RaiseAndSetWithAction(ref _includingEmptyTitleWindow, value, ReloadWindows);
    }

    public bool IncludingChildWindow
    {
        get => _includingChildWindow;
        set => this.RaiseAndSetWithAction(ref _includingChildWindow, value, ReloadWindows);
    }

    public bool IncludingMessageOnlyWindow
    {
        get => _includingMessageOnlyWindow;
        set => this.RaiseAndSetWithAction(ref _includingMessageOnlyWindow, value, ReloadWindows);
    }

    public void ReloadWindows()
    {
        var nativeWindows = _nativeWindowCollectionManager.FindWindows(new WindowSearchingFilter(SearchText)
        {
            IncludingInvisibleWindow = IncludingInvisibleWindow,
            IncludingEmptyTitleWindow = IncludingEmptyTitleWindow,
            IncludingChildWindow = IncludingChildWindow,
            IncludingMessageOnlyWindow = IncludingMessageOnlyWindow,
        });

        var tree = BuildTree(nativeWindows);

        NativeTree.Clear();
        NativeTree.AddRange(tree);
    }

    private IEnumerable<NativeTreeNode> BuildTree(ImmutableArray<NativeWindowModel> nativeWindows)
    {
        if (OperatingSystem.IsLinux())
        {
            return nativeWindows.OfType<LinuxNativeWindowModel>().Select(x => new LinuxNativeWindowNode(x) { ChildWindows = [] });
        }
        else if (OperatingSystem.IsWindows())
        {
            var processWindowDictionary = new Dictionary<int, List<WindowsNativeWindowModel>>();
            foreach (var window in nativeWindows.OfType<WindowsNativeWindowModel>())
            {
                if (!processWindowDictionary.TryGetValue(window.ProcessId, out var processWindows))
                {
                    processWindows = [];
                    processWindowDictionary[window.ProcessId] = processWindows;
                }
                processWindows.Add(window);
            }
            foreach (var (pid, windows) in processWindowDictionary)
            {

            }
            return processWindowDictionary
                .Select(x => new NativeProcessNode(x.Key)
                {
                    ProcessName = TryGetProcessName(x.Key),
                    Windows =
                    [
                        ..x.Value.Select<WindowsNativeWindowModel, NativeWindowNode>(w => new WindowsNativeWindowNode(w)
                        {
                            ChildWindows = [],
                        }),
                    ]
                });
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }

    private static string? TryGetProcessName(int pid)
    {
        try
        {
            return Process.GetProcessById(pid).ProcessName;
        }
        catch
        {
            return null;
        }
    }
}
