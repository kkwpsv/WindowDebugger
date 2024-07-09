using ReactiveUI;
using SeWzc.WinInfo.Models;

namespace SeWzc.WinInfo.ViewModels;

public class WindowValueViewModel(WindowProperty model) : ViewModelBase
{
    protected WindowProperty Model { get; } = model;

    public string Name => Model.Name;

    public IReadOnlyWindowPropertyValue Value
    {
        get => Model.GetValue();
        set
        {
            if (value is not IWindowPropertyValue windowPropertyValue)
            {
                this.RaisePropertyChanged();
                return;
            }
            Model.SetValue(windowPropertyValue);
            this.RaisePropertyChanged();
        }
    }
}