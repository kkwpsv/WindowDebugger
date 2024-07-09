using System.Collections.Generic;
using System.Linq;
using SeWzc.WinInfo.Models;

namespace SeWzc.WinInfo.ViewModels;

public class WindowProxyViewModel : ViewModelBase
{
    private readonly WindowProxy _window;

    public WindowProxyViewModel(WindowProxy window)
    {
        _window = window;
        Name = window.Name;
        Handle = window.Handle;
        Children = window.GetChildren().Select(x => new WindowProxyViewModel(x)).ToList();
    }

    /// <summary>
    /// 窗口名称。
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 窗口 Handle/ID。
    /// </summary>
    public int Handle { get; }

    /// <summary>
    /// 子窗口。
    /// </summary>
    public IReadOnlyList<WindowProxyViewModel> Children { get; }

    public IReadOnlyList<WindowValueViewModel> GetWindowValues()
    {
        return _window.GetWindowValues().Select(x => new WindowValueViewModel(x)).ToList();
    }
}