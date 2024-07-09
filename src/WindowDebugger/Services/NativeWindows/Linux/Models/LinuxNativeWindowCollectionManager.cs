using SeWzc.X11Sharp;

namespace SeWzc.WinInfo.Models;

public class LinuxNativeWindowCollectionManager
{
    private readonly X11Display _display = X11Display.Open();

    public IReadOnlyList<WindowProxy> GetWindows()
    {
        var rootWindow = _display.DefaultRootWindow;
        return [new WindowProxy(rootWindow)];
    }
}
