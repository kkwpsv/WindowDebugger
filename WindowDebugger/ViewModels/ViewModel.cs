using Lsj.Util.WPF;

namespace WindowDebugger.ViewModels
{
    public class ViewModel : ModelObject
    {
        public static ViewModel Instance { get; } = new ViewModel();

        public AsyncObservableCollection<WindowItem> Windows { get; set; } = new AsyncObservableCollection<WindowItem>();

        private WindowItem _currentWindow;
        public WindowItem SelectedWindow { get => _currentWindow; set => SetField(ref _currentWindow, value); }
    }
}
