using System.Collections.Immutable;
using Lsj.Util.Win32.Extensions;
using WindowDebugger.Services.NativeWindows.Windows;

namespace WindowDebugger.Services.NativeWindows;

public class NativeWindowCollectionManager
{
    public ImmutableArray<NativeWindowModel> FindWindows()
    {
        if (OperatingSystem.IsLinux())
        {
            return [];
        }

        if (OperatingSystem.IsWindows())
        {
            var list = WindowExtensions.GetAllWindowHandle()
#pragma warning disable CA1416
                .Select(x => new WindowsNativeWindowModel(x))
#pragma warning restore CA1416
                .OfType<NativeWindowModel>()
                .ToList();
            return [..list];
        }

        throw new PlatformNotSupportedException();
    }
}
