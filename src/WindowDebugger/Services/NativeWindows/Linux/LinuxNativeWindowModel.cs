using SeWzc.X11Sharp;
using WindowDebugger.Services.NativeWindows.Linux.Models;

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

        _windowAttributes = window.GetAttributes() ?? throw new ArgumentException("Failed to get window attributes.");
    }

    internal X11DisplayWindow Window { get; }

    public override string? Title { get; }

    public override int ProcessId { get; }

    public int Handle { get; }

    #region attributes

    private readonly WindowAttributes _windowAttributes;

    public int Left => _windowAttributes.X;

    public int Top => _windowAttributes.Y;

    public int Width => _windowAttributes.Width;

    public int Height => _windowAttributes.Height;

    public int Depth => _windowAttributes.Depth;

    public int BorderWidth => _windowAttributes.BorderWidth;

    public WindowClasses WindowClasses => _windowAttributes.WindowClass;

    public MapState MapState => _windowAttributes.MapState;

    public bool OverrideRedirect => _windowAttributes.OverrideRedirect;

    #endregion

    public IEnumerable<IGrouping<string, WindowProperty>> WindowPropertyGroups => GetWindowValuesGrouped();

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

    public IEnumerable<IGrouping<string, WindowProperty>> GetWindowValuesGrouped()
    {
        return GetWindowValues()
            .ToLookup(x =>
            {
                var name = x.Name;
                if (!name.StartsWith('_'))
                    return "普通属性";
                var index = name.IndexOf('_', 1);
                return "扩展属性：" +  (index > 0 ? name[1..index] : name[1..]);
            });
    }
}
