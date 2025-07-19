using System.Collections.Immutable;

namespace WindowDebugger.Services.NativeWindows;

public partial class NativeWindowCollectionManager
{
    public ImmutableArray<NativeWindowModel> FindWindows(WindowSearchingFilter filter)
    {
        if (OperatingSystem.IsLinux())
        {
            var list = FindWindowsOnLinux(filter)
                .OfType<NativeWindowModel>()
                .ToList();
            return [..list];
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
