using Lsj.Util.Text;
using Lsj.Util.Win32.Enums;
using Lsj.Util.Win32.Extensions;
using Lsj.Util.WPF;
using System;
using System.Runtime.InteropServices;
using System.Text;
using static Lsj.Util.Win32.Kernel32;
using static Lsj.Util.Win32.User32;

namespace WindowDebugger.ViewModels
{
    public class WindowItem : ModelObject
    {
        private IntPtr _windowHandle;
        public IntPtr WindowHandle { get => _windowHandle; set => SetField(ref _windowHandle, value, extraAction: RefreshItem); }

        private string _text;
        public string Text { get => _text; }

        private WindowStyles _styles;
        public WindowStyles Styles { get => _styles; set => SetStyles(value, out var _); }

        private WindowStylesEx _stylesEx;
        public WindowStylesEx StylesEx { get => _stylesEx; set => SetStylesEx(value, out var _); }

        public void RefreshItem()
        {
            RefreshText();
            RefreshStyles();
            RefreshStylesEx();
        }

        public bool SetText(string value, out int errorCode)
        {
            var result = SetWindowText(_windowHandle, value);
            errorCode = Marshal.GetLastWin32Error();
            RefreshText();
            return result;
        }

        public void RefreshText()
        {
            var sb = new StringBuilder(50);
            GetWindowText(_windowHandle, sb, 50);
            var text = sb.ToString();
            SetField(ref _text, text, propertyName: nameof(Text));
        }

        public bool SetStyles(WindowStyles value, out int errorCode)
        {
            SetLastError(0);
            SetWindowLong(_windowHandle, GetWindowLongIndexes.GWL_STYLE, (IntPtr)value);
            errorCode = Marshal.GetLastWin32Error();
            var result = errorCode == 0;
            RefreshStyles();
            return result;
        }

        public void RefreshStyles()
        {
            var style = (WindowStyles)GetWindowLong(WindowHandle, GetWindowLongIndexes.GWL_STYLE).SafeToUInt32();
            SetField(ref _styles, style, propertyName: nameof(Styles));
        }

        public bool SetStylesEx(WindowStylesEx value, out int errorCode)
        {
            SetLastError(0);
            SetWindowLong(_windowHandle, GetWindowLongIndexes.GWL_EXSTYLE, (IntPtr)value);
            errorCode = Marshal.GetLastWin32Error();
            var result = errorCode == 0;
            RefreshStylesEx();
            return result;
        }

        public void RefreshStylesEx()
        {
            var style = (WindowStylesEx)GetWindowLong(WindowHandle, GetWindowLongIndexes.GWL_EXSTYLE).SafeToUInt32();
            SetField(ref _stylesEx, style, propertyName: nameof(StylesEx));
        }

        public override string ToString() => $"0x{WindowHandle.ToString("X8")}{(!Text.IsNullOrEmpty() ? $"({Text})" : "")}";
    }
}
