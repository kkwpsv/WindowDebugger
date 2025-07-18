using System.Collections.Immutable;
using System.Diagnostics;
using ReactiveUI;
using WindowDebugger.Services.NativeWindows;
using WindowDebugger.Services.NativeWindows.Linux;
using WindowDebugger.Services.NativeWindows.Windows;
using WindowDebugger.Utils;

namespace WindowDebugger.Views;

public record WindowListViewModel : ReactiveRecord
{
    private readonly MainViewModel _owner;
    private readonly NativeWindowCollectionManager _nativeWindowCollectionManager;
    private string? _searchText;
    private bool _includingInvisibleWindow;
    private bool _includingEmptyTitleWindow;
    private bool _includingChildWindow;
    private bool _includingMessageOnlyWindow;

    public WindowListViewModel(MainViewModel owner)
    {
        _owner = owner;
        _nativeWindowCollectionManager = new();
    }

    public string? SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetWithAction(ref _searchText, value, _owner.ReloadWindows);
    }

    public bool IncludingInvisibleWindow
    {
        get => _includingInvisibleWindow;
        set => this.RaiseAndSetWithAction(ref _includingInvisibleWindow, value, _owner.ReloadWindows);
    }

    public bool IncludingEmptyTitleWindow
    {
        get => _includingEmptyTitleWindow;
        set => this.RaiseAndSetWithAction(ref _includingEmptyTitleWindow, value, _owner.ReloadWindows);
    }

    public bool IncludingChildWindow
    {
        get => _includingChildWindow;
        set => this.RaiseAndSetWithAction(ref _includingChildWindow, value, _owner.ReloadWindows);
    }

    public bool IncludingMessageOnlyWindow
    {
        get => _includingMessageOnlyWindow;
        set => this.RaiseAndSetWithAction(ref _includingMessageOnlyWindow, value, _owner.ReloadWindows);
    }

    public IEnumerable<NativeTreeNode> ReloadWindows()
    {
        var nativeWindows = _nativeWindowCollectionManager.FindWindows(new WindowSearchingFilter(SearchText)
        {
            Including = GetWindowIncluding(),
        });

        return BuildTree(nativeWindows);
    }

    public WindowIncluding GetWindowIncluding()
    {
        var value = WindowIncluding.Normal;

        if (IncludingInvisibleWindow)
        {
            value |= WindowIncluding.InvisibleWindow;
        }

        if (IncludingEmptyTitleWindow)
        {
            value |= WindowIncluding.EmptyTitleWindow;
        }

        if (IncludingChildWindow)
        {
            value |= WindowIncluding.ChildWindow;
        }

        if (IncludingMessageOnlyWindow)
        {
            value |= WindowIncluding.MessageOnlyWindow;
        }

        return value;
    }

    private IEnumerable<NativeTreeNode> BuildTree(ImmutableArray<NativeWindowModel> nativeWindows)
    {
        if (OperatingSystem.IsLinux())
        {
            var processWindowDictionary = GroupByProcess<LinuxNativeWindowModel>(nativeWindows);
            return processWindowDictionary
                .Select(x => new NativeProcessNode(x.Key)
                {
                    ProcessName = TryGetProcessName(x.Key),
                    Windows = [..x.Value.Select(ConvertModelToNode)],
                });
        }

        if (OperatingSystem.IsWindows())
        {
            var processWindowDictionary = GroupByProcess<WindowsNativeWindowModel>(nativeWindows);
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
                    ],
                });
        }

        throw new PlatformNotSupportedException();
    }

    private static NativeWindowNode ConvertModelToNode(LinuxNativeWindowModel model)
    {
        return new LinuxNativeWindowNode(model)
        {
            ChildWindows = [..model.GetChildren().Select(ConvertModelToNode)],
        };
    }

    private static Dictionary<int, List<T>> GroupByProcess<T>(ImmutableArray<NativeWindowModel> nativeWindows)
        where T : NativeWindowModel
    {
        var processWindowDictionary = new Dictionary<int, List<T>>();
        foreach (var window in nativeWindows.OfType<T>())
        {
            if (!processWindowDictionary.TryGetValue(window.ProcessId, out var processWindows))
            {
                processWindows = [];
                processWindowDictionary[window.ProcessId] = processWindows;
            }
            processWindows.Add(window);
        }
        return processWindowDictionary;
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
