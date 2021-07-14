using System;
using System.IO;
using System.Net;
using GuaranteedRate.Sextant.Logging;
using Newtonsoft.Json;

namespace GuaranteedRate.Sextant.WebClients
{
	/**
     * This reporter creates a blocking queue to accept messages AND
     * creates a worker task to process messages from the queue in a separate thead.
     * 
     */

	public class AsyncWebEventReporter : AsyncEventReporter, IDisposable, IEventReporter
    {
        private Func<string> _url;

        protected override string Name { get; } = typeof(AsyncWebEventReporter).Name;

        public AsyncWebEventReporter(Func<string> url, int queueSize = DEFAULT_QUEUE_SIZE, int retries = DEFAULT_RETRIES, int timeout = DEFAULT_TIMEOUT) : base(queueSize, retries, timeout)
        {
            if (url == null) throw new ArgumentNullException("url", "Base URL must be provided");
            
            CreateClient(url);
            Init();
        }

        protected void CreateClient(Func<string> url)
        {
            _url = url;
        }



        /// <summary>
        /// Any additional actions that need to be applied to the WebRequest object
        /// prior to being processed by PostEvent
        /// </summary>
        /// <param name="webRequest"></param>
        protected override void ExtraSetup(WebRequest webRequest)
        {

        }

        protected override bool PostEvent(object formattedData)
        {
            try
            {
                /**
                 * According to documentation, .NET will reuse connection but not WebRequest object
                 */
                WebRequest webRequest = WebRequest.Create(_url());
                if (webRequest != null)
                {
                    webRequest.Method = "POST";
                    webRequest.Timeout = _timeout;
                    webRequest.ContentType = ContentType;
                    ExtraSetup(webRequest);

                    var stringFormattedData = JsonConvert.SerializeObject(formattedData);

                    using (Stream stream = webRequest.GetRequestStream())
                    {
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(stream))
                            sw.Write(stringFormattedData);
                    }

                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            var jsonResponse = sr.ReadToEnd();

                            HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse;
                            if (response != null)
                            {
                                if (!SUCCESS_CODES.Contains(response.StatusCode))
                                {
                                    Logger.Warn(Name,
                                        $"Webservice at {_url()} returned status code: {response.StatusCode}");
                                    return false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Logger.Warn(Name, $"WebService url invalid. url={_url()}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(Name, $"Log by Post to Service: {_url()} failed: {ex}");
                return false;
            }
            return true;
        }



    }
}