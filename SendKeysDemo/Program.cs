using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace SendKeysDemo
{
    public class Program
    {
        public static void Main()
        {
            const double fastForwardSpeed = 10;
            var emulator = Process.GetProcessesByName("mGBA").FirstOrDefault();
            if (emulator == null)
            {
                Console.Error.WriteLine("Error: mGBA is not running");
                Console.Error.WriteLine($"Start it first with a pre-arranged save file and fast forward speed set to {fastForwardSpeed}x");
                Environment.Exit(-1);
            }

            Console.WriteLine($"Attached to {emulator.ProcessName}, PID: {emulator.Id}");
            Console.WriteLine("Press ENTER to begin shiny hunting.");
            Console.ReadLine();

            Win32.SetForegroundWindow(emulator, millisecondsToSleepAfter: 100);
            Win32.ShowWindow(emulator);

            Console.WriteLine("Beginning shiny hunt!");
            var screenshotDir = Directory.CreateDirectory(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
            for (int attempt = 1; attempt <= 8192; attempt++)
            {
                ShinyHuntFireRedStarterSoftReset(emulator, attempt, screenshotDir.Name, fastForwardSpeed);
            }
        }

        private static void ShinyHuntFireRedStarterSoftReset(Process emulator, int attempt, string screenshotDir, double fastForwardSpeed = 1)
        {
            Console.WriteLine($"Resetting emulator for attempt #{attempt}");

            var controller = new EmulatorController(emulator);
            controller.Reset();
            Thread.Sleep(1000);

            //get past intro and into the main menu
            //these timings are very dependent on the emulator and PC running it :s
            if (fastForwardSpeed == 0) fastForwardSpeed = 1;
            controller.SpamPress(Button.Start, 20, TimeSpan.FromMilliseconds(400 / fastForwardSpeed));

            //continue saved game, skip the "previously on your quest..." recap, and select the starter in front of you
            controller.SpamPress(Button.A, 20, TimeSpan.FromMilliseconds(300 / fastForwardSpeed));

            //decline naming the starter, skip through Gary's dialogue
            Thread.Sleep(TimeSpan.FromMilliseconds(3500 / fastForwardSpeed));
            controller.SpamPress(Button.B, 25, TimeSpan.FromMilliseconds(400 / fastForwardSpeed));

            //open menu and check the starter for shiny
            controller.Press(Button.Start);
            Thread.Sleep(TimeSpan.FromMilliseconds(1000 / fastForwardSpeed));
            controller.SpamPress(Button.A, 5, TimeSpan.FromMilliseconds(400 / fastForwardSpeed));
            Thread.Sleep(TimeSpan.FromMilliseconds(1000 / fastForwardSpeed));

            //screenshot the starter's stat screen
            try
            {
                var screenshotFile = Path.Combine(screenshotDir, $"{attempt:0000}.png");
                Console.WriteLine($"Saving screenshot {screenshotFile}");
                Screenshot.Save(emulator, screenshotFile);
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Error saving screenshot (did the emulator close?)");
                throw;
            }
        }
    }
}
