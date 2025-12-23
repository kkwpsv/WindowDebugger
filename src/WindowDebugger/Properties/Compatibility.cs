// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming

#if !NET6_0_OR_GREATER

namespace System.Runtime.Versioning
{
    [AttributeUsage(
        AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum
        | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event
        | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public sealed class SupportedOSPlatformAttribute(string platformName) : Attribute
    {
    }
}

namespace System
{
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public static class NetFrameworkExtensions
    {
        extension(OperatingSystem)
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsWindows() => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsLinux() => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsMacOS() => false;
        }

        extension(Environment)
        {
            public static int ProcessId
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Process.GetCurrentProcess().Id;
            }

            public static string? ProcessPath
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Process.GetCurrentProcess().MainModule?.FileName;
            }
        }

        extension(Enum)
        {
            public static string[] GetNames<T>() where T : Enum
            {
                return Enum.GetNames(typeof(T));
            }

            public static T Parse<T>(string value, bool ignoreCase = false) where T : Enum
            {
                return (T)Enum.Parse(typeof(T), value, ignoreCase);
            }
        }
    }
}

#endif
