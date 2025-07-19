using Lsj.Util.Win32;
using Lsj.Util.Win32.Structs;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using static Lsj.Util.Win32.Enums.SystemParametersInfoParameters;
using static Lsj.Util.Win32.UnsafePInvokeExtensions;

namespace WindowDebugger.Native.Windows;

[SupportedOSPlatform("windows")]
internal static class SystemParameters
{
    // TODO: move to Lsj.Util.Win32
    [StructLayout(LayoutKind.Sequential)]
    internal struct NONCLIENTMETRICS
    {
        public int cbSize;
        public int iBorderWidth;
        public int iScrollWidth;
        public int iScrollHeight;
        public int iCaptionWidth;
        public int iCaptionHeight;
        public LOGFONT lfCaptionFont;
        public int iSmCaptionWidth;
        public int iSmCaptionHeight;
        public LOGFONT lfSmCaptionFont;
        public int iMenuWidth;
        public int iMenuHeight;
        public LOGFONT lfMenuFont;
        public LOGFONT lfStatusFont;
        public LOGFONT lfMessageFont;
        [SupportedOSPlatform("windows6.0.6000" /* Vista */)]
        public int iPaddedBorderWidth;
    }

    public unsafe static string? GetSystemDefaultFontName()
    {
        var ncm = stackalloc NONCLIENTMETRICS[1];
        ncm->cbSize = SizeOf<NONCLIENTMETRICS>();

        return User32.SystemParametersInfo(SPI_GETNONCLIENTMETRICS, ncm->cbSize, ncm, 0) ? (string)ncm->lfMessageFont.lfFaceName : null;
    }
}
