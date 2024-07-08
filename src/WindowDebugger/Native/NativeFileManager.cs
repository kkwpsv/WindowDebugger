namespace WindowDebugger.Native;

public static partial class NativeFileManager
{
    public static void RevealFile(string path)
    {
        if (OperatingSystem.IsLinux())
        {

        }
        else if (OperatingSystem.IsWindows())
        {
            WindowsRevealFile(path);
        }
        else if (OperatingSystem.IsMacOS())
        {

        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }
}
