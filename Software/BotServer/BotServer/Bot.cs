using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace BotServer
{
    internal class Bot
    {
        // default servo positions in microseconds
        private int ToolheadGrabPosition = 1010;
        private int ToolheadReleasePosition = 1600;
        // time for toolhead to move in milliseconds
        private int ToolheadActuationTime = 1000;

        private ServoController Controller = new ServoController();

        /// <summary>
        /// Servo channel assignments
        /// </summary>
        private enum ServoChannels : byte
        {
            Pivot = 0,
            YAxis = 1,
            Toolhead = 2
        }

        public Bot
            (
            )
        {
            // connect to the controller
            Controller.Connect(@"/dev/ttyACM0");
        }

        /// <summary>
        /// Moves a DVD from the drive tray to the out pile
        /// Assumes currently in the home position (raised and over the drive)
        /// </summary>
        public void UnloadDisc
            (
            )
        {
            Console.WriteLine("Unload disc");

            LowertoDrive();
            GrabDisc();
            Raise();
            PivottoOutTray();
            LowertoOutTray();
            ReleaseDisc();
            Raise();
            PivottoDrive();
        }

        /// <summary>
        /// Moves a DVD from the in pile to the drive tray
        /// Assumes currently in the home position (raised and over the drive)
        /// </summary>
        public void LoadDisc
            (
            )
        {
            Console.WriteLine("Load Disc");

            PivottoInTray();
            LowertoDisc();
            GrabDisc();
            Raise();
            PivottoDrive();
            LowertoDrive();
            ReleaseDisc();
            Raise();
        }

        /// <summary>
        /// Moves to home position
        /// </summary>
        public void Home
            (
            )
        {
            Console.WriteLine("Home");

            ReleaseDisc();
            Raise();
            PivottoDrive();
        }

        /// <summary>
        /// Moves pivot to over in tray
        /// </summary>
        private void PivottoInTray
            (
            )
        {
        }

        /// <summary>
        /// Moves pivot to over DVD drive
        /// </summary>
        private void PivottoDrive
            (
            )
        {
        }

        /// <summary>
        /// Moves pivot to over out tray
        /// </summary>
        private void PivottoOutTray
            (
            )
        {
        }

        /// <summary>
        /// Lifts the Y axis to maximum height
        /// </summary>
        private void Raise
            (
            )
        {
        }

        /// <summary>
        /// Lowers the Y axis to a disc
        /// </summary>
        private void LowertoDisc
            (
            )
        {
        }

        /// <summary>
        /// Lowers the Y axis to the DVD drive
        /// </summary>
        private void LowertoDrive
            (
            )
        {
        }

        /// <summary>
        /// Lowers to the out tray
        /// </summary>
        private void LowertoOutTray
            (
            )
        {
        }

        /// <summary>
        /// Grabs the disc positioned at the toolhead
        /// </summary>
        private void GrabDisc
            (
            )
        {
            Controller.SetPosition((byte)ServoChannels.Toolhead, ToolheadGrabPosition);
            Thread.Sleep(ToolheadActuationTime);
        }

        /// <summary>
        /// Releases the disc currently being held
        /// </summary>
        private void ReleaseDisc
            (
            )
        {
            Controller.SetPosition((byte)ServoChannels.Toolhead, ToolheadReleasePosition);
            Thread.Sleep(ToolheadActuationTime);
            Controller.Off((byte)ServoChannels.Toolhead);
        }

        /// <summary>
        /// Configures the bot
        /// </summary>
        /// <param name="Settings">Settings to use</param>
        public void Configure
            (
            NameValueCollection Settings
            )
        {
            ToolheadGrabPosition = int.Parse(Settings["toolheadgrab"]);
            ToolheadReleasePosition = int.Parse(Settings["toolheadrelease"]);
        }
    }
}
