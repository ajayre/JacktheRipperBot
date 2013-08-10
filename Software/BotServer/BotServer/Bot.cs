using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace BotServer
{
    internal class Bot
    {
        // default servo positions in microseconds
        // get these values from the Maestro Control Center application
        private int ToolheadGrab = 1265;
        private int ToolheadRelease = 1435;
        private int YAxisDown = 962;
        private int YAxisStop = 950;
        private int YAxisUp = 935;
        private int PivotInTray = 2507;
        private int PivotDrive = 2168;
        private int PivotOutTray = 1823;

        // maximum time for toolhead to complete a move, in milliseconds
        private const int MAX_TOOLHEAD_MOVE_TIME = 5000;
        // maximum time for pivot to complete a move
        private const int MAX_PIVOT_MOVE_TIME = 15000;
        // time it takes to lower disc to drive tray
        private const int YAXIS_LOWER_TO_DRIVE_TIME = 4000;
        // time to allow pivot motion to settle
        private const int PIVOT_SETTLE_TIME = 1000;

        private ServoController Controller = new ServoController();

        public Bot
            (
            )
        {
            // connect to the controller
            Controller.Connect(@"/dev/ttyACM1");

            // configure channels
            Controller.SetSpeed(ServoChannels.Pivot, 6);
            Controller.SetAcceleration(ServoChannels.Pivot, 1);
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

            LowertoDisc();
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
            Controller.SetPositionandWait(ServoChannels.Pivot, PivotInTray, MAX_PIVOT_MOVE_TIME);
            Thread.Sleep(PIVOT_SETTLE_TIME);
        }

        /// <summary>
        /// Moves pivot to over DVD drive
        /// </summary>
        private void PivottoDrive
            (
            )
        {
            Controller.SetPositionandWait(ServoChannels.Pivot, PivotDrive, MAX_PIVOT_MOVE_TIME);
            Thread.Sleep(PIVOT_SETTLE_TIME);
        }

        /// <summary>
        /// Moves pivot to over out tray
        /// </summary>
        private void PivottoOutTray
            (
            )
        {
            Controller.SetPositionandWait(ServoChannels.Pivot, PivotOutTray, MAX_PIVOT_MOVE_TIME);
            Thread.Sleep(PIVOT_SETTLE_TIME);
        }

        /// <summary>
        /// Lifts the Y axis to maximum height
        /// </summary>
        private void Raise
            (
            )
        {
            Controller.SetPosition(ServoChannels.YAxis, YAxisUp);
            while (!Controller.SwitchPressed(ServoChannels.RaisedSwitch)) ;
            Controller.SetPosition(ServoChannels.YAxis, YAxisStop);
        }

        /// <summary>
        /// Lowers the Y axis to a disc
        /// </summary>
        private void LowertoDisc
            (
            )
        {
            // fixme - remove
            LowertoDrive();

            /*Controller.SetPosition(ServoChannels.YAxis, YAxisDown);
            while (!Controller.SwitchPressed(ServoChannels.DiscSwitch)) ;
            Controller.SetPosition(ServoChannels.YAxis, YAxisStop);*/
        }

        /// <summary>
        /// Lowers the Y axis to the DVD drive
        /// </summary>
        private void LowertoDrive
            (
            )
        {
            Controller.SetPosition(ServoChannels.YAxis, YAxisDown);
            Thread.Sleep(YAXIS_LOWER_TO_DRIVE_TIME);
            Controller.SetPosition(ServoChannels.YAxis, YAxisStop);
        }

        /// <summary>
        /// Lowers to the out tray
        /// </summary>
        private void LowertoOutTray
            (
            )
        {
            // no lowering - release disc from maximum height
        }

        /// <summary>
        /// Grabs the disc positioned at the toolhead
        /// </summary>
        private void GrabDisc
            (
            )
        {
            Controller.SetPositionandWait(ServoChannels.Toolhead, ToolheadGrab, MAX_TOOLHEAD_MOVE_TIME);
        }

        /// <summary>
        /// Releases the disc currently being held
        /// </summary>
        private void ReleaseDisc
            (
            )
        {
            Controller.SetPositionandWait(ServoChannels.Toolhead, ToolheadRelease, MAX_TOOLHEAD_MOVE_TIME);
            Controller.Off(ServoChannels.Toolhead);
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
            ToolheadGrab = int.Parse(Settings["toolheadgrab"]);
            ToolheadRelease = int.Parse(Settings["toolheadrelease"]);
        }
    }
}
