using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Net;
using System.Web;

// Examples:
// http://192.168.1.70:9090/botapi/?command=loaddisc

// run on command line using:
// mono ./Botserver.exe
// mono ./Botserver.exe myconfig.xml

namespace BotServer
{
    class Program
    {
        // optional suffix to application version
        public const String VERSIONSUFFIX = "";

        /// <summary>
        /// Server listens on this port
        /// </summary>
        private const int Port = 9090;

        private static WebServer Server;
        private static Bot Bot;
        private static Configuration Config;

        static void Main(string[] args)
        {
            Config = new Configuration();

            // if config file specified then load it
            if (args.Length == 1)
            {
                Config.Load(args[0]);
            }

            // create bot
            Bot = new Bot(Config);
            
            // start listening for HTTP requests
            Server = new WebServer(ServerResponse, String.Format("http://+:{0}/botapi/", Port));
            Server.Run();

            // keep running until killed
            while (true);
        }

        /// <summary>
        /// Called when a HTTP request is made
        /// Processes the request
        /// </summary>
        /// <param name="Request">Request to process</param>
        /// <returns>Result</returns>
        private static string ServerResponse
            (
            HttpListenerRequest Request
            )
        {
            try
            {
                NameValueCollection Parameters = ParseUrlParameters(Request.RawUrl);

                switch (Parameters["command"].ToLower())
                {
                    case "version":
                        return GetVersion();

                    case "loaddisc":
                        Bot.LoadDisc();
                        break;

                    case "unloaddisc":
                        Bot.UnloadDisc();
                        break;

                    case "test":
                        Bot.Test();
                        break;

                    case "home":
                        Bot.Home();
                        break;

                    default:
                        throw new Exception("Unknown command");
                }
            }
            catch (Exception Exc)
            {
                return "Error: " + Exc.Message;
            }

            return "OK";
        }

        /// <summary>
        /// Extracts the parameters from a URL
        /// </summary>
        /// <param name="RawUrl">URL to parse</param>
        /// <returns>List of parameters and values</returns>
        private static NameValueCollection ParseUrlParameters
            (
            string RawUrl
            )
        {
            String currurl = RawUrl;
            String querystring = null;

            // Check to make sure some query string variables
            // exist and if not add some and redirect.
            int iqs = currurl.IndexOf('?');
            // If query string variables exist, put them in
            // a string.
            if (iqs >= 0)
            {
                querystring = (iqs < currurl.Length - 1) ? currurl.Substring(iqs + 1) : String.Empty;
            }

            // Parse the query string variables into a NameValueCollection.
            NameValueCollection qscoll = HttpUtility.ParseQueryString(querystring);

            return qscoll;
        }

        /// <summary>
        /// Gets the complete version string
        /// </summary>
        /// <returns>String containing version</returns>
        public static String GetVersion
            (
            )
        {
            String Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //Version += " build " + Revision.REVSTR;
            if (VERSIONSUFFIX.Length > 0)
            {
                Version += " " + VERSIONSUFFIX;
            }

            return Version;
        }
    }
}
