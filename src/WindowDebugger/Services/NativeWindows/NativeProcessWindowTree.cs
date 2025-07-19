using System.Collections.Immutable;
using WindowDebugger.Services.NativeWindows.Linux;
using WindowDebugger.Services.NativeWindows.Windows;

namespace WindowDebugger.Services.NativeWindows;

public abstract record NativeTreeNode
{
    public abstract IReadOnlyList<NativeTreeNode> Children { get; }
}

public record NativeProcessNode(int ProcessId) : NativeTreeNode
{
    public string? ProcessName { get; init; }

    public override IReadOnlyList<NativeTreeNode> Children => Windows;

    public required ImmutableArray<NativeWindowNode> Windows { get; init; }
}

public abstract record NativeWindowNode(NativeWindowModel Window) : NativeTreeNode
{
    public override IReadOnlyList<NativeTreeNode> Children => ChildWindows;

    public required ImmutableArray<NativeWindowNode> ChildWindows { get; init; }
}

public record WindowsNativeWindowNode(WindowsNativeWindowModel Window) : NativeWindowNode(Window)
{
    public new WindowsNativeWindowModel Window => (WindowsNativeWindowModel)base.Window;
}

public record LinuxNativeWindowNode(LinuxNativeWindowModel Window) : NativeWindowNode(Window)
{
    public new LinuxNativeWindowModel Window => (LinuxNativeWindowModel)base.Window;
}

public static class NativeTreeNodeExtensions
{
    public static IEnumerable<NativeWindowNode> EnumerableAllWindows(this IEnumerable<NativeTreeNode> tree)
    {
        foreach (var node in tree)
        {
            if (node is NativeProcessNode processNode)
            {
                foreach (var windowNode in processNode.Windows.EnumerableAllWindows())
                {
                    yield return windowNode;
                }
            }
            else if (node is NativeWindowNode windowNode)
            {
                yield return windowNode;
                foreach (var childWindowNode in windowNode.ChildWindows.EnumerableAllWindows())
                {
                    yield return childWindowNode;
                }
            }
        }
    }
}
