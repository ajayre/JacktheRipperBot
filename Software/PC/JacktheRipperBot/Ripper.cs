using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace JacktheRipperBot
{
    internal class Ripper
    {
        /// <summary>
        /// Rips a DVD
        /// </summary>
        /// <param name="Command">Rip command to execute</param>
        /// <param name="Arguments">Arguments to pass to rip command</param>
        /// <param name="DriveLetter">Letter of DVD drive</param>
        /// <param name="DiscName">Name of DVD</param>
        /// <param name="Timestamp">Timestamp of current time</param>
        public void Rip
            (
            string Command,
            string Arguments,
            string DriveLetter,
            string DiscName,
            string Timestamp
            )
        {
            // if no command then don't do anything
            if (Command.Length == 0) return;

            // construct complete arguments
            Arguments = Arguments.Replace("{Drive}", DriveLetter).Replace("{Name}", DiscName).Replace("{Timestamp}", Timestamp);

            Program.Log.OutputTimestampLine(Command + " " + Arguments);

            try
            {
                Process Rip = new Process();
                Rip.StartInfo.FileName = Command;
                Rip.StartInfo.Arguments = Arguments;
                Rip.StartInfo.ErrorDialog = false;
                Rip.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                Rip.Start();

                while (!Rip.HasExited)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception Exc)
            {
                Program.Log.OutputTimestampLine("RIPPING ERROR: " + Exc.Message);
            }
        }
    }
}
