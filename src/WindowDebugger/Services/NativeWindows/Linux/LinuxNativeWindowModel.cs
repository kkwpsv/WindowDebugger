using SeWzc.WinInfo.Models;
using SeWzc.X11Sharp;

namespace WindowDebugger.Services.NativeWindows.Linux;

public record LinuxNativeWindowModel : NativeWindowModel
{
    public LinuxNativeWindowModel(X11DisplayWindow window) : base(window.Window.ToPtrInt())
    {
        Window = window;
        Title = window.GetUtf8Property(Window.Display.Atoms.NetWmName) ?? "";
        Handle = window.Window.ToInt32();
        if (window.GetProperty(window.Display.InternAtom("_NET_WM_PID")) is PropertyData.Format32Array { Value: [var pid, ..] })
        {
            ProcessId = (int)pid;
        }
    }

    internal X11DisplayWindow Window { get; }

    public override string? Title { get; }

    public override int ProcessId { get; }

    public int Handle { get; }

    public IReadOnlyList<LinuxNativeWindowModel> GetChildren()
    {
        return Window.QueryTree(out _, out _, out var children)
            ? children.OrderBy(x => x.Window.ToInt32()).Select(child => new LinuxNativeWindowModel(child)).ToList()
            : [];
    }

    public IReadOnlyList<WindowProperty> GetWindowValues()
    {
        var properties = Window.ListProperties();
        var windowProperties = from property in properties
            let propertyData = Window.GetProperty(property)
            select new WindowProperty(Window, property);
        return windowProperties.ToList();
    }
}
