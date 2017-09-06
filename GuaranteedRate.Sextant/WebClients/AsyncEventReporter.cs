using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GuaranteedRate.Sextant.Logging;
using Newtonsoft.Json;

namespace GuaranteedRate.Sextant.WebClients
{
    /**
     * This reporter creates a blocking queue to accept messages AND
     * creates a worker task to process messages from the queue in a separate thead.
     * 
     */

    public class AsyncEventReporter : IDisposable, IEventReporter
    {
        /// <summary>
        /// This is the set of http status codes that are condsidered successful
        /// responses.  The list does not include all 2xx codes because the class
        /// does not have any ability to relay information back to the original
        /// class that wanted the data sent.
        /// </summary>
        public static ISet<HttpStatusCode> SUCCESS_CODES = new HashSet<HttpStatusCode>
        {
            HttpStatusCode.Accepted,
            HttpStatusCode.Continue,
            HttpStatusCode.Created,
            HttpStatusCode.NoContent,
            HttpStatusCode.OK
        };

        private readonly BlockingCollection<object> _eventQueue;
        private readonly int _retries;
        private readonly int _timeout;
        private string _url;
        public string ContentType { get; set; } = "application/json";
        protected const int DEFAULT_QUEUE_SIZE = 1000;
        protected const int DEFAULT_RETRIES = 3;
        protected const int DEFAULT_TIMEOUT = 45000;
        
        protected virtual string Name { get; } = typeof (AsyncEventReporter).Name;
        private volatile bool _finished;
        

        public AsyncEventReporter(string url, int queueSize = DEFAULT_QUEUE_SIZE, int retries = DEFAULT_RETRIES, int timeout = DEFAULT_TIMEOUT)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url", "Base URL must be provided");
            }

            _eventQueue = new BlockingCollection<object>(new ConcurrentQueue<object>(), queueSize);
            _retries = retries;
            _timeout = timeout;
            CreateClient(url);

            Init();
        }

        public AsyncEventReporter(int queueSize = DEFAULT_QUEUE_SIZE, int retries = DEFAULT_RETRIES, int timeout = DEFAULT_TIMEOUT)
        {
            _eventQueue = new BlockingCollection<object>(new ConcurrentQueue<object>(), queueSize);
            _retries = retries;
            _timeout = timeout;

            Init();
        }

        protected void CreateClient(string url)
        {
            _url = url;
        }

        private void Init()
        {
            _finished = false;

            /**
             * Taken directly from
             * https://msdn.microsoft.com/en-us/library/dd997371(v=vs.110).aspx
             */
            // A simple blocking consumer with no cancellation.

            Task.Run(() =>
            {
                while (!_eventQueue.IsCompleted)
                {
                    object nextEvent = null;
                    // Blocks if number.Count == 0 
                    // IOE means that Take() was called on a completed collection. 
                    // Some other thread can call CompleteAdding after we pass the 
                    // IsCompleted check but before we call Take.  
                    // In this example, we can simply catch the exception since the  
                    // loop will break on the next iteration. 
                    try
                    {
                        nextEvent = _eventQueue.Take();
                    }
                    catch (InvalidOperationException e)
                    {
                        Logger.Warn(Name, $"InvalidOperationException reading from queue: {e}");
                    }

                    if (nextEvent != null)
                    {
                        bool success = false;
                        int tries = 0;
                        while (!success && tries < _retries)
                        {
                            success = PostEvent(nextEvent);
                            if (!success)
                            {
                                Logger.Info(Name, $"Post failed, try number: {tries} to url: {_url}");
                            }
                            tries++;
                        }
                        if (!success)
                        {
                            Logger.Error(Name, $"Post failed after {tries} tries to url: {_url}");
                        }
                    }
                }
                _finished = true;
            });
        }

        /**
         * This is the correct way to cleanly shutdown.
         * Once called this method *WILL BLOCK* until the queue has been drained.
         */

        public void Shutdown()
        {
            _eventQueue.CompleteAdding();
            while (!_finished)
            {
                Thread.Sleep(1000);
            }
        }

        public bool ReportEvent(object formattedData)
        {
            _eventQueue.Add(formattedData);
            return true;
        }

        /// <summary>
        /// Any additional actions that need to be applied to the WebRequest object
        /// prior to being processed by PostEvent
        /// </summary>
        /// <param name="webRequest"></param>
        protected virtual void ExtraSetup(WebRequest webRequest)
        {

        }

        protected virtual bool PostEvent(object formattedData)
        {
            try
            {
                /**
                 * According to documentation, .NET will reuse connection but not WebRequest object
                 */
                WebRequest webRequest = WebRequest.Create(_url);
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
                                    Logger.Warn(Name, $"Webservice at {_url} returned status code: {response.StatusCode}");
                                    return false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Logger.Warn(Name, $"WebService url invalid. url={_url}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(Name, $"Log by Post to Service: {_url} failed: {ex}");
                return false;
            }
            return true;
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Shutdown();
                }
            }

            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}