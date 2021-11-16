using Lsj.Util.Text;
using Lsj.Util.Win32;
using Lsj.Util.Win32.BaseTypes;
using Lsj.Util.Win32.Enums;
using Lsj.Util.Win32.Extensions;
using Lsj.Util.Win32.NativeUI;
using Lsj.Util.Win32.Structs;
using Lsj.Util.WPF;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using static Lsj.Util.Win32.BaseTypes.HRESULT;
using static Lsj.Util.Win32.Constants;
using static Lsj.Util.Win32.Enums.ProcessAccessRights;
using static Lsj.Util.Win32.Enums.RedrawWindowFlags;
using static Lsj.Util.Win32.Enums.ThreadAccessRights;
using static Lsj.Util.Win32.Enums.WindowsMessages;
using static Lsj.Util.Win32.Extensions.WindowExtensions;
using static Lsj.Util.Win32.Gdi32;
using static Lsj.Util.Win32.Kernel32;
using static Lsj.Util.Win32.UnsafePInvokeExtensions;
using static Lsj.Util.Win32.User32;

namespace WindowDebugger.ViewModels
{
    public class WindowItem : ModelObject
    {
        private readonly Win32Window _window;

        public WindowItem(HWND hwnd)
        {
            _window = new Win32Window(hwnd);
            _window.CustomErrorHandler = OnError;
        }

        public IntPtr WindowHandle { get => _window.Handle; }

        private string _errorString;
        public string ErrorString { get => _errorString; set => SetField(ref _errorString, value); }

        public string Text { get => _window.Text; set => _window.Text = value; }

        public WindowStyles Styles { get => _window.WindowStyles; set => _window.WindowStyles = value; }

        public WindowStylesEx StylesEx { get => _window.WindowStylesEx; set => _window.WindowStylesEx = value; }

        public ClassStyles ClassStyles { get => _window.ClassStyles; set => _window.ClassStyles = value; }

        public int ProcessID { get => _window.ProcessID; }

        public string ProcessName { get => _window.ProcessName; }

        public int ThreadID { get => _window.ThreadID; }

        public string ClassName { get => _window.ClassName; }

        public int Left { get => _window.Rect.left; }

        public int Top { get => _window.Rect.top; }

        public int Width { get => _window.Rect.right - _window.Rect.left; }

        public int Height { get => _window.Rect.bottom - _window.Rect.top; }

        public ShowWindowCommands WindowShowStates { get => _window.ShowStates; set => _window.ShowStates = value; }

        public DPI_AWARENESS DpiAwareness { get => _window.DpiAwareness; }

        public IntPtr ParentWindowHandle
        {
            get => _window.ParentWindowHandle;
            set
            {
                _window.ParentWindowHandle = value;
                OnPropertyChanged(nameof(DpiAwareness));//may be reset by system
            }
        }

        public IntPtr OwnerWindowHandle { get => _window.OwnerWindowHandle; set => _window.OwnerWindowHandle = value; }

        private BitmapSource _screenshot;
        public BitmapSource Screenshot { get => _screenshot; set => SetField(ref _screenshot, value); }

        public DWMInfo DWMInfo { get => _window.ParentWindowHandle == GetDesktopWindow() ? _window.DWMInfo : null; }

        public bool IsTouchWindow { get => _window.IsTouchWindow; }

        public string VirtualDesktopID { get => _window.DesktopID.ToString(); }

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
                var threadHandle = CreateRemoteThread(process, NullRef<SECURITY_ATTRIBUTES>(), UIntPtr.Zero, null, NULL, 0, out NullRef<DWORD>());
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

            OnPropertyChanged(nameof(StylesEx));
        }

        public void UpdateWindowRect(int left, int top, int width, int height)
        {
            _window.Rect = new RECT { left = left, top = top, right = Left + width, bottom = Top + height };
            OnPropertyChanged(nameof(Left));
            OnPropertyChanged(nameof(Top));
            OnPropertyChanged(nameof(Width));
            OnPropertyChanged(nameof(Height));
        }

        public void RefreshItem()
        {
            OnPropertyChanged("");
        }

        public void RefreshScreenShot()
        {
            try
            {
                var screenShot = GetWindowScreenshot(WindowHandle);
                if (screenShot != NULL)
                {
                    var imageSource = Imaging.CreateBitmapSourceFromHBitmap(screenShot, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    SetField(ref _screenshot, imageSource, propertyName: nameof(Screenshot));
                    DeleteObject(screenShot);
                }
            }
            catch
            {

            }
        }

        public override string ToString() => $"0x{WindowHandle.ToString("X8")}{(!Text.IsNullOrEmpty() ? $"({Text})" : "")}";

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
    }
}
