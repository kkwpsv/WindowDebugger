using System.Runtime.Versioning;
using SeWzc.X11Sharp;
using WindowDebugger.Services.NativeWindows.Linux;

namespace WindowDebugger.Services.NativeWindows;

partial class NativeWindowCollectionManager
{
    [SupportedOSPlatform("linux")]
    private X11Display? _display;

    [SupportedOSPlatform("linux")]
    private LinuxNativeWindowModel[] FindWindowsOnLinux(WindowSearchingFilter filter)
    {
        var display = _display ??= X11Display.Open();

        return [..new LinuxNativeWindowModel(display.DefaultRootWindow)
            .GetChildren()
            .Select(x=>new LinuxNativeWindowModel(x.Window))];
    }
}
