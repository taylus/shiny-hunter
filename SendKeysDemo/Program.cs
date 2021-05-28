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
            var emulators = Process.GetProcessesByName("mGBA");
            if (!emulators.Any())
            {
                Console.Error.WriteLine("Error: mGBA is not running");
                Console.Error.WriteLine($"Start it first with a pre-arranged save file and fast forward speed set to {fastForwardSpeed}x");
                Environment.Exit(-1);
            }

            foreach (var emulator in emulators)
            {
                Console.WriteLine($"Attached to {emulator.ProcessName}, PID: {emulator.Id}");
            }

            Console.WriteLine("Press ENTER to begin shiny hunting.");
            Console.ReadLine();
            Console.WriteLine("Beginning shiny hunt!");

            var screenshotDir = Directory.CreateDirectory(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
            for (int attempt = 1; attempt <= 8192; attempt++)
            {
                ShinyHuntFireRedStarterSoftReset(emulators, attempt, screenshotDir.Name, fastForwardSpeed);
            }
        }

        private static void ShinyHuntFireRedStarterSoftReset(Process[] emulators, int attempt, string screenshotDir, double fastForwardSpeed = 1)
        {
            var controller = new EmulatorController(emulators);
            controller.Reset();
            Thread.Sleep(1000);

            //get past intro and into the main menu
            //these timings are very dependent on the emulator and PC running it :s
            if (fastForwardSpeed == 0) fastForwardSpeed = 1;
            controller.SpamPress(Button.Start, 8, TimeSpan.FromMilliseconds(400 / fastForwardSpeed));

            //continue saved game, skip the "previously on your quest..." recap, and select the starter in front of you
            controller.SpamPress(Button.A, 8, TimeSpan.FromMilliseconds(300 / fastForwardSpeed));

            //decline naming the starter, skip through Gary's dialogue
            Thread.Sleep(TimeSpan.FromMilliseconds(3500 / fastForwardSpeed));
            controller.SpamPress(Button.B, 10, TimeSpan.FromMilliseconds(400 / fastForwardSpeed));

            //open menu and check the starter for shiny
            controller.Press(Button.Start);
            Thread.Sleep(TimeSpan.FromMilliseconds(1000 / fastForwardSpeed));
            controller.SpamPress(Button.A, 3, TimeSpan.FromMilliseconds(400 / fastForwardSpeed));
            Thread.Sleep(TimeSpan.FromMilliseconds(1000 / fastForwardSpeed));

            foreach (var emulator in emulators)
            {
                //screenshot the starter's stat screen
                try
                {
                    var screenshotFile = Path.Combine(screenshotDir, $"{attempt:0000}_{emulator.Id}.png");
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
}
