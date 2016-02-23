using GuaranteedRate.Sextant.Loggers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
        public static ISet<HttpStatusCode> SUCCESS_CODES = new HashSet<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Accepted, HttpStatusCode.Continue };

        private readonly string url;
        private readonly BlockingCollection<string> eventQueue;
        private readonly int retries;

        protected const int DEFAULT_QUEUE_SIZE = 1000;
        protected const int DEFAULT_RETRIES = 3;

        public string ContentType { get; set; }

        private volatile bool finished;

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
            finished = false;

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
                        if (!success)
                        {
                            Loggly.Error(this.GetType().Name.ToString(), "Post failed after " + tries + " tries");
                        }
                    }
                }
                finished = true;
            });
        }

        /**
         * This is the correct way to cleanly shutdown.
         * Once called this method *WILL BLOCK* until the queue has been drained.
         */
        public void Shutdown()
        {
            eventQueue.CompleteAdding();
            while (!finished)
            {
                Thread.Sleep(1000);
            }
        }

        public bool ReportEvent(string formattedData)
        {
            eventQueue.Add(formattedData);
            return true;
        }

        /**
         * This is an empty method that allows subclasses to add 
         * additional functionality
         * 
         */
        protected virtual void ExtraSetup(WebRequest webRequest)
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
                    ExtraSetup(webRequest);

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
                                if (!SUCCESS_CODES.Contains(response.StatusCode))
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
