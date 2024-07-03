using System.Runtime.InteropServices;

namespace WindowDebugger.Framework;

/// <summary>
/// 包含平台相关的选项。
/// </summary>
/// <typeparam name="T">选项的类型。</typeparam>
public record PlatformOption<T>
{
    /// <summary>
    /// Linux 平台的选项。
    /// </summary>
    public required T Linux { get; init; }

    /// <summary>
    /// MacOS 平台的选项。
    /// </summary>
    public required T MacOS { get; init; }

    /// <summary>
    /// Windows 平台的选项。
    /// </summary>
    public required T Windows { get; init; }

    /// <summary>
    /// 获取当前平台下的选项。
    /// </summary>
    /// <returns>当前平台下的选项。</returns>
    /// <exception cref="PlatformNotSupportedException">当遇到未知平台时引发。</exception>
    public T GetOption()
    {
        if (OperatingSystem.IsWindows())
        {
            return Windows;
        }

        if (OperatingSystem.IsLinux())
        {
            return Linux;
        }

        if (OperatingSystem.IsMacOS())
        {
            return MacOS;
        }

        throw new PlatformNotSupportedException($"Unknown platform {RuntimeInformation.OSDescription}");
    }
}
