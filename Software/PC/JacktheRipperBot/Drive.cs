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
    /// Reference: http://www.geekpedia.com/tutorial174_Opening-and-closing-the-CD-tray-in-.NET.html
    /// </summary>
    internal class Drive
    {
        /// <summary>
        /// Time to wait for disc to be scanned and made available in milliseconds
        /// </summary>
        public long MaxDiscWaitTime = 180000L;

        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(String command, StringBuilder buffer, Int32 bufferSize, IntPtr hwndCallback);

        public Drive
            (
            )
        {
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
            
            int Result1 = mciSendString(String.Format("open {0}: type CDAudio alias drive{0} shareable", DriveLetter), null, 0, IntPtr.Zero);
            int Result2 = mciSendString(String.Format("set drive{0} door open", DriveLetter), null, 0, IntPtr.Zero);
            int Result3 = mciSendString(String.Format("close drive{0}", DriveLetter), null, 0, IntPtr.Zero);

            Program.Log.OutputTimestampLine(String.Format("MCI Result codes: {0}, {1}, {2}", Result1, Result2, Result3));
        }

        /// <summary>
        /// Closes the DVD drive tray
        /// </summary>
        /// <param name="Drive">DVD drive</param>
        /// <param name="Wait">true to wait for disc to be inserted</param>
        /// <returns>true for success, false for failure</returns>
        public bool CloseTray
            (
            DriveInfo Drive,
            bool Wait
            )
        {
            string DriveLetter = Drive.Name;
            if (DriveLetter.EndsWith(@":\"))
            {
                DriveLetter = DriveLetter.TrimEnd(new char[] { ':', '\\' });
            }

            int Result1 = mciSendString(String.Format("open {0}: type CDAudio alias drive{0}", DriveLetter), null, 0, IntPtr.Zero);
            int Result2 = mciSendString(String.Format("set drive{0} door closed", DriveLetter), null, 0, IntPtr.Zero);
            int Result3 = mciSendString(String.Format("close drive{0}", DriveLetter), null, 0, IntPtr.Zero);

            Program.Log.OutputTimestampLine(String.Format("MCI Result codes: {0}, {1}, {2}", Result1, Result2, Result3));

            if (Wait)
                return WaitForDisc(Drive);
            else
                return true;
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
            string ScanningFinishedFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ScanningFinished.txt";

            // intialize scanning finished file
            using (StreamWriter Writer = new StreamWriter(ScanningFinishedFile))
            {
                Writer.WriteLine("0");
                Writer.Close();
            }

            Stopwatch Watch = new Stopwatch();

            // wait for scanning to finish or timeout to occur
            using (FileStream ReaderStream = File.Open(ScanningFinishedFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader Reader = new StreamReader(ReaderStream))
                {
                    Watch.Reset();
                    Watch.Start();
                    while (Watch.ElapsedMilliseconds < MaxDiscWaitTime)
                    {
                        ReaderStream.Position = 0;
                        int Finished = int.Parse(Reader.ReadLine());
                        if (Finished == 1) break;

                        System.Threading.Thread.Sleep(1000);
                    }
                    Watch.Stop();
                }
            }

            if (Watch.ElapsedMilliseconds >= MaxDiscWaitTime) return false;

            return true;
        }
    }
}
