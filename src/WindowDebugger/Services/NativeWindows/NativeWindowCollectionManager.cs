using System.Collections.Immutable;
using Lsj.Util.Win32.Extensions;
using WindowDebugger.Services.NativeWindows.Windows;

namespace WindowDebugger.Services.NativeWindows;

public partial class NativeWindowCollectionManager
{
    public ImmutableArray<NativeWindowModel> FindWindows(WindowSearchingFilter filter)
    {
        if (OperatingSystem.IsLinux())
        {
            return [];
        }

        if (OperatingSystem.IsWindows())
        {
            var list = FindWindowsOnWindows(filter)
                .OfType<NativeWindowModel>()
                .ToList();
            return [..list];
        }

        throw new PlatformNotSupportedException();
    }
}
