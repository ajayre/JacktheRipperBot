using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;

namespace JacktheRipperBot
{
    internal class Bot
    {
        /// <summary>
        /// Port that the bot is listening on
        /// </summary>
        private const int Port = 9090;
        /// <summary>
        /// Maximum time to wait for the bot to perform an action in milliseconds
        /// </summary>
        private const int BotTimeout = 60000;

        public Bot
            (
            )
        {
        }

        /// <summary>
        /// Loads a disc into the DVD tray
        /// </summary>
        /// <param name="IPAddress">IP address or hostname of bot</param>
        public void LoadDisc
            (
            string IPAddress
            )
        {
            // construct request to bot
            Uri Url = new Uri(new Uri("http://" + IPAddress + ":" + Port.ToString(), UriKind.Absolute), "botapi/").AddQuery("command", "loaddisc");
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(Url);
            Request.Method = WebRequestMethods.Http.Get;

            // send request to bot and get response
            string Result;
            using (var Response = (HttpWebResponse)Request.GetResponse())
            {
                using (var Reader = new StreamReader(Response.GetResponseStream()))
                {
                    Result = Reader.ReadToEnd();
                }
            }

            if (Result.StartsWith("Error")) throw new Exception(Result);
        }

        /// <summary>
        /// Unloads a disc from the DVD tray
        /// </summary>
        /// <param name="IPAddress">IP address or hostname of bot</param>
        public void UnloadDisc
            (
            string IPAddress
            )
        {
            // construct request to bot
            Uri Url = new Uri(new Uri("http://" + IPAddress + ":" + Port.ToString(), UriKind.Absolute), "botapi/").AddQuery("command", "unloaddisc");
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(Url);
            Request.Method = WebRequestMethods.Http.Get;
            Request.Timeout = BotTimeout;

            // send request to bot and get response
            string Result;
            using (var Response = (HttpWebResponse)Request.GetResponse())
            {
                using (var Reader = new StreamReader(Response.GetResponseStream()))
                {
                    Result = Reader.ReadToEnd();
                }
            }

            if (Result.StartsWith("Error")) throw new Exception(Result);
        }
    }

    public static class Net
    {
        /// <summary>
        /// From: http://stackoverflow.com/questions/829080/how-to-build-a-query-string-for-a-url-in-c
        /// Usage:
        ///
        /// Uri url = new Uri("http://localhost/rest/something/browse").
        ///          AddQuery("page", "0").
        ///          AddQuery("pageSize", "200");
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Uri AddQuery(this Uri uri, string name, string value)
        {
            var ub = new UriBuilder(uri);
            var queryString = HttpUtility.ParseQueryString(uri.Query);

            queryString.Add(name, value);

            ub.Query = queryString.ToString();

            return ub.Uri;
        }
    }
}
