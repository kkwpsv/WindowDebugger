using System.Collections.Immutable;
using Avalonia.Collections;
using ReactiveUI;
using WindowDebugger.Services.NativeWindows;

namespace WindowDebugger.Views;

public record MainViewModel : ReactiveRecord
{
    private readonly NativeWindowCollectionManager _nativeWindowCollectionManager;
    private ImmutableArray<NativeTreeNode> _tree = [];

    public MainViewModel()
    {
        _nativeWindowCollectionManager = new();
    }

    public AvaloniaList<NativeWindowModel> NativeWindows { get; } = [];

    public ImmutableArray<NativeTreeNode> Tree
    {
        get => _tree;
        private set => this.RaiseAndSetIfChanged(ref _tree, value);
    }

    public async Task ReloadWindows()
    {
        var nativeWindows = _nativeWindowCollectionManager.FindWindows();

        NativeWindows.Clear();
        NativeWindows.AddRange(nativeWindows);
    }
}
