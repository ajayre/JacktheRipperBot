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
        public void Rip
            (
            string Command,
            string DriveLetter,
            string DiscName
            )
        {
            // construct complete command
            Command = Command.Replace("{Drive}", DriveLetter).Replace("{Name}", DiscName);

            // fixme = to do
            System.Threading.Thread.Sleep(2000);
        }
    }
}
