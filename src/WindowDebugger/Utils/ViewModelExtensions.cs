using System.Runtime.CompilerServices;
using ReactiveUI;

namespace WindowDebugger.Utils;

public static class ViewModelExtensions
{
    public static void RaiseAndSetWithAction<T>(this ReactiveRecord obj, ref T backingField, T value, Action action, [CallerMemberName] string? propertyName = null)
    {
        var oldValue = backingField;
        var newValue = obj.RaiseAndSetIfChanged(ref backingField, value, propertyName);
        if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
        {
            return;
        }

        action();
    }
}
