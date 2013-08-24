using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Forms;

namespace JacktheRipperBot
{
    static class Program
    {
        // optional suffix to application version
        public const String VERSIONSUFFIX = "";

        public static Log Log = new Log();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            // save user settings
            Properties.Settings.Default.Save();

            Log.Close();
        }

        /// <summary>
        /// Catch unhandled exceptions on worker threads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(ToFullDisplayString((Exception)e.ExceptionObject), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Catch unhandled exceptions on user interface thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(ToFullDisplayString(e.Exception), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // gets the complete version string
        public static String GetVersion
            (
            )
        {
            String Version = Application.ProductVersion;
            //Version += " build " + Revision.REVSTR;
            if (VERSIONSUFFIX.Length > 0)
            {
                Version += " " + VERSIONSUFFIX;
            }

            return Version;
        }

        // gets the complete copyright string
        public static String GetCopyright
            (
            )
        {
            // get all copyright attributes on this assembly
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

            // if there aren't any copyright attributes, return an empty string
            if (attributes.Length == 0)
                return "";

            // if there is a copyright attribute, return its value
            return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
        }

        public static string ToFullDisplayString
            (
            Exception ex
            )
        {
            StringBuilder DisplayText = new StringBuilder();

            DisplayText.AppendFormat("Oops, an error occurred. Press Ctrl+C to copy this text to the clipboard then paste into an email along with a detailed description of what you were doing to andy@britishideas.com.{0}{0}",
                Environment.NewLine);

            WriteExceptionDetail(DisplayText, ex);

            foreach (Exception Inner in GetNestedExceptionList(ex))
            {
                DisplayText.AppendFormat("Inner exception start{0}", Environment.NewLine);
                WriteExceptionDetail(DisplayText, Inner);
                DisplayText.AppendFormat("Inner exception end{0}", Environment.NewLine);
            }
            return DisplayText.ToString();
        }

        public static void WriteExceptionDetail
            (
            StringBuilder Builder,
            Exception ex
            )
        {
            Builder.AppendFormat("Message: {0}{1}", ex.Message, Environment.NewLine);
            Builder.AppendFormat("Type: {0}{1}", ex.GetType(), Environment.NewLine);
            Builder.AppendFormat("HelpLink: {0}{1}", ex.HelpLink, Environment.NewLine);
            Builder.AppendFormat("Source: {0}{1}", ex.Source, Environment.NewLine);
            Builder.AppendFormat("TargetSite: {0}{1}", ex.TargetSite, Environment.NewLine);
            Builder.AppendFormat("Data: {0}", Environment.NewLine);
            foreach (DictionaryEntry de in ex.Data)
            {
                Builder.AppendFormat("\t{0} : {1}", de.Key, de.Value);
            }
            Builder.AppendFormat("StackTrace: {0}{1}", ex.StackTrace, Environment.NewLine);
        }

        public static IEnumerable<Exception> GetNestedExceptionList
            (
            Exception ex
            )
        {
            Exception Current = ex;
            do
            {
                Current = Current.InnerException;
                if (Current != null)
                {
                    yield return Current;
                }
            }
            while (Current != null);
        }    
    }
}
