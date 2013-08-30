using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JacktheRipperBot
{
    internal class Ripper
    {
        /// <summary>
        /// Rips a DVD
        /// </summary>
        /// <param name="Command">Rip command to execute</param>
        /// <param name="DriveLetter">Letter of DVD drive</param>
        /// <param name="DiscName">Name of DVD</param>
        /// <param name="Timestamp">Timestamp of current time</param>
        public void Rip
            (
            string Command,
            string DriveLetter,
            string DiscName,
            string Timestamp
            )
        {
            // construct complete command
            Command = Command.Replace("{Drive}", DriveLetter).Replace("{Name}", DiscName).Replace("{Timestamp}", Timestamp);

            Program.Log.OutputTimestampLine(Command);

            // fixme = to do
            System.Threading.Thread.Sleep(2000);
        }
    }
}
