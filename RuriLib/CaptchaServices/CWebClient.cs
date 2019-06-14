using System;
using System.Net;

namespace RuriLib.CaptchaServices
{
    /// <summary>
    /// A WebClient with customizable timeout.
    /// </summary>
    public class CWebClient : WebClient
    {
        /// <summary>The maximum time to wait for a response.</summary>
        public int Timeout { get; set; } = 100;

        /// <summary>
        /// Gets the WebRequest.
        /// </summary>
        /// <param name="uri">The uri</param>
        /// <returns>The WebRequest</returns>
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = Timeout * 1000;
            return w;
        }
    }
}