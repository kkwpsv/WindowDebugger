using ReactiveUI;

namespace WindowDebugger.Services.NativeWindows.Linux.ViewModels;

public class WindowInfoTabViewModel : ViewModelBase
{
    private IReadOnlyList<WindowValueViewModel> _values = [];
    public string Name { get; } = "Tag Name";

    public IReadOnlyList<WindowValueViewModel> Values
    {
        get => _values;
        set => this.RaiseAndSetIfChanged(ref _values, value);
    }
}