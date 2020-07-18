using System;
using System.Runtime.InteropServices;

namespace Be.Windows.Forms
{
    internal static class NativeMethods
    {
        // Key definitions
        public const int WM_KEYDOWN = 0x100;

        public const int WM_KEYUP = 0x101;

        public const int WM_CHAR = 0x102;

        // Caret definitions
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ShowCaret(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyCaret();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetCaretPos(int X, int Y);
    }
}