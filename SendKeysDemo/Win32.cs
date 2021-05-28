using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace SendKeysDemo
{
    public static class Win32
    {
        [DllImport("User32.dll")]
        private static extern int SetForegroundWindow(IntPtr handle);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);

        public static void SetForegroundWindow(Process process)
        {
            SetForegroundWindow(process.MainWindowHandle);
        }

        public static void SetForegroundWindow(Process process, int millisecondsToSleepAfter)
        {
            SetForegroundWindow(process.MainWindowHandle);
            Thread.Sleep(millisecondsToSleepAfter);
        }

        public static void ShowWindow(Process process)
        {
            const int SW_NORMAL = 1;
            ShowWindow(process.MainWindowHandle, SW_NORMAL);
        }
    }
}
