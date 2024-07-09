namespace WindowDebugger.Services.NativeWindows.Linux;

public record LinuxNativeWindowModel(nint Id) : NativeWindowModel(Id)
{
    public override string? Title { get; }

    public override int ProcessId { get; }

    public override nint GetParent()
    {
        return 0;
    }
}
