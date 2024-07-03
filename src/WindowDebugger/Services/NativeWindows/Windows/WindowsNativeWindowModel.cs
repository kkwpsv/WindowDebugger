using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Avalonia.Media.Imaging;
using Lsj.Util.Win32;
using Lsj.Util.Win32.BaseTypes;
using Lsj.Util.Win32.Enums;
using Lsj.Util.Win32.Extensions;
using Lsj.Util.Win32.NativeUI;
using Lsj.Util.Win32.Structs;
using ReactiveUI;
using static Lsj.Util.Win32.BaseTypes.HRESULT;
using static Lsj.Util.Win32.Constants;
using static Lsj.Util.Win32.Enums.DPI_AWARENESS;
using static Lsj.Util.Win32.Enums.ProcessAccessRights;
using static Lsj.Util.Win32.Enums.RedrawWindowFlags;
using static Lsj.Util.Win32.Enums.ThreadAccessRights;
using static Lsj.Util.Win32.Enums.WindowMessages;
using static Lsj.Util.Win32.Kernel32;
using static Lsj.Util.Win32.UnsafePInvokeExtensions;
using static Lsj.Util.Win32.User32;

namespace WindowDebugger.Services.NativeWindows.Windows;

[SupportedOSPlatform("windows")]
public record WindowsNativeWindowModel : NativeWindowModel
{
    private readonly Win32Window _window;
    private string? _errorString;
    private Bitmap? _screenshot;

    public WindowsNativeWindowModel(HWND hwnd) : base(hwnd)
    {
        _window = new Win32Window(hwnd);
        _window.CustomErrorHandler = OnError;
    }

    public nint WindowHandle => _window.Handle;

    public string? ErrorString
    {
        get => _errorString;
        set => this.RaiseAndSetIfChanged(ref _errorString, value);
    }

    public string Text
    {
        get => _window.Text;
        set => _window.Text = value;
    }

    public WindowStyles Styles
    {
        get => _window.WindowStyles;
        set
        {
            _window.WindowStyles = value;
            this.RaisePropertyChanged();
        }
    }

    public WindowStylesEx StylesEx
    {
        get => _window.WindowStylesEx;
        set
        {
            _window.WindowStylesEx = value;
            this.RaisePropertyChanged();
        }
    }

    public ClassStyles ClassStyles
    {
        get => _window.ClassStyles;
        set
        {
            _window.ClassStyles = value;
            this.RaisePropertyChanged();
        }
    }

    public int ProcessID
    {
        get => _window.ProcessID;
    }

    public string ProcessName
    {
        get => _window.ProcessName;
    }

    public int ThreadID
    {
        get => _window.ThreadID;
    }

    public string ClassName
    {
        get => _window.ClassName;
    }

    public int Left
    {
        get => _window.Rect.left;
    }

    public int Top
    {
        get => _window.Rect.top;
    }

    public int Width
    {
        get => _window.Rect.right - _window.Rect.left;
    }

    public int Height
    {
        get => _window.Rect.bottom - _window.Rect.top;
    }

    public ShowWindowCommands WindowShowStates
    {
        get => _window.ShowStates;
        set
        {
            _window.ShowStates = value;
            this.RaisePropertyChanged();
        }
    }

    public WindowDisplayAffinities WindowDisplayAffinity
    {
        get => ProcessID == Process.GetCurrentProcess().Id && (Styles & WindowStyles.WS_CHILD) == 0 ? _window.DisplayAffinity : default;
        set
        {
            _window.DisplayAffinity = value;
            this.RaisePropertyChanged();
        }
    }

    public DPI_AWARENESS DpiAwareness => GetWithDefaultValueWhenException(() => _window.DpiAwareness, DPI_AWARENESS_UNAWARE);

    public int Dpi => _window.Dpi;

    public nint ParentWindowHandle
    {
        get => _window.ParentWindowHandle;
        set
        {
            _window.ParentWindowHandle = value;
            this.RaisePropertyChanged(nameof(DpiAwareness)); //may be reset by system
        }
    }

    public nint OwnerWindowHandle
    {
        get => _window.OwnerWindowHandle;
        set => _window.OwnerWindowHandle = value;
    }


    public Bitmap? Screenshot
    {
        get => _screenshot;
        set => this.RaiseAndSetIfChanged(ref _screenshot, value);
    }

    public DWMInfo DWMInfo
    {
        get => _window.ParentWindowHandle == GetDesktopWindow() ? _window.DWMInfo : null;
    }

    public bool IsTouchWindow
    {
        get => _window.IsTouchWindow;
    }

    public string VirtualDesktopID => GetWithDefaultValueWhenException(() => _window.DesktopID.ToString(), null);

