using ReactiveUI;

namespace WindowDebugger.Services.NativeWindows;

public abstract record NativeWindowModel(NativeWindowId Id) : ReactiveRecord
{
    public abstract string? Title { get; }

    public abstract int ProcessId { get; }
}
