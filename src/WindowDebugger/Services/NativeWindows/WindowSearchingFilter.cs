using System.Globalization;

namespace WindowDebugger.Services.NativeWindows;

/// <summary>
/// 表示在搜索窗口时使用的过滤器。
/// </summary>
public readonly record struct WindowSearchingFilter
{
    public WindowSearchingFilter()
    {
        SearchText = null;
        SearchingValueAsHex = null;
        SearchingValueAsDecimal = null;
    }

    public WindowSearchingFilter(string? searchText)
    {
        searchText = searchText?.Trim();
        SearchText = searchText;

        if (string.IsNullOrWhiteSpace(searchText))
        {
            // There is no search text.
            SearchingValueAsHex = null;
            SearchingValueAsDecimal = null;
        }
        else
        {
            if (searchText.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                // This must be a hex value.
                SearchingValueAsHex = uint.TryParse(searchText, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var uintHexVal)
                    ? uintHexVal
                    : null;
                SearchingValueAsDecimal = null;
            }
            else
            {
                // This may be a hex value or a decimal value.
                SearchingValueAsHex = uint.TryParse(searchText, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var uintHexVal)
                    ? uintHexVal
                    : null;
                SearchingValueAsDecimal = uint.TryParse(searchText, out var uintVal) ? uintVal : null;
            }
        }
    }

    public string? SearchText { get; }

    public uint? SearchingValueAsHex { get; }

    public uint? SearchingValueAsDecimal { get; }

    public WindowIncluding Including { get; init; } = WindowIncluding.All;

    public WindowGrouping Grouping { get; init; } = WindowGrouping.ProcessThenWindowTree;

    public WindowSearchingProperties SearchingProperties { get; init; } = WindowSearchingProperties.All;

    public WindowSorting Sorting { get; init; } = WindowSorting.Raw;
}

/// <summary>
/// 表示在搜索窗口时包含的窗口类型。
/// </summary>
[Flags]
public enum WindowIncluding
{
    /// <summary>
    /// 包含顶级的、有标题的、可见的窗口。
    /// </summary>
    Normal = 0,

    /// <summary>
    /// 除此之外，还包含不可见窗口。
    /// </summary>
    InvisibleWindow = 1 << 0,

    /// <summary>
    /// 除此之外，还包含无标题窗口。
    /// </summary>
    EmptyTitleWindow = 1 << 1,

    /// <summary>
    /// 除此之外，还包含子窗口。
    /// </summary>
    ChildWindow = 1 << 2,

    /// <summary>
    /// 除此之外，还包含纯消息窗口。
    /// </summary>
    MessageOnlyWindow = 1 << 3,

    /// <summary>
    /// 包含所有类型的窗口。
    /// </summary>
    All = InvisibleWindow | EmptyTitleWindow | ChildWindow | MessageOnlyWindow,
}

/// <summary>
/// 列表中窗口的分组方式。
/// </summary>
public enum WindowGrouping
{
    /// <summary>
    /// 进程 / 顶级窗口 / 子窗口。
    /// </summary>
    ProcessThenWindowTree,

    /// <summary>
    /// 顶级窗口 / 子窗口。
    /// </summary>
    WindowTree,

    /// <summary>
    /// 进程 / 窗口。
    /// </summary>
    ProcessThenWindow,

    /// <summary>
    /// 无（以列表显示窗口）。
    /// </summary>
    PlainList,
}

/// <summary>
/// 表示在搜索窗口时使用的属性。
/// </summary>
[Flags]
public enum WindowSearchingProperties
{
    /// <summary>
    /// 窗口句柄（Windows）或 Id（Linux）。
    /// </summary>
    Id,

    /// <summary>
    /// 窗口标题。
    /// </summary>
    Title,

    /// <summary>
    /// 进程名。
    /// </summary>
    ProcessName,

    /// <summary>
    /// 包含所有属性。
    /// </summary>
    All = Id | Title | ProcessName,
}

/// <summary>
/// 表示窗口列表的排序方式。
/// </summary>
public enum WindowSorting
{
    /// <summary>
    /// 不排序，从系统调用中拿到了什么顺序就显示为什么顺序。
    /// </summary>
    Raw,

    /// <summary>
    /// 按窗口句柄（Windows）或 Id（Linux）升序排序。
    /// </summary>
    AscendingById,

    /// <summary>
    /// 按窗口标题升序排序。
    /// </summary>
    AscendingByTitle,
}
