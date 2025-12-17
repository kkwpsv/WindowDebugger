using Lsj.Util.Win32.BaseTypes;
using Lsj.Util.Win32.Enums;
using static Lsj.Util.Win32.Enums.EventConstants;
using static Lsj.Util.Win32.Enums.SetWinEventHookFlags;
using static Lsj.Util.Win32.User32;

namespace WindowDebugger.Utils;

/// <summary>
/// 前台窗口跟踪器，用于监视系统前台窗口的变化。
/// </summary>
public class ForegroundWindowTracker
{
    private readonly Wineventproc _winEventProc;

    private HWINEVENTHOOK _hookHandle;

    public ForegroundWindowTracker()
    {
        _winEventProc = WinEventProc;
    }

    /// <summary>
    /// 获取或设置是否启用前台窗口跟踪。
    /// </summary>
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

        _hookHandle = SetWinEventHook(
            EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
            default, _winEventProc,
            0, 0,
            WINEVENT_OUTOFCONTEXT);
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
        ForegroundWindowChanged?.Invoke(this, current);
    }
}
