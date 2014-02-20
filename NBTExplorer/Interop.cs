using System;
using System.Runtime.InteropServices;

namespace NBTExplorer
{
    internal static class Interop
    {
        public static bool WinInteropAvailable
        {
            get { return IsWindows && Type.GetType("Mono.Runtime") == null; }
        }

        public static bool IsWindows
        {
            get { return Environment.OSVersion.Platform == PlatformID.Win32NT; }
        }

        public static bool IsWinXP
        {
            get
            {
                OperatingSystem OS = Environment.OSVersion;
                return (OS.Platform == PlatformID.Win32NT) &&
                    ((OS.Version.Major > 5) || ((OS.Version.Major == 5) && (OS.Version.Minor == 1)));
            }
        }

        public static bool IsWinVista
        {
            get
            {
                OperatingSystem OS = Environment.OSVersion;
                return (OS.Platform == PlatformID.Win32NT) && (OS.Version.Major >= 6);
            }
        }

        public static IntPtr SendMessage (IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            if (WinInteropAvailable)
                return NativeInterop.SendMessage(hWnd, msg, wParam, lParam);
            else
                return IntPtr.Zero;
        }
    }

    internal static class NativeInterop
    {
        public const int WM_PRINTCLIENT = 0x0318;
        public const int PRF_CLIENT = 0x00000004;

        public const int TV_FIRST = 0x1100;
        public const int TVM_SETBKCOLOR = TV_FIRST + 29;
        public const int TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;

        public const int TVS_EX_DOUBLEBUFFER = 0x0004;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage (IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }
}
