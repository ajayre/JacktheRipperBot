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
        private ServoController Controller = new ServoController();
        private Configuration Config;

        public Bot
            (
            Configuration Config
            )
        {
            this.Config = Config;

            // connect to the controller
            Controller.Connect(Config.MaestroPort);

            // configure channels
            Controller.Off(ServoChannels.Pivot);
            Controller.SetSpeed(ServoChannels.Pivot, Config.PivotSpeed);
            Controller.SetAcceleration(ServoChannels.Pivot, Config.PivotAcceleration);

            Controller.Off(ServoChannels.YAxis);
            Controller.SetSpeed(ServoChannels.YAxis, 0);
            Controller.SetAcceleration(ServoChannels.YAxis, 0);

            Controller.Off(ServoChannels.Toolhead);
            Controller.SetSpeed(ServoChannels.Toolhead, 0);
            Controller.SetAcceleration(ServoChannels.Toolhead, Config.ToolheadAcceleration);
        }

        /// <summary>
        /// Moves a DVD from the drive tray to the out pile
        /// Assumes currently in the home position (raised and over the drive)
        /// </summary>
        public void UnloadDisc
            (
            )
        {
            ReleaseDisc();
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
            ReleaseDisc();
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
        /// Moves a disc from in tray to out tray without putting it
        /// into the drive
        /// </summary>
        public void Test
            (
            )
        {
            for (int i = 0; i < 5; i++)
            {
                LoadDisc();
                UnloadDisc();
            }
        }

        /// <summary>
        /// Moves to home position
        /// </summary>
        public void Home
            (
            )
        {
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
            Controller.SetPositionandWait(ServoChannels.Pivot, Config.PivotInTray, Config.MaxPivotMoveTime);
            Thread.Sleep(Config.PivotSettleTime);
        }

        /// <summary>
        /// Moves pivot to over DVD drive
        /// </summary>
        private void PivottoDrive
            (
            )
        {
            Controller.SetPositionandWait(ServoChannels.Pivot, Config.PivotDrive, Config.MaxPivotMoveTime);
            Thread.Sleep(Config.PivotSettleTime);
        }

        /// <summary>
        /// Moves pivot to over out tray
        /// </summary>
        private void PivottoOutTray
            (
            )
        {
            Controller.SetPositionandWait(ServoChannels.Pivot, Config.PivotOutTray, Config.MaxPivotMoveTime);
            Thread.Sleep(Config.PivotSettleTime);
        }

        /// <summary>
        /// Lifts the Y axis to maximum height
        /// </summary>
        public void Raise
            (
            )
        {
            Controller.SetPosition(ServoChannels.YAxis, Config.YAxisUp);
            while (!Controller.SwitchPressed(ServoChannels.RaisedSwitch)) ;
            Controller.SetPosition(ServoChannels.YAxis, Config.YAxisStop);
        }

        /// <summary>
        /// Lowers the Y axis to a disc
        /// </summary>
        private void LowertoDisc
            (
            )
        {
            Controller.SetPosition(ServoChannels.YAxis, Config.YAxisDown);
            while (!Controller.SwitchPressed(ServoChannels.DiscSwitch)) ;
            Controller.SetPosition(ServoChannels.YAxis, Config.YAxisStop);
        }

        /// <summary>
        /// Lowers the Y axis to the DVD drive
        /// </summary>
        private void LowertoDrive
            (
            )
        {
            Controller.SetPosition(ServoChannels.YAxis, Config.YAxisDown);
            Thread.Sleep(Config.YAxisLowertoDriveTime);
            Controller.SetPosition(ServoChannels.YAxis, Config.YAxisStop);
        }

        /// <summary>
        /// Lowers to the out tray
        /// </summary>
        private void LowertoOutTray
            (
            )
        {
            Controller.SetPosition(ServoChannels.YAxis, Config.YAxisDown);
            Thread.Sleep(Config.YAxisLowertoOutTrayDropTime);
            Controller.SetPosition(ServoChannels.YAxis, Config.YAxisStop);            
        }

        /// <summary>
        /// Grabs the disc positioned at the toolhead
        /// </summary>
        public void GrabDisc
            (
            )
        {
            for (int Position = Config.ToolheadRelease; Position >= Config.ToolheadGrab; Position -= 10)
            {
                Controller.SetPositionandWait(ServoChannels.Toolhead, Position, Config.MaxToolheadMoveTime);
                Thread.Sleep(Config.ToolheadSpeed);
            }
        }

        /// <summary>
        /// Releases the disc currently being held
        /// </summary>
        public void ReleaseDisc
            (
            )
        {
            Controller.SetPositionandWait(ServoChannels.Toolhead, Config.ToolheadRelease, Config.MaxToolheadMoveTime);
            Controller.Off(ServoChannels.Toolhead);
        }
    }
}
