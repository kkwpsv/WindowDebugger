using Lsj.Util.Text;
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
            var windows = WindowExtensions.GetAllWindowHandle();
            Windows.Clear();
            foreach (var handle in windows)
            {
                var item = new WindowItem { WindowHandle = handle };
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
                        if (item.WindowHandle.SafeToUInt32() == uintHexVal || item.ProcessID == uintHexVal || item.ThreadID == uintHexVal)
                        {
                            Windows.Add(item);
                        }
                    }

                    if (uint.TryParse(_searchText, out var uintVal))
                    {
                        if (item.WindowHandle.SafeToUInt32() == uintVal || item.ProcessID == uintVal || item.ThreadID == uintVal)
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
    }
}
