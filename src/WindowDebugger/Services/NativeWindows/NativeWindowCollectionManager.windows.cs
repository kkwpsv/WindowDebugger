using System.Runtime.Versioning;
using Lsj.Util.Win32;
using Lsj.Util.Win32.BaseTypes;
using Lsj.Util.Win32.Extensions.NativeUI;
using Lsj.Util.Win32.NativeUI;
using WindowDebugger.Services.NativeWindows.Windows;

namespace WindowDebugger.Services.NativeWindows;

public partial class NativeWindowCollectionManager
{
    [SupportedOSPlatform("windows")]
    private WindowsNativeWindowModel[] FindWindowsOnWindows(WindowSearchingFilter filter)
    {
        return WindowExtensions.GetAllWindow(
            x => new WindowsNativeWindowModel(x),
            hwnd => WindowFilter(hwnd, filter),
            hwnd => DescendantsFilter(hwnd, filter));
    }

    [SupportedOSPlatform("windows")]
    private (bool includeSelf, bool includeDescendants, bool continueEnum) WindowFilter(HWND handle, WindowSearchingFilter filter)
    {
        return (DescendantsFilter(handle, filter).includeSelf, filter.Including.HasFlag(WindowIncluding.ChildWindow), true);
    }

    [SupportedOSPlatform("windows")]
    private (bool includeSelf, bool continueEnum) DescendantsFilter(HWND handle, WindowSearchingFilter filter)
    {
        var win = new Win32Window(handle);

        if (!filter.Including.HasFlag(WindowIncluding.MessageOnlyWindow))
        {
            if (win.ParentWindowHandle == User32.HWND_MESSAGE)
            {
                return (false, true);
            }
        }

        if (!filter.Including.HasFlag(WindowIncluding.InvisibleWindow))
        {
            if (!win.IsVisible)
            {
                return (false, true);
            }
        }

        var text = win.Text;
        if (!filter.Including.HasFlag(WindowIncluding.EmptyTitleWindow))
        {
            if (string.IsNullOrEmpty(text))
            {
                return (false, true);
            }
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            if ((text.IndexOf(filter.SearchText, StringComparison.CurrentCultureIgnoreCase) > -1)
                || (win.ProcessName.IndexOf(filter.SearchText, StringComparison.CurrentCultureIgnoreCase) > -1))
            {
                return (true, true);
            }

            if (filter.SearchingValueAsHex is { } uintHexVal)
            {
                if (((IntPtr)handle).SafeToUInt32() == uintHexVal || win.ProcessID == uintHexVal || win.ThreadID == uintHexVal
                    || ((IntPtr)win.OwnerWindowHandle).SafeToUInt32() == uintHexVal || ((IntPtr)win.ParentWindowHandle).SafeToUInt32() == uintHexVal)
                {
                    return (true, true);
                }
            }

            if (filter.SearchingValueAsDecimal is { } uintVal)
            {
                if (((IntPtr)handle).SafeToUInt32() == uintVal || win.ProcessID == uintVal || win.ThreadID == uintVal
                    || ((IntPtr)win.OwnerWindowHandle).SafeToUInt32() == uintVal || ((IntPtr)win.ParentWindowHandle).SafeToUInt32() == uintVal)
                {
                    return (true, true);
                }
            }

            return (false, true);
        }
        else
        {
            return (true, true);
        }
    }
}
