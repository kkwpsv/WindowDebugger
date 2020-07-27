using Lsj.Util.Text;
using Lsj.Util.Win32.Extensions;
using Lsj.Util.WPF;
using System;
using System.Collections.ObjectModel;

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
#if NET48
                    if ((item.Text?.IndexOf(_searchText, StringComparison.CurrentCultureIgnoreCase) > -1)
                        || (item.ProcessName?.IndexOf(_searchText, StringComparison.CurrentCultureIgnoreCase) > -1))
#else
                    if ((item.Text?.Contains(_searchText, StringComparison.CurrentCultureIgnoreCase) ?? false)
                        || (item.ProcessName?.Contains(_searchText, StringComparison.CurrentCultureIgnoreCase) ?? false))
#endif
                    {
                        Windows.Add(item);
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
