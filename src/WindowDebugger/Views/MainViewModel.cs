using Avalonia.Collections;
using ReactiveUI;
using WindowDebugger.Services.NativeWindows;

namespace WindowDebugger.Views;

public record MainViewModel : ReactiveRecord
{
    public WindowListViewModel WindowList { get; }

    public AvaloniaList<NativeTreeNode> NativeTree { get; } = [];

    public MainViewModel()
    {
        WindowList = new WindowListViewModel(this);
    }

    public void ReloadWindows()
    {
        var tree = WindowList.ReloadWindows();
        NativeTree.Clear();
        NativeTree.AddRange(tree);
    }
}
