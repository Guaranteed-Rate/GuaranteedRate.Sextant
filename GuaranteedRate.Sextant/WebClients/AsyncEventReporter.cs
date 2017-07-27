using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using GuaranteedRate.Sextant.Logging;

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
        private string _url;

        protected const int DEFAULT_QUEUE_SIZE = 1000;
        protected const int DEFAULT_RETRIES = 3;

        protected virtual string Name { get; } = typeof(AsyncEventReporter).Name;
        private volatile bool _finished;

        protected HttpClient Client { get; set; }
        
        public AsyncEventReporter(string url, int queueSize = DEFAULT_QUEUE_SIZE, int retries = DEFAULT_RETRIES)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url", "Base URL must be provided");
            }

            _eventQueue = new BlockingCollection<object>(new ConcurrentQueue<object>(), queueSize);
            _retries = retries;
            CreateClient(url);

            Init();
        }

        public AsyncEventReporter(int queueSize = DEFAULT_QUEUE_SIZE, int retries = DEFAULT_RETRIES)
        {
            _eventQueue = new BlockingCollection<object>(new ConcurrentQueue<object>(), queueSize);
            _retries = retries;

            Init();
        }

        protected void CreateClient(string url)
        {
            _url = url;
            Client = new HttpClient();  
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
                                Logger.Info(Name, $"Post failed, try number: {tries}");
                            }
                            tries++;
                        }
                        if (!success)
                        {
                            Logger.Error(Name, $"Post failed after {tries} tries");
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

        /**
         * This is an empty method that allows subclasses to add 
         * additional functionality
         * 
         */

        protected virtual void ExtraSetup(WebRequest webRequest)
        {

        }

        protected virtual bool PostEvent(object data)
        {
            try
            {
                var content = new ObjectContent<object>(data, new JsonMediaTypeFormatter());
                var response = Client.PostAsync(_url, content).Result;  // Blocking call!
            }
            catch (ApiException ex)
            {
                var error =
                    $"The following request returned a {ex.StackTrace} status code, resource endpoint: {Client.BaseAddress} model: {ex.Message}. Response {ex.Response}";
                Logger.Warn(Name, error);
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

            Client = null;

            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}