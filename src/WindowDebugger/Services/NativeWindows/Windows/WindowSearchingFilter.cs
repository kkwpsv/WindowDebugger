namespace WindowDebugger.Services.NativeWindows.Windows;

public record WindowSearchingFilter
{
    public string? SearchText { get; init; }
    public bool IncludingInvisibleWindow { get; init; } = true;
    public bool IncludingEmptyTitleWindow { get; init; } = true;
    public bool IncludingChildWindow { get; init; } = true;
    public bool IncludingMessageOnlyWindow { get; init; } = true;
}
