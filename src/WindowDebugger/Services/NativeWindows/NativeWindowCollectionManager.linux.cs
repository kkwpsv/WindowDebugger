using System.Runtime.Versioning;
using WindowDebugger.Services.NativeWindows.Windows;

namespace WindowDebugger.Services.NativeWindows;

partial class NativeWindowCollectionManager
{
    [SupportedOSPlatform("linux")]
    private WindowsNativeWindowModel[] FindWindowsOnLinux(WindowSearchingFilter filter)
    {
        return [];
    }
}
