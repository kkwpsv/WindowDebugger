using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ReactiveUI;
using WindowDebugger.Localizations;
using WindowDebugger.Services.NativeWindows;
using WindowDebugger.Services.NativeWindows.Linux;
using WindowDebugger.Services.NativeWindows.Windows;
using WindowDebugger.Utils;
using WindowDebugger.Views.Converters;
using NativeWindowGrouping = WindowDebugger.Services.NativeWindows.WindowGrouping;
using NativeWindowSorting = WindowDebugger.Services.NativeWindows.WindowSorting;

namespace WindowDebugger.Views;

public class WindowListViewModel : ReactiveObject
{
    private readonly MainViewModel _owner;
    private readonly NativeWindowCollectionManager _nativeWindowCollectionManager;
    private string? _searchText;
    private bool _includingInvisibleWindow;
    private bool _includingEmptyTitleWindow;
    private bool _includingChildWindow;
    private bool _includingMessageOnlyWindow;
    private bool _isGroupedByProcess;
    private NativeWindowGrouping _windowGrouping = NativeWindowGrouping.PlainList;
    private NativeWindowSorting _windowSorting = NativeWindowSorting.Raw;
    private string _localizedWindowIncluding;
    private string _localizedWindowGrouping;
    private string _localizedSorting;

    public WindowListViewModel(MainViewModel owner)
    {
        _owner = owner;
        _nativeWindowCollectionManager = new();
        ReloadSearchingFilter();
    }

    public string? SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetWithAction(ref _searchText, value, ReloadAll);
    }

    public bool IncludingInvisibleWindow
    {
        get => _includingInvisibleWindow;
        set => this.RaiseAndSetWithAction(ref _includingInvisibleWindow, value, ReloadAll);
    }

    public bool IncludingEmptyTitleWindow
    {
        get => _includingEmptyTitleWindow;
        set => this.RaiseAndSetWithAction(ref _includingEmptyTitleWindow, value, ReloadAll);
    }

    public bool IncludingChildWindow
    {
        get => _includingChildWindow;
        set => this.RaiseAndSetWithAction(ref _includingChildWindow, value, ReloadAll);
    }

    public bool IncludingMessageOnlyWindow
    {
        get => _includingMessageOnlyWindow;
        set => this.RaiseAndSetWithAction(ref _includingMessageOnlyWindow, value, ReloadAll);
    }

    public bool IsGroupedByProcess
    {
        get => _isGroupedByProcess;
        private set => this.RaiseAndSetIfChanged(ref _isGroupedByProcess, value);
    }

    public NativeWindowGrouping? WindowGrouping
    {
        get => _windowGrouping;
        set
        {
            if (this.SetCheckedField(ref _windowGrouping, value, out var changedValue))
            {
                IsGroupedByProcess = changedValue is NativeWindowGrouping.ProcessThenWindow or NativeWindowGrouping.ProcessThenWindowTree;
                ReloadAll();
            }
        }
    }

    public NativeWindowSorting? WindowSorting
    {
        get => _windowSorting;
        set
        {
            if (this.SetCheckedField(ref _windowSorting, value, out var changedValue))
            {
                ReloadAll();
            }
        }
    }

    public string LocalizedWindowIncluding
    {
        get => _localizedWindowIncluding;
        private set => this.RaiseAndSetIfChanged(ref _localizedWindowIncluding, value);
    }

    public string LocalizedWindowGrouping
    {
        get => _localizedWindowGrouping;
        private set => this.RaiseAndSetIfChanged(ref _localizedWindowGrouping, value);
    }

    public string LocalizedSorting
    {
        get => _localizedSorting;
        private set => this.RaiseAndSetIfChanged(ref _localizedSorting, value);
    }

    private void ReloadAll()
    {
        ReloadSearchingFilter();
        _owner.ReloadWindows();
    }

