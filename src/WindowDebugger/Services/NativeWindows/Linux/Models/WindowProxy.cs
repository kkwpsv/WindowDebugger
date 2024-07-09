using System.Collections.Generic;
using System.Linq;
using SeWzc.X11Sharp;

namespace SeWzc.WinInfo.Models;

public class WindowProxy
{
    public WindowProxy(X11DisplayWindow window)
    {
        Window = window;
        Name = Window.GetUtf8Property(Window.Display.Atoms.NetWmName) ?? "";
        Handle = Window.Window.ToInt32();
    }

    internal X11DisplayWindow Window { get; }

    public string Name { get; }

    public int Handle { get; }

    public IReadOnlyList<WindowProxy> GetChildren()
    {
        return Window.QueryTree(out _, out _, out var children)
            ? children.OrderBy(x => x.Window.ToInt32()).Select(child => new WindowProxy(child)).ToList()
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