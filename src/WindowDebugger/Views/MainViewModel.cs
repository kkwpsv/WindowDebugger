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

        NativeTree.Clear();
        NativeTree.AddRange(nativeWindows
            .Select<NativeWindowModel, NativeWindowNode>(x => x switch
            {
                WindowsNativeWindowModel wm => new WindowsNativeWindowNode(wm) { ChildWindows = [] },
                LinuxNativeWindowModel lm => new LinuxNativeWindowNode(lm) { ChildWindows = [] },
                _ => throw new PlatformNotSupportedException(),
            }));
    }
}
