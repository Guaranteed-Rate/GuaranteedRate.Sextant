using GuaranteedRate.Sextant.Loggers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.WebClients
{
    /**
     * This reporter creates a blocking queue to accept messages AND
     * creates a worker task to process messages from the queue in a separate thead.
     * 
     */
    public class AsyncEventReporter : IEventReporter
    {
        private readonly string url;
        private readonly BlockingCollection<string> eventQueue;
        private readonly int retries;

        protected const int DEFAULT_QUEUE_SIZE = 1000;
        protected const int DEFAULT_RETRIES = 3;

        public string ContentType { get; set; }

        public AsyncEventReporter(string url, int queueSize = DEFAULT_QUEUE_SIZE, int retries = DEFAULT_RETRIES)
        {
            this.url = url;
            ContentType = "application/json";
            this.eventQueue = new BlockingCollection<string>(new ConcurrentQueue<string>(), queueSize);
            this.retries = retries;
            init();
        }

        private void init()
        {
            /**
             * Taken directly from
             * https://msdn.microsoft.com/en-us/library/dd997371(v=vs.110).aspx
             */
            // A simple blocking consumer with no cancellation.
            Task.Run(() =>
            {
                while (!eventQueue.IsCompleted)
                {

                    string nextEvent = null;
                    // Blocks if number.Count == 0 
                    // IOE means that Take() was called on a completed collection. 
                    // Some other thread can call CompleteAdding after we pass the 
                    // IsCompleted check but before we call Take.  
                    // In this example, we can simply catch the exception since the  
                    // loop will break on the next iteration. 
                    try
                    {
                        nextEvent = eventQueue.Take();
                    }
                    catch (InvalidOperationException e) 
                    {
                        Loggly.Warn(this.GetType().Name.ToString(), "InvalidOperationException reading from queue:" + e);
                    }

                    if (nextEvent != null)
                    {
                        bool success = false;
                        int tries = 0;
                        while (!success && tries < retries) 
                        { 
                            success = PostEvent(nextEvent);
                            if (!success)
                            {
                                Loggly.Info(this.GetType().Name.ToString(), "Post failed, try number: " + tries);
                            }
                            tries++;
                        }
                    }
                }
            });
        }

        /**
         * This is the correct way to cleanly shutdown.
         * Once called you can't add new events to the queue, but you can still process
         * the existing events.
         * 
         * Unclear how much the Encompass Plugin lifecycle respects this
         */
        public void Shutdown()
        {
            eventQueue.CompleteAdding();
        }

        public bool ReportEvent(string formattedData)
        {
            eventQueue.Add(formattedData);
            return true;
        }

        protected void ExtraSetup(WebRequest webRequest)
        {

        }

        private bool PostEvent(string formattedData)
        {
            try
            {
                /**
                 * According to documentation, .NET will reuse connection but not WebRequest object
                 */
                WebRequest webRequest = WebRequest.Create(url);
                if (webRequest != null)
                {
                    webRequest.Method = "POST";
                    webRequest.Timeout = 45000;
                    webRequest.ContentType = ContentType;

                    using (Stream stream = webRequest.GetRequestStream())
                    {
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(stream))
                            sw.Write(formattedData);
                    }

                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            var jsonResponse = sr.ReadToEnd();

                            HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse;
                            if (response != null)
                            {
                                if (response.StatusCode != HttpStatusCode.OK)
                                {
                                    Loggly.Warn(this.GetType().Name.ToString(), "Webservice at " + url + " returned status code:" + response.StatusCode);
                                    return false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Loggly.Warn(this.GetType().Name.ToString(), "WebService url invalid. url=" + url);
                }
            }
            catch (Exception ex)
            {
                Loggly.Error(this.GetType().Name.ToString(), "Log by Post to Service failed: " + ex.ToString());
                return false;
            }
            return true;
        }
    }
}
