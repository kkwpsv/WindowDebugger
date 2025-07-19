// using System.Collections.Generic;
// using System.Linq;
// using System.Reactive;
// using System.Windows.Input;
// using ReactiveUI;
//
// namespace SeWzc.WinInfo.ViewModels;
//
// public class MainWindowViewModel : ViewModelBase
// {
//     private WindowProxyViewModel? _selectedWindow;
//     private IReadOnlyList<WindowInfoTabViewModel> _tabs;
//
//     private IReadOnlyList<WindowProxyViewModel> _windows =
//         Container.Current.MainModel.GetWindows().Select(x => new WindowProxyViewModel(x)).ToList();
//
//     public MainWindowViewModel()
//     {
//         _tabs = [new WindowInfoTabViewModel()];
//         RefreshAllCommand = ReactiveCommand.Create(() =>
//         {
//             Windows = Container.Current.MainModel.GetWindows().Select(x => new WindowProxyViewModel(x)).ToList();
//             if (SelectedWindow is not null)
//             {
//                 var windowProxyViewModel = Find(SelectedWindow.Handle, Windows);
//                 SelectedWindow = windowProxyViewModel;
//             }
//         });
//     }
//
//     public string Title => Container.Current.MainModel.Title;
//
//     public IReadOnlyList<WindowProxyViewModel> Windows
//     {
//         get => _windows;
//         set => this.RaiseAndSetIfChanged(ref _windows, value);
//     }
//
//     public WindowProxyViewModel? SelectedWindow
//     {
//         get => _selectedWindow;
//         set
//         {
//             this.RaiseAndSetIfChanged(ref _selectedWindow, value);
//             var windowValueViewModels = value?.GetWindowValues() ?? [];
//             Tabs =
//             [
//                 new WindowInfoTabViewModel
//                 {
//                     Values = windowValueViewModels
//                 }
//             ];
//         }
//     }
//
//     public IReadOnlyList<WindowInfoTabViewModel> Tabs
//     {
//         get => _tabs;
//         set => this.RaiseAndSetIfChanged(ref _tabs, value);
//     }
//     
//     public ReactiveCommand<Unit, Unit> RefreshAllCommand { get; }
//
//     public static WindowProxyViewModel? Find(int handle, IReadOnlyList<WindowProxyViewModel> windows)
//     {
//         foreach (var windowProxyViewModel in windows)
//         {
//             if (windowProxyViewModel.Handle == handle)
//                 return windowProxyViewModel;
//             if (Find(handle, windowProxyViewModel.Children) is { } result)
//                 return result;
//         }
//
//         return null;
//     }
// }