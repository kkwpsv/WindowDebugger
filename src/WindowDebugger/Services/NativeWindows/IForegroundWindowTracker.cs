using System.Runtime.Versioning;
using Lsj.Util.Win32.BaseTypes;
using Lsj.Util.Win32.Enums;
using static Lsj.Util.Win32.Enums.EventConstants;
using static Lsj.Util.Win32.Enums.SetWinEventHookFlags;
using static Lsj.Util.Win32.User32;

namespace WindowDebugger.Services.NativeWindows;

/// <summary>
/// 前台窗口跟踪器，用于监视系统前台窗口的变化。
/// </summary>
public interface IForegroundWindowTracker
{
    /// <summary>
    /// 获取或设置是否允许跟踪自身应用的前台窗口变化。
    /// </summary>
    bool AllowsTrackSelf { get; set; }

    /// <summary>
    /// 获取或设置是否启用前台窗口跟踪。
    /// </summary>
    bool IsEnabled { get; set; }
}

/// <summary>
/// 未实现任何功能的前台窗口跟踪器。
/// </summary>
public class EmptyForegroundWindowTracker : IForegroundWindowTracker
{
    /// <inheritdoc />
    public bool AllowsTrackSelf { get; set; }

    /// <inheritdoc />
    public bool IsEnabled { get; set; }
}

/// <summary>
/// Windows 平台专属前台窗口跟踪器，用于监视系统前台窗口的变化。
/// </summary>
[SupportedOSPlatform("windows")]
public class WindowsForegroundWindowTracker : IForegroundWindowTracker
{
    private readonly Wineventproc _winEventProc;

    private HWINEVENTHOOK _hookHandle;

    public WindowsForegroundWindowTracker()
    {
        _winEventProc = WinEventProc;
    }

    /// <inheritdoc />
    public bool AllowsTrackSelf { get; set; }

    /// <inheritdoc />
    public bool IsEnabled
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;

            if (value)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }
    }

    /// <summary>
    /// 当前台窗口改变时引发事件。
    /// </summary>
    public event EventHandler<HWND>? ForegroundWindowChanged;

    private void Enable()
    {
        if (_hookHandle != 0)
        {
            Disable();
        }

        var flags = AllowsTrackSelf
            ? WINEVENT_OUTOFCONTEXT
            : WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS;
        _hookHandle = SetWinEventHook(
            EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
            default, _winEventProc,
            0, 0,
            flags);
    }

    private void Disable()
    {
        if (_hookHandle != 0)
        {
            UnhookWinEvent(_hookHandle);
            _hookHandle = default;
        }
    }

    private void WinEventProc(HWINEVENTHOOK hWinEventHook, EventConstants @event, HWND hwnd,
        LONG idObject, LONG idChild, DWORD idEventThread, DWORD dwmsEventTime)
    {
        var current = GetForegroundWindow();
        try
        {
            ForegroundWindowChanged?.Invoke(this, current);
        }
        catch (Exception ex)
        {
            // 来自非托管代码的回调，捕获所有异常以防止崩溃。
        }
    }
}
