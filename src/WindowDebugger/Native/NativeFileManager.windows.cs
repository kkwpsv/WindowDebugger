using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace WindowDebugger.Native;

partial class NativeFileManager
{
    [SupportedOSPlatform("windows")]
    private static void WindowsRevealFile(string path)
    {
        var pidl = ILCreateFromPathW(path);

        try
        {
            SHOpenFolderAndSelectItems(pidl, 0, 0, 0);
        }
        finally
        {
            ILFree(pidl);
        }
    }

    [DllImport("shell32", CharSet = CharSet.Unicode, SetLastError = false)]
    private static extern nint ILCreateFromPathW(string pszPath);

    [DllImport("shell32", SetLastError = false)]
    private static extern void ILFree(nint pidl);

    [DllImport("shell32", CharSet = CharSet.Unicode, SetLastError = false)]
    private static extern int SHOpenFolderAndSelectItems(nint pidlFolder, int cild, nint apidl, int dwFlags);
}
