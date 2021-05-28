using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SendKeysDemo
{
    public class Program
    {
        public static void Main()
        {
            var emulator = Process.GetProcessesByName("mGBA").FirstOrDefault();
            if (emulator == null)
            {
                Console.Error.WriteLine("Error: mGBA is not running");
                Environment.Exit(-1);
            }

            Win32.SetForegroundWindow(emulator, millisecondsToSleepAfter: 100);
            Win32.ShowWindow(emulator);

            Console.WriteLine($"Attached to {emulator.ProcessName}, PID: {emulator.Id}");
            Console.WriteLine("Beginning shiny hunt!");
            for (int attempt = 1; ; attempt++)
            {
                ShinyHuntFireRedStarterSoftReset(emulator, attempt);
            }
        }

        private static void ShinyHuntFireRedStarterSoftReset(Process emulator, int attempt)
        {
            Console.WriteLine($"Resetting emulator for attempt #{attempt}");

            var controller = new EmulatorController(emulator);
            controller.Reset();

            //get past intro and into the main menu
            controller.SpamPress(Button.Start, 20, TimeSpan.FromMilliseconds(400));

            //continue saved game, skip the "previously on your quest..." recap, and select the starter in front of you
            controller.SpamPress(Button.A, 32, TimeSpan.FromMilliseconds(300));

            //decline naming the starter, skip through Gary's dialogue
            Thread.Sleep(3500);
            controller.SpamPress(Button.B, 25, TimeSpan.FromMilliseconds(400));

            //open menu and check the starter for shiny
            controller.SpamPress(Button.Start, 3, TimeSpan.FromMilliseconds(50));
            Thread.Sleep(1000);
            controller.SpamPress(Button.A, 8, TimeSpan.FromMilliseconds(400));
            Thread.Sleep(1000);

            //screenshot the starter's stat screen
            var screenshotFile = $"{Guid.NewGuid()}.png";
            Console.WriteLine($"Saving screenshot {screenshotFile}");
            Screenshot.Save(emulator, screenshotFile);
        }
    }
}
