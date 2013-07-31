using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Timers;

namespace JacktheRipperBot
{
    /// <summary>
    /// Primarily from http://stackoverflow.com/questions/16207628/best-way-to-detect-dvd-insertion-in-drive-c-sharp
    /// and http://www.geekpedia.com/tutorial174_Opening-and-closing-the-CD-tray-in-.NET.html
    /// </summary>
    internal class Drive
    {
        public delegate void OpticalDiskArrivedEventHandler(Object sender, OpticalDiskArrivedEventArgs e);

        /// <summary>
        ///     Gets or sets the time, in seconds, before the drive watcher checks for new media insertion relative to the last occurance of check.
        /// </summary>
        public int Interval = 1;

        /// <summary>
        /// Time to wait for disc to be scanned and made available in milliseconds
        /// </summary>
        public long MaxDiscWaitTime = 120000L;

        private Timer _driveTimer;
        private Dictionary<string, bool> _drives;
        private bool _haveDisk;
        private string ClosingDrive;
        private bool ClosingDriveClosed;
        private Stopwatch Watch = new Stopwatch();

        /// <summary>
        ///     Occurs when a new optical disk is inserted or ejected.
        /// </summary>
        public event OpticalDiskArrivedEventHandler OpticalDiskArrived;

        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(String command, StringBuilder buffer, Int32 bufferSize, IntPtr hwndCallback);

        public Drive
            (
            )
        {
            // start watching for discs to be inserted
            Start();
        }

        private void OnOpticalDiskArrived(OpticalDiskArrivedEventArgs e)
        {
            OpticalDiskArrivedEventHandler handler = OpticalDiskArrived;
            if (handler != null) handler(this, e);
        }

        public void Start()
        {
            _drives = new Dictionary<string, bool>();
            foreach (
                DriveInfo drive in
                    DriveInfo.GetDrives().Where(driveInfo => driveInfo.DriveType.Equals(DriveType.CDRom)))
            {
                _drives.Add(drive.Name, drive.IsReady);
            }
            _driveTimer = new Timer { Interval = Interval * 1000 };
            _driveTimer.Elapsed += DriveTimerOnElapsed;
            _driveTimer.Start();
        }

        public void Stop()
        {
            if (_driveTimer != null)
            {
                _driveTimer.Stop();
                _driveTimer.Dispose();
            }
        }

        private void DriveTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (!_haveDisk)
            {
                try
                {
                    _haveDisk = true;
                    foreach (DriveInfo drive in from drive in DriveInfo.GetDrives()
                                                where drive.DriveType.Equals(DriveType.CDRom)
                                                where _drives.ContainsKey(drive.Name)
                                                where !_drives[drive.Name].Equals(drive.IsReady)
                                                select drive)
                    {
                        _drives[drive.Name] = drive.IsReady;
                        OnOpticalDiskArrived(new OpticalDiskArrivedEventArgs { Drive = drive });
                    }
                }
                finally
                {
                    _haveDisk = false;
                }
            }
        }

        /// <summary>
        /// Opens the DVD drive tray
        /// </summary>
        /// <param name="Drive">DVD drive</param>
        public void OpenTray
            (
            DriveInfo Drive
            )
        {
            string DriveLetter = Drive.Name;
            if (DriveLetter.EndsWith(@":\"))
            {
                DriveLetter = DriveLetter.TrimEnd(new char[] { ':', '\\' });
            }
            
            mciSendString(String.Format("open {0}: type CDAudio alias drive{0}", DriveLetter), null, 0, IntPtr.Zero);
            mciSendString(String.Format("set drive{0} door open", DriveLetter), null, 0, IntPtr.Zero);
        }

        /// <summary>
        /// Closes the DVD drive tray
        /// </summary>
        /// <param name="Drive">DVD drive</param>
        /// <returns>true for success, false for failure</returns>
        public bool CloseTray
            (
            DriveInfo Drive
            )
        {
            string DriveLetter = Drive.Name;
            if (DriveLetter.EndsWith(@":\"))
            {
                DriveLetter = DriveLetter.TrimEnd(new char[] { ':', '\\' });
            }

            mciSendString(String.Format("open {0}: type CDAudio alias drive{0}", DriveLetter), null, 0, IntPtr.Zero);
            mciSendString(String.Format("set drive{0} door closed", DriveLetter), null, 0, IntPtr.Zero);

            return WaitForDisc(Drive);
        }

        /// <summary>
        /// Wait for a disc to be inserted
        /// </summary>
        /// <param name="Drive"></param>
        /// <returns>true for success, false for failure</returns>
        private bool WaitForDisc
            (
            DriveInfo Drive
            )
        {
            // configure drive close detection parameters
            this.OpticalDiskArrived += Drive_OpticalDiskArrived;
            ClosingDrive = Drive.Name;
            ClosingDriveClosed = false;

            // wait for drive to close or timeout to occur
            Watch.Reset();
            Watch.Start();
            while (Watch.ElapsedMilliseconds < MaxDiscWaitTime)
            {
                if (ClosingDriveClosed)
                {
                    try
                    {
                        string Name = Drive.VolumeLabel;
                        break;
                    }
                    catch (IOException)
                    {
                    }
                }

                System.Threading.Thread.Sleep(100);
            }
            Watch.Stop();

            this.OpticalDiskArrived -= Drive_OpticalDiskArrived;

            if (Watch.ElapsedMilliseconds >= MaxDiscWaitTime) return false;

            return true;
        }

        /// <summary>
        /// Called when a disc is inserted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Drive_OpticalDiskArrived(object sender, OpticalDiskArrivedEventArgs e)
        {
            // if this is the drive we are looking for then set flag
            if (e.Drive.Name == ClosingDrive)
            {
                ClosingDriveClosed = true;
            }
        }
    }

    internal class OpticalDiskArrivedEventArgs : EventArgs
    {
        public DriveInfo Drive;
    }
}