#pragma warning disable CS8774 // 退出时，成员必须具有非 null 值。
    [MemberNotNull(nameof(_localizedWindowIncluding), nameof(_localizedWindowGrouping), nameof(_localizedSorting))]
    private void ReloadSearchingFilter()
    {
        LocalizedWindowIncluding = (IncludingInvisibleWindow, IncludingEmptyTitleWindow, IncludingChildWindow, IncludingMessageOnlyWindow) switch
        {
            (false, false, false, false) => Lang.Current.App.UI.Filter.IncludingNoneSlim,
            (true, true, true, true) => Lang.Current.App.UI.Filter.IncludingAllSlim,
            _ => Lang.Current.App.UI.Filter.IncludingPartialSlim,
        };
        LocalizedWindowGrouping = WindowGrouping switch
        {
            NativeWindowGrouping.ProcessThenWindow => Lang.Current.App.UI.Filter.GroupByProcessThenWindowSlim,
            NativeWindowGrouping.ProcessThenWindowTree => Lang.Current.App.UI.Filter.GroupByProcessThenWindowTreeSlim,
            NativeWindowGrouping.WindowTree => Lang.Current.App.UI.Filter.GroupByWindowTreeSlim,
            _ => Lang.Current.App.UI.Filter.GroupByPlainListSlim,
        };
        LocalizedSorting = WindowSorting switch
        {
            NativeWindowSorting.AscendingById => Lang.Current.App.UI.Filter.SortByIdSlim,
            NativeWindowSorting.AscendingByTitle => Lang.Current.App.UI.Filter.SortByTitleSlim,
            _ => Lang.Current.App.UI.Filter.SortByRawSlim,
        };
    }
#pragma warning restore CS8774 // 退出时，成员必须具有非 null 值。

    public IEnumerable<NativeTreeNode> ReloadWindows()
    {
        var filter = GetSearchingFilter();
        var nativeWindows = _nativeWindowCollectionManager.FindWindows(filter);
        return BuildTree(nativeWindows);
    }

    public WindowSearchingFilter GetSearchingFilter()
    {
        return new WindowSearchingFilter(SearchText)
        {
            Including = GetWindowIncluding(),
            Grouping = _windowGrouping,
        };
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
            IEnumerable<NativeTreeNode> tree =
                _windowGrouping is NativeWindowGrouping.ProcessThenWindow or NativeWindowGrouping.ProcessThenWindowTree
                    ? GroupByProcess<WindowsNativeWindowModel>(nativeWindows)
                        .Select(x => new NativeProcessNode(x.Key)
                        {
                            ProcessName = TryGetProcessName(x.Key),
                            Windows =
                            [
                                ..x.Value.NestBy(_windowGrouping, _windowSorting),
                            ],
                        })
                    : nativeWindows.OfType<WindowsNativeWindowModel>().NestBy(_windowGrouping, _windowSorting);
            return tree;
        }

        throw new PlatformNotSupportedException();
    }

    private NativeWindowNode ConvertModelToNode(LinuxNativeWindowModel model)
    {
        return new LinuxNativeWindowNode(model)
        {
            ChildWindows = [..model.GetChildren().OrderBy(_windowSorting).Select(ConvertModelToNode)],
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

file static class Extensions
{
    public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> source, NativeWindowSorting sorting)
        where T : NativeWindowModel
    {
        if (sorting is WindowSorting.Raw)
        {
            return source;
        }
        return sorting switch
        {
            NativeWindowSorting.AscendingById => source.OrderBy(w => w.Id),
            NativeWindowSorting.AscendingByTitle => source.OrderBy(w => w.Title, StringComparer.OrdinalIgnoreCase),
            _ => throw new ArgumentOutOfRangeException(nameof(sorting), sorting, null),
        };
    }

    public static IEnumerable<WindowsNativeWindowNode> NestBy(this IEnumerable<WindowsNativeWindowModel> source,
        NativeWindowGrouping grouping, NativeWindowSorting sorting)
    {
        var isNested = grouping is NativeWindowGrouping.WindowTree or NativeWindowGrouping.ProcessThenWindowTree;
        if (!isNested)
        {
            return source.OrderBy(sorting).Select(w => new WindowsNativeWindowNode(w)
            {
                ChildWindows = [],
            });
        }

        var sourceList = (source as IReadOnlyList<WindowsNativeWindowModel>) ?? source.ToList();

        var hwndMap = new Dictionary<nint, List<WindowsNativeWindowModel>>();
        foreach (var model in sourceList)
        {
            if (hwndMap.TryGetValue(model.ParentWindowHandle, out var existedList))
            {
                existedList.Add(model);
            }
            else
            {
                existedList = [model];
                hwndMap[model.ParentWindowHandle] = existedList;
            }
        }

        var allHandles = sourceList.Select(w => w.Id).ToHashSet();
        var roots = sourceList.Where(w => !allHandles.Contains(w.ParentWindowHandle));

        return roots.OrderBy(sorting).Select(w => new WindowsNativeWindowNode(w)
        {
            ChildWindows = [..BuildTree(w.Id)],
        });

        IEnumerable<WindowsNativeWindowNode> BuildTree(nint parent)
        {
            if (!hwndMap.TryGetValue(parent, out var children))
            {
                yield break;
            }
            foreach (var child in children.OrderBy(sorting))
            {
                yield return new WindowsNativeWindowNode(child)
                {
                    ChildWindows = [..BuildTree(child.Id)],
                };
            }
        }
    }
}
