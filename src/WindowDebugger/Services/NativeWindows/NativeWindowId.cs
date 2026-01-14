using Lsj.Util.Win32.BaseTypes;

namespace WindowDebugger.Services.NativeWindows;

/// <summary>
/// 表示 Windows 上的窗口句柄（<see cref="HWND"/>）或 Linux X11 上的窗口 XID。
/// </summary>
public readonly struct NativeWindowId : IEquatable<NativeWindowId>
{
    /// <summary>
    /// 获取 Windows 上的窗口句柄（<see cref="HWND"/>）或 Linux X11 上的窗口 XID。
    /// </summary>
    public nint NativeId { get; }

    public NativeWindowId()
    {
    }

    public NativeWindowId(nint nativeId)
    {
        NativeId = nativeId;
    }

    public bool Equals(NativeWindowId other) => NativeId == other.NativeId;

    public override bool Equals(object? obj) => obj is NativeWindowId other && Equals(other);

    public override int GetHashCode() => NativeId.GetHashCode();

    public static implicit operator nint(NativeWindowId id) => id.NativeId;

    public static implicit operator NativeWindowId(nint nativeId) => new(nativeId);

    public static implicit operator NativeWindowId(HWND hwnd) => new(hwnd);

#if NET6_0_OR_GREATER
    public static implicit operator NativeWindowId(SeWzc.X11Sharp.X11Window xid) => new(xid);
#endif

    public static bool operator ==(NativeWindowId a, NativeWindowId b) => a.NativeId == b.NativeId;

    public static bool operator !=(NativeWindowId a, NativeWindowId b) => a.NativeId != b.NativeId;

    public static bool operator ==(NativeWindowId a, nint b) => a.NativeId == b;

    public static bool operator !=(NativeWindowId a, nint b) => a.NativeId != b;

    public static bool operator ==(NativeWindowId a, HWND b) => a.NativeId == b;

    public static bool operator !=(NativeWindowId a, HWND b) => a.NativeId != b;

#if NET6_0_OR_GREATER
    public static bool operator ==(NativeWindowId a, SeWzc.X11Sharp.X11Window b) => a.NativeId == b;

    public static bool operator !=(NativeWindowId a, SeWzc.X11Sharp.X11Window b) => a.NativeId != b;
#endif

    public override string ToString()
    {
        return NativeId.ToString("X8");
    }
}
