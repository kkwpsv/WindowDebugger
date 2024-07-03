using ReactiveUI;

namespace WindowDebugger.Services.NativeWindows;

public record NativeWindowModel(nint Id) : ReactiveRecord;
