using Lsj.Util.Text;
using Lsj.Util.Win32.Extensions;
using Lsj.Util.WPF;
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
            var windows = WindowExtensions.GetAllTopLevelWindowHandle();
            Windows.Clear();
            foreach (var handle in windows)
            {
                var item = new WindowItem { WindowHandle = handle };
                if (!_searchText.IsNullOrEmpty())
                {
                    item.RefreshText();
                    if (item.Text.Contains(_searchText))
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
        public WindowItem SelectedWindow { get => _currentWindow; set => SetField(ref _currentWindow, value); }
    }
}
