using ReactiveUI;

namespace WindowDebugger.Services.NativeWindows;

public abstract record NativeWindowModel(nint Id) : ReactiveRecord
{
    public abstract string? Title { get; }

    public abstract int ProcessId { get; }

    public abstract nint GetParent();
}
