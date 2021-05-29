using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ShinyHunter
{
    /// <summary>
    /// Take screenshots of windows using Win32 interop.
    /// </summary>
    /// <remarks>
    /// Adapted from <see cref="https://stackoverflow.com/a/24879511/7512368"/> and
    /// <see cref="https://www.cyotek.com/blog/getting-a-window-rectangle-without-the-drop-shadow"/>
    /// to eliminate the annoying drop shadow borders around the window size reported from GetWindowRect.
    /// </remarks>
    public class Screenshot
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(IntPtr handle, int dwAttribute, out Rect pvAttribute, int cbAttribute);

        private const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;
        private const int S_OK = 0;

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr handle, ref Rect rect);

        private static Rectangle GetWindowRectangle(IntPtr handle)
        {
            if (DwmGetWindowAttribute(handle, DWMWA_EXTENDED_FRAME_BOUNDS, out Rect rect, Marshal.SizeOf(typeof(Rect))) != S_OK)
            {
                //fallback to GetWindowRect if getting the frame bounds attribute doesn't work for some reason
                GetWindowRect(handle, ref rect);
            }
            return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        public static Bitmap CaptureWindow(Process process)
        {
            var bounds = GetWindowRectangle(process.MainWindowHandle);
            var result = new Bitmap(bounds.Width, bounds.Height);
            using (var graphics = Graphics.FromImage(result))
            {
                graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }
            return result;
        }
    }
}
