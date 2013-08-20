using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace BotServer
{
    internal class Configuration
    {
        // servo positions in microseconds
        // get these values from the Maestro Control Center application
        public int ToolheadGrab = 1160;
        public int ToolheadRelease = 2000;
        public int YAxisDown = 968;
        public int YAxisStop = 950;
        public int YAxisUp = 935;
        public int PivotInTray = 2527;
        public int PivotDrive = 2145;
        public int PivotOutTray = 1783;

        // maximum time for toolhead to complete a move, in milliseconds
        public int MaxToolheadMoveTime = 5000;
        // maximum time for pivot to complete a move
        public int MaxPivotMoveTime = 15000;
        // time it takes to lower disc to drive tray
        public int YAxisLowertoDriveTime = 4900;
        // time it takes to lower to out tray drop point
        public int YAxisLowertoOutTrayDropTime = 3000;
        // time to allow pivot motion to settle
        public int PivotSettleTime = 1000;

        // pivot speed and acceleration
        public int PivotSpeed = 8;
        public int PivotAcceleration = 0;

        // toolhead speed and acceleration
        public int ToolheadSpeed = 20;
        public int ToolheadAcceleration = 1;

        public string MaestroPort = @"/dev/ttyACM0";

        /// <summary>
        /// Loads a configuation from a file
        /// </summary>
        /// <param name="FileName">Path and name of configuration file</param>
        public void Load
            (
            string FileName
            )
        {
            // check file exists
            if (!File.Exists(FileName))
            {
                throw new Exception(String.Format("The file {0} cannot be found", FileName));
            }

            using (XmlReader Reader = new XmlTextReader(FileName))
            {
                XElement Config = XElement.Load(Reader);

                ToolheadGrab = (int)Config.Element("ToolheadGrab");
                ToolheadRelease = (int)Config.Element("ToolheadRelease");
                YAxisDown = (int)Config.Element("YAxisDown");
                YAxisStop = (int)Config.Element("YAxisStop");
                YAxisUp = (int)Config.Element("YAxisUp");
                PivotInTray = (int)Config.Element("PivotInTray");
                PivotDrive = (int)Config.Element("PivotDrive");
                PivotOutTray = (int)Config.Element("PivotOutTray");
                MaxToolheadMoveTime = (int)Config.Element("MaxToolheadMoveTime");
                MaxPivotMoveTime = (int)Config.Element("MaxPivotMoveTime");
                YAxisLowertoDriveTime = (int)Config.Element("YAxisLowertoDriveTime");
                YAxisLowertoOutTrayDropTime = (int)Config.Element("YAxisLowertoOutTrayDropTime");
                PivotSettleTime = (int)Config.Element("PivotSettleTime");
                PivotSpeed = (int)Config.Element("PivotSpeed");
                PivotAcceleration = (int)Config.Element("PivotAcceleration");
                ToolheadSpeed = (int)Config.Element("ToolheadSpeed");
                ToolheadAcceleration = (int)Config.Element("ToolheadAcceleration");
                MaestroPort = Config.Element("MaestroPort").Value;
            }
        }
    }
}
