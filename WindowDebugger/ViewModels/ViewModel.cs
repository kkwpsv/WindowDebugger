using Lsj.Util.Win32.Enums;
using Lsj.Util.WPF;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using static Lsj.Util.Win32.User32;

namespace WindowDebugger.ViewModels
{
    public class ViewModel : ModelObject
    {
        public static ViewModel Instance { get; } = new ViewModel();

        public AsyncObservableCollection<WindowItem> Windows { get; set; } = new AsyncObservableCollection<WindowItem>();

        private WindowItem _currentWindow;
        public WindowItem SelectedWindow { get => _currentWindow; set => SetField(ref _currentWindow, value); }
    }

    public class WindowItem : ModelObject
    {
        private IntPtr _windowHandle;
        public IntPtr WindowHandle { get => _windowHandle; set => SetField(ref _windowHandle, value, extraAction: RefreshItem); }

        private string _text;
        public string Text { get => _text; }

        private WindowStyles _styles;
        public WindowStyles Styles { get => _styles; set => SetStyles(value, out var _); }

        public void RefreshItem()
        {
            RefreshText();
            RefreshStyles();
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
            SetWindowLong(_windowHandle, GetWindowLongIndexes.GWL_STYLE, (IntPtr)value);
            //TODO SetLastWin32Error To 0;
            errorCode = Marshal.GetLastWin32Error();
            var result = errorCode == 0;
            RefreshStyles();
            return result;
        }

        public void RefreshStyles()
        {
            var style = (WindowStyles)(int)(long)GetWindowLong(WindowHandle, GetWindowLongIndexes.GWL_STYLE);
            SetField(ref _styles, style, propertyName: nameof(Styles));
        }

        public override string ToString() => $"{WindowHandle.ToString("X8")}({Text})";
    }
}
