using ReactiveUI;

namespace WindowDebugger.Views;

public record NativeTreeNode : ReactiveRecord;

public record NativeProcessTreeNode(int ProcessId) : NativeTreeNode;

public record NativeWindowTreeNode(nint Id) : NativeTreeNode;
