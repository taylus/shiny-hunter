using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SendKeysDemo
{
    public class Program
    {
        [DllImport("User32.dll")]
        private static extern int SetForegroundWindow(IntPtr handle);

        public static void Main()
        {
            //test sending keyboard input to a process
            var process = Process.GetProcessesByName("notepad").FirstOrDefault();
            if (process == null)
            {
                Console.Error.WriteLine("notepad is not running");
                Console.Error.Flush();
                Environment.Exit(-1);
            }

            SetForegroundWindow(process.MainWindowHandle);
            SendKeys.SendWait("test");

            //test capturing a screenshot of the process
            var screenshot = ScreenCapture.CaptureWindow(process.MainWindowHandle);
            var screenshotFileName = $"{Guid.NewGuid()}.png";
            screenshot.Save(screenshotFileName, ImageFormat.Png);
            Process.Start(new ProcessStartInfo() { FileName = screenshotFileName, UseShellExecute = true });
        }
    }
}
