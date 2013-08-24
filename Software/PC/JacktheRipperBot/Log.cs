using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JacktheRipperBot
{
    public class Log
    {
        private StreamWriter Writer = null;

        public Log
            (
            )
        {
            string LogFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\JacktheRipperBot-";
            LogFile += DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";

            FileStream LogStream = File.Open(LogFile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            Writer = new StreamWriter(LogStream);
            Writer.AutoFlush = true;
        }

        public void Close
            (
            )
        {
            Writer.Flush();
            Writer.Close();
            Writer = null;
        }

        /// <summary>
        /// Outputs text to the log
        /// </summary>
        /// <param name="Text">Text to output</param>
        public void Output
            (
            string Text
            )
        {
            if (Writer == null) return;

            Writer.Write(Text);
        }

        /// <summary>
        /// Outputs a text to the log and appends a newline
        /// </summary>
        /// <param name="Text">Text to output</param>
        public void OutputLine
            (
            string Text
            )
        {
            if (Writer == null) return;

            Writer.WriteLine(Text);
        }

        /// <summary>
        /// Outputs a timestamp followed by the text and then a newline, to the log
        /// </summary>
        /// <param name="Text">Text to output</param>
        public void OutputTimestampLine
            (
            string Text
            )
        {
            OutputLine(DateTime.Now.ToString() + ": " + Text);
        }
    }
}
