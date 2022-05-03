using Lsj.Util.Text;
using Lsj.Util.Win32;
using Lsj.Util.Win32.BaseTypes;
using Lsj.Util.Win32.Extensions;
using Lsj.Util.Win32.NativeUI;
using Lsj.Util.WPF;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace WindowDebugger.ViewModels
{
    public class ViewModel : ModelObject
    {
        public static ViewModel Instance { get; } = new ViewModel();

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set => SetField(ref _searchText, value, extraAction: RefreshWindowList);
        }

        public ObservableCollection<WindowItem> Windows
        {
            get;
        } = new ObservableCollection<WindowItem>();

        private WindowItem _currentWindow;
        public WindowItem SelectedWindow { get => _currentWindow; set => SetField(ref _currentWindow, value, extraAction: () => { value?.RefreshItem(); value?.RefreshScreenShot(); }); }

        private bool _dwmIsCompositionEnabled;

        public bool DwmIsCompositionEnabled { get => _dwmIsCompositionEnabled; }

        private bool _includeNonVisibleWindows = false;

        public bool IncludeNonVisibleWindows { get => _includeNonVisibleWindows; set => SetField(ref _includeNonVisibleWindows, value, extraAction: RefreshWindowList); }

        private bool _includeNonTitledWindows = false;

        public bool IncludeNonTitledWindows { get => _includeNonTitledWindows; set => SetField(ref _includeNonTitledWindows, value, extraAction: RefreshWindowList); }

        private bool _includeChildWindows = false;

        public bool IncludeChildWindows { get => _includeChildWindows; set => SetField(ref _includeChildWindows, value, extraAction: RefreshWindowList); }

        private bool _includeMessageOnlyWindows = false;

        public bool IncludeMessageOnlyWindows { get => _includeMessageOnlyWindows; set => SetField(ref _includeMessageOnlyWindows, value, extraAction: RefreshWindowList); }

        private uint? _searchStringHexUintValue;
        private uint? _searchStringUintValue;

        public void RefreshWindowList()
        {
            Dwmapi.DwmIsCompositionEnabled(out var isEnabled);
            SetField(ref _dwmIsCompositionEnabled, isEnabled, propertyName: nameof(DwmIsCompositionEnabled));

            if (!_searchText.IsNullOrEmpty())
            {
                _searchStringHexUintValue =
                    uint.TryParse(_searchText, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var uintHexVal) ||
                    (_searchText.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase) && uint.TryParse(_searchText, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out uintHexVal))
                    ? uintHexVal
                    : null;

                _searchStringUintValue = uint.TryParse(_searchText, out var uintVal) ? uintVal : null;
            }
            else
            {
                _searchStringHexUintValue = null;
                _searchStringUintValue = null;
            }

            var windows = WindowExtensions.GetAllWindow(x => new WindowItem(x), WindowFilter, DescendantsFilter);
            Windows.Clear();
            foreach (var win in windows)
            {
                Windows.Add(win);
            }
        }

        private (bool includeSelf, bool includeDescendants, bool continueEnum) WindowFilter(HWND handle)
        {
            return (DescendantsFilter(handle).includeSelf, IncludeChildWindows, true);
        }

        private (bool includeSelf, bool continueEnum) DescendantsFilter(HWND handle)
        {
            var win = new Win32Window(handle);

            if (!IncludeMessageOnlyWindows)
            {
                if (win.ParentWindowHandle == User32.HWND_MESSAGE)
                {
                    return (false, true);
                }
            }

            if (!IncludeNonVisibleWindows)
            {
                if (!win.IsVisible)
                {
                    return (false, true);
                }
            }

            var text = win.Text;
            if (!IncludeNonTitledWindows)
            {
                if (text.IsNullOrEmpty())
                {
                    return (false, true);
                }
            }

            if (!_searchText.IsNullOrEmpty())
            {
                if ((text.IndexOf(_searchText, StringComparison.CurrentCultureIgnoreCase) > -1)
                    || (win.ProcessName.IndexOf(_searchText, StringComparison.CurrentCultureIgnoreCase) > -1))
                {
                    return (true, true);
                }

                if (_searchStringHexUintValue is uint uintHexVal)
                {
                    if (((IntPtr)handle).SafeToUInt32() == uintHexVal || win.ProcessID == uintHexVal || win.ThreadID == uintHexVal
                        || ((IntPtr)win.OwnerWindowHandle).SafeToUInt32() == uintHexVal || ((IntPtr)win.ParentWindowHandle).SafeToUInt32() == uintHexVal)
                    {
                        return (true, true);
                    }
                }

                if (_searchStringUintValue is uint uintVal)
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
}
