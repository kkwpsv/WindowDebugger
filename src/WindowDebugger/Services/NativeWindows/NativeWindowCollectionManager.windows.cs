using System.Globalization;
using System.Runtime.Versioning;
using Lsj.Util.Text;
using Lsj.Util.Win32;
using Lsj.Util.Win32.BaseTypes;
using Lsj.Util.Win32.Extensions;
using Lsj.Util.Win32.NativeUI;
using WindowDebugger.Services.NativeWindows.Windows;

namespace WindowDebugger.Services.NativeWindows;

public partial class NativeWindowCollectionManager
{
    private uint? _searchStringHexUintValue;
    private uint? _searchStringUintValue;

    [SupportedOSPlatform("windows")]
    private WindowsNativeWindowModel[] FindWindowsOnWindows(WindowSearchingFilter filter)
    {
        if (!filter.SearchText.IsNullOrEmpty())
        {
            _searchStringHexUintValue =
                uint.TryParse(filter.SearchText, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var uintHexVal)
                || (filter.SearchText.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase) && uint.TryParse(filter.SearchText, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out uintHexVal))
                    ? uintHexVal
                    : null;

            _searchStringUintValue = uint.TryParse(filter.SearchText, out var uintVal) ? uintVal : null;
        }
        else
        {
            _searchStringHexUintValue = null;
            _searchStringUintValue = null;
        }

        return WindowExtensions.GetAllWindow(
            x => new WindowsNativeWindowModel(x),
            hwnd => WindowFilter(hwnd, filter),
            hwnd => DescendantsFilter(hwnd, filter));
    }

    [SupportedOSPlatform("windows")]
    private (bool includeSelf, bool includeDescendants, bool continueEnum) WindowFilter(HWND handle, WindowSearchingFilter filter)
    {
        return (DescendantsFilter(handle, filter).includeSelf, filter.IncludingChildWindow, true);
    }

    [SupportedOSPlatform("windows")]
    private (bool includeSelf, bool continueEnum) DescendantsFilter(HWND handle, WindowSearchingFilter filter)
    {
        var win = new Win32Window(handle);

        if (!filter.IncludingMessageOnlyWindow)
        {
            if (win.ParentWindowHandle == User32.HWND_MESSAGE)
            {
                return (false, true);
            }
        }

        if (!filter.IncludingInvisibleWindow)
        {
            if (!win.IsVisible)
            {
                return (false, true);
            }
        }

        var text = win.Text;
        if (!filter.IncludingEmptyTitleWindow)
        {
            if (text.IsNullOrEmpty())
            {
                return (false, true);
            }
        }

        if (!filter.SearchText.IsNullOrEmpty())
        {
            if ((text.IndexOf(filter.SearchText, StringComparison.CurrentCultureIgnoreCase) > -1)
                || (win.ProcessName.IndexOf(filter.SearchText, StringComparison.CurrentCultureIgnoreCase) > -1))
            {
                return (true, true);
            }

            if (_searchStringHexUintValue is { } uintHexVal)
            {
                if (((IntPtr)handle).SafeToUInt32() == uintHexVal || win.ProcessID == uintHexVal || win.ThreadID == uintHexVal
                    || ((IntPtr)win.OwnerWindowHandle).SafeToUInt32() == uintHexVal || ((IntPtr)win.ParentWindowHandle).SafeToUInt32() == uintHexVal)
                {
                    return (true, true);
                }
            }

            if (_searchStringUintValue is { } uintVal)
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
