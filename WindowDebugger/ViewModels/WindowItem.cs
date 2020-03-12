using Lsj.Util.Text;
using Lsj.Util.Win32.Enums;
using Lsj.Util.Win32.Extensions;
using Lsj.Util.Win32.Structs;
using Lsj.Util.WPF;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.RightsManagement;
using System.Text;
using System.Windows;
using static Lsj.Util.Win32.Kernel32;
using static Lsj.Util.Win32.User32;
using static Lsj.Util.Win32.Enums.SetWindowPosFlags;

namespace WindowDebugger.ViewModels
{
    public class WindowItem : ModelObject
    {
        private IntPtr _windowHandle;
        public IntPtr WindowHandle { get => _windowHandle; set => SetField(ref _windowHandle, value, extraAction: RefreshItem); }

        private string _lastError;
        public string LastError { get => _lastError; private set => SetField(ref _lastError, value); }

        private string _text;
        public string Text { get => _text; set => SetText(value); }

        private WindowStyles _styles;
        public WindowStyles Styles { get => _styles; set => SetStyles(value); }

        private WindowStylesEx _stylesEx;
        public WindowStylesEx StylesEx { get => _stylesEx; set => SetStylesEx(value); }

        private int _processID;
        public int ProcessID { get => _processID; }

        private int _left;
        public int Left { get => _left; set => SetField(ref _left, value); }

        private int _top;
        public int Top { get => _top; set => SetField(ref _top, value); }

        private int _width;
        public int Width { get => _width; set => SetField(ref _width, value); }

        private int _height;
        public int Height { get => _height; set => SetField(ref _height, value); }


        public void RefreshItem()
        {
            RefreshText();
            RefreshStyles();
            RefreshStylesEx();
            RefreshProcessID();
            RefreshWindowRect();
            RefreshWindowPlacement();
        }

        private void SetError() => LastError = ErrorMessageExtensions.GetSystemErrorMessageFromCode((uint)Marshal.GetLastWin32Error());

        private void SetText(string value)
        {
            LastError = null;
            var result = SetWindowText(_windowHandle, value);
            if (!result)
            {
                SetError();
            }
            RefreshText();
        }

        public void RefreshText()
        {
            var sb = new StringBuilder(50);
            GetWindowText(_windowHandle, sb, 50);
            var text = sb.ToString();
            SetField(ref _text, text, propertyName: nameof(Text));
        }

        private void SetStyles(WindowStyles value)
        {
            SetLastError(0);
            SetWindowLong(_windowHandle, GetWindowLongIndexes.GWL_STYLE, (IntPtr)value);
            var errorCode = Marshal.GetLastWin32Error();
            if (errorCode != 0)
            {
                SetError();
            }
            RefreshStyles();
        }

        public void RefreshStyles()
        {
            var style = (WindowStyles)GetWindowLong(WindowHandle, GetWindowLongIndexes.GWL_STYLE).SafeToUInt32();
            SetField(ref _styles, style, propertyName: nameof(Styles));
        }

        private void SetStylesEx(WindowStylesEx value)
        {
            SetLastError(0);
            SetWindowLong(_windowHandle, GetWindowLongIndexes.GWL_EXSTYLE, (IntPtr)value);
            var errorCode = Marshal.GetLastWin32Error();
            if (errorCode != 0)
            {
                SetError();
            }
            RefreshStylesEx();
        }

        public void RefreshStylesEx()
        {
            var style = (WindowStylesEx)GetWindowLong(WindowHandle, GetWindowLongIndexes.GWL_EXSTYLE).SafeToUInt32();
            SetField(ref _stylesEx, style, propertyName: nameof(StylesEx));
        }

        public void RefreshProcessID()
        {
            GetWindowThreadProcessId(WindowHandle, out var processid);
            SetField(ref _processID, (int)processid, propertyName: nameof(ProcessID));
        }

        public bool UpdateWindowRect()
        {
            var result = SetWindowPos(WindowHandle, IntPtr.Zero, _left, _top, _width, _height, SetWindowPosFlags.SWP_NOZORDER);
            if (!result)
            {
                SetError();
            }
            RefreshWindowRect();
            RefreshWindowPlacement();
            return result;
        }

        public void RefreshWindowRect()
        {
            if (GetWindowRect(WindowHandle, out var rect))
            {
                Left = rect.left;
                Top = rect.top;
                Height = rect.bottom - rect.top;
                Width = rect.right - rect.left;
            }
        }

        public void RefreshWindowPlacement()
        {
            var placement = new WINDOWPLACEMENT();
            placement.length = (uint)Marshal.SizeOf<WINDOWPLACEMENT>();
            if (GetWindowPlacement(WindowHandle, ref placement))
            {

            }
        }

        public void SetTopMost(bool isTopMost)
        {
            SetWindowPos(_windowHandle, isTopMost ? HWND_TOPMOST : HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            RefreshStylesEx();
        }

        public override string ToString() => $"0x{WindowHandle.ToString("X8")}{(!Text.IsNullOrEmpty() ? $"({Text})" : "")}";
    }
}
