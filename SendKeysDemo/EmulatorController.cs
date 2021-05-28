using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace SendKeysDemo
{
    public class EmulatorController
    {
        private readonly IEnumerable<Process> emulators;

        public EmulatorController(IEnumerable<Process> emulatorProcesses)
        {
            emulators = emulatorProcesses;
        }

        public void Reset()
        {
            foreach (var emulator in emulators)
            {
                Console.WriteLine($"Resetting emulator {emulator.Id}");
                Win32.SetForegroundWindow(emulator, millisecondsToSleepAfter: 100);
                SendKeys.SendWait("^r");
            }
        }

        public void Press(Button button)
        {
            var command = ButtonToSendKeysString(button);
            if (command == null) return;
            foreach (var emulator in emulators)
            {
                Win32.SetForegroundWindow(emulator, millisecondsToSleepAfter: 100);
                Console.WriteLine($"Pressing {{{button}}} in emulator {emulator.Id}");
                SendKeys.SendWait(command);
            }
        }

        public void SpamPress(Button button, int times, TimeSpan delayBetweenPresses)
        {
            for (int i = 0; i < times; i++)
            {
                Press(button);
                Thread.Sleep(delayBetweenPresses);
            }
        }

        public static string ButtonToSendKeysString(Button button)
        {
            return button switch
            {
                Button.Up => "{UP}",
                Button.Down => "{DOWN}",
                Button.Left => "{LEFT}",
                Button.Right => "{RIGHT}",
                Button.A => "x",
                Button.B => "z",
                Button.L => "a",
                Button.R => "s",
                Button.Start => "{ENTER}",
                Button.Select => "{BACKSPACE}",
                _ => null,
            };
        }
    }

    public enum Button
    {
        Up,
        Down,
        Left,
        Right,
        A,
        B,
        L,
        R,
        Start,
        Select
    }
}
