using Lsj.Util.Text;
using Lsj.Util.Win32;
using Lsj.Util.Win32.Extensions;
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

        public void RefreshWindowList()
        {
            Dwmapi.DwmIsCompositionEnabled(out var isEnabled);
            SetField(ref _dwmIsCompositionEnabled, isEnabled, propertyName: nameof(DwmIsCompositionEnabled));

            var windows = WindowExtensions.GetAllWindowHandle();
            Windows.Clear();
            foreach (var handle in windows)
            {
                var item = new WindowItem(handle);
                if (!_searchText.IsNullOrEmpty())
                {
                    if ((item.Text?.IndexOf(_searchText, StringComparison.CurrentCultureIgnoreCase) > -1)
                        || (item.ProcessName?.IndexOf(_searchText, StringComparison.CurrentCultureIgnoreCase) > -1))
                    {
                        Windows.Add(item);
                    }

                    if (uint.TryParse(_searchText, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var uintHexVal) ||
                        (_searchText.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase) && uint.TryParse(_searchText, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out uintHexVal)))
                    {
                        if (item.WindowHandle.SafeToUInt32() == uintHexVal || item.ProcessID == uintHexVal || item.ThreadID == uintHexVal
                             || item.OwnerWindowHandle.SafeToUInt32() == uintHexVal || item.ParentWindowHandle.SafeToUInt32() == uintHexVal)
                        {
                            Windows.Add(item);
                        }
                    }

                    if (uint.TryParse(_searchText, out var uintVal))
                    {
                        if (item.WindowHandle.SafeToUInt32() == uintVal || item.ProcessID == uintVal || item.ThreadID == uintVal
                            || item.OwnerWindowHandle.SafeToUInt32() == uintVal || item.ParentWindowHandle.SafeToUInt32() == uintVal)
                        {
                            Windows.Add(item);
                        }
                    }
                }
                else
                {
                    Windows.Add(item);
                }
            }
        }

        private WindowItem _currentWindow;
        public WindowItem SelectedWindow { get => _currentWindow; set => SetField(ref _currentWindow, value, extraAction: () => { value?.RefreshItem(); value?.RefreshScreenShot(); }); }

        private bool _dwmIsCompositionEnabled;

        public bool DwmIsCompositionEnabled { get => _dwmIsCompositionEnabled; }
    }
}
