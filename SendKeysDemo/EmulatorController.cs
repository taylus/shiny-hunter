using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace SendKeysDemo
{
    public class EmulatorController
    {
        private readonly Process emulator;

        public EmulatorController(Process emulatorProcess)
        {
            emulator = emulatorProcess;
        }

        public void Reset()
        {
            Win32.SetForegroundWindow(emulator);
            SendKeys.SendWait("^r");
        }

        public void Press(Button button)
        {
            var command = ButtonToSendKeysString(button);
            if (command != null)
            {
                Win32.SetForegroundWindow(emulator);
                Console.WriteLine($"Pressing {{{button}}}");
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