    private void SetError()
    {
        ErrorString = ErrorMessageExtensions.GetSystemErrorMessageFromCode((uint)Marshal.GetLastWin32Error());
    }

    public bool SetForeground()
    {
        var result = SetForegroundWindow(WindowHandle);
        if (!result)
        {
            SetError();
        }
        return result;
    }

    public void CloseWindow()
    {
        PostMessage(WindowHandle, WM_CLOSE, 0, NULL);
    }

    public bool RedrawWindow()
    {
        return User32.RedrawWindow(WindowHandle, NullRef<RECT>(), NULL, RDW_INVALIDATE);
    }

    public void FlashWindow()
    {
        User32.FlashWindow(WindowHandle, true);
    }

    public void KillProcess()
    {
        var process = OpenProcess(PROCESS_TERMINATE, false, ProcessID);
        if (process == NULL || !TerminateProcess(process, 0xFFFFFFFF) || !CloseHandle(process))
        {
            SetError();
        }
    }

    public void ForceKillProcess()
    {
        var process = OpenProcess(PROCESS_CREATE_THREAD, false, ProcessID);
        if (process != NULL)
        {
            var threadHandle = CreateRemoteThread(process, NullRef<SECURITY_ATTRIBUTES>(), 0, null, NULL, 0, out NullRef<DWORD>());
            if (threadHandle == NULL || !CloseHandle(threadHandle))
            {
                SetError();
            }
            if (!CloseHandle(process))
            {
                SetError();
            }
        }
        else
        {
            SetError();
        }
    }

    public void SuspendThread()
    {
        var thread = OpenThread(THREAD_SUSPEND_RESUME, false, ThreadID);
        if (thread == NULL || Kernel32.SuspendThread(thread) == (DWORD)(-1) || !CloseHandle(thread))
        {
            SetError();
        }
    }

    public void ResumeThread()
    {
        var thread = OpenThread(THREAD_SUSPEND_RESUME, false, ThreadID);
        if (thread == NULL || Kernel32.ResumeThread(thread) == (DWORD)(-1) || !CloseHandle(thread))
        {
            SetError();
        }
    }

    public void KillThread()
    {
        var thread = OpenThread(THREAD_TERMINATE, false, ThreadID);
        if (thread != NULL)
        {
            TerminateThread(thread, 0xFFFFFFFF);
            CloseHandle(thread);
        }
    }

    public void SetTopMost(bool isTopMost)
    {
        if (isTopMost)
        {
            _window.SetTopMost();
        }
        else
        {
            _window.SetNoTopMost();
        }

        this.RaisePropertyChanged(nameof(StylesEx));
    }

    public void UpdateWindowRect(int left, int top, int width, int height)
    {
        _window.Rect = new RECT { left = left, top = top, right = left + width, bottom = top + height };
        this.RaisePropertyChanged(nameof(Left));
        this.RaisePropertyChanged(nameof(Top));
        this.RaisePropertyChanged(nameof(Width));
        this.RaisePropertyChanged(nameof(Height));
    }

    public void RefreshItem()
    {
        this.RaisePropertyChanged("");
    }

    public void RefreshScreenShot()
    {
        // try
        // {
        //     var screenShot = GetWindowScreenshot(WindowHandle);
        //     if (screenShot != NULL)
        //     {
        //         var imageSource = Imaging.CreateBitmapSourceFromHBitmap(screenShot, nint.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        //         SetField(ref _screenshot, imageSource, propertyName: nameof(Screenshot));
        //         DeleteObject(screenShot);
        //     }
        // }
        // catch
        // {
        //
        // }
    }

    public override string ToString() => $"0x{WindowHandle:X8}{(!string.IsNullOrEmpty(Text) ? $"({Text})" : "")}";

    private void OnError(SystemErrorCodes? win32ErrorCode, HRESULT? hResult)
    {
        if (win32ErrorCode == SystemErrorCodes.ERROR_INVALID_WINDOW_HANDLE)
        {
            // The window is closed
            // Ignore
            return;
        }
        if (win32ErrorCode != null)
        {
            ErrorString = ErrorMessageExtensions.GetSystemErrorMessageFromCode(win32ErrorCode.Value);
        }
        if (hResult.HasValue)
        {
            if (hResult.Value == TYPE_E_ELEMENTNOTFOUND)
            {
                //Getting VirtualDesktopID may get this eroor
                //Ignore
                return;
            }
            ErrorString = Marshal.GetExceptionForHR(hResult.Value).Message;
        }
    }

    private T GetWithDefaultValueWhenException<T>(Func<T> getAction, T defaultValue)
    {
        try
        {
            return getAction();
        }
        catch
        {
            return default;
        }
    }
}
