using Avalonia;
using Avalonia.Media;
using WindowDebugger.Framework;

namespace WindowDebugger.Startup;

public static class FontStartup
{
    public static AppBuilder UseLocalizedSystemDefaultFont(this AppBuilder builder, FallbackFontNames? fallbackFontNames = null)
    {
        var fallback = fallbackFontNames?.GetOption() ?? FallbackFontNames.Default.GetOption();
        var faceName = GetLocalizedSystemDefaultFontName() ?? fallback[0];
        var fontFallbacks = new FontFallback[fallback.Length];

        for (var i = 0; i < fallback.Length; i++)
        {
            fontFallbacks[i] = new FontFallback { FontFamily = fallback[i] };
        }

        builder.With(new FontManagerOptions
        {
            DefaultFamilyName = faceName,
            FontFallbacks = fontFallbacks,
        });

        return builder;
    }

    public static string? GetLocalizedSystemDefaultFontName()
    {
        if (OperatingSystem.IsWindows())
        {
            return global::WindowDebugger.Native.Windows.SystemParameters.GetSystemDefaultFontName();
        }

        if (OperatingSystem.IsLinux())
        {
            return null;
        }

        return null;
    }
}

/// <summary>
/// 各个平台下的回滚字体名称。
/// </summary>
public record FallbackFontNames : PlatformOption<string[]>
{
    private static readonly Lazy<FallbackFontNames> DefaultLazy = new(() => new FallbackFontNames
    {
        Windows = ["Segoe UI", "Arial"],
        Linux = ["Noto Sans CJK SC"],
        MacOS = ["PingFang SC"],
    });

    /// <summary>
    /// 获取各平台下默认的回滚字体名称。
    /// </summary>
    public static FallbackFontNames Default => DefaultLazy.Value;
}
