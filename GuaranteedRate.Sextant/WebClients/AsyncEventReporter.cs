using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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

    /// <summary>
    /// This reporter creates a blocking queue to accept messages AND
    /// creates a worker task to process messages from the queue in a separate thead.
    /// </summary>
    public abstract class AsyncEventReporter : IDisposable, IEventReporter
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

        protected readonly BlockingCollection<object> _eventQueue;
        protected readonly int _retries;
        protected readonly int _timeout;
        public string ContentType { get; set; } = "application/json";
        protected const int DEFAULT_QUEUE_SIZE = 1000;
        protected const int DEFAULT_RETRIES = 3;
        protected const int DEFAULT_TIMEOUT = 45000;

        protected abstract string Name { get; }
        protected volatile bool _finished;
        protected bool LogRecurisively = true;


        protected AsyncEventReporter(int queueSize = DEFAULT_QUEUE_SIZE, int retries = DEFAULT_RETRIES, int timeout = DEFAULT_TIMEOUT)
        {
            _eventQueue = new BlockingCollection<object>(new ConcurrentQueue<object>(), queueSize);
            if (retries == 0)
            {
                _retries = 1;
            }
            else
            {
                _retries = retries;
            }
            _timeout = timeout;

            Init();
        }

        protected void Init()
        {
            _finished = false;
            /**
             * Taken directly from
             * https://msdn.microsoft.com/en-us/library/dd997371(v=vs.110).aspx
             */
            Task.Run(() =>
            {
                while (!_eventQueue.IsCompleted)
                {
                    try
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
                        catch (InvalidOperationException) { }

                        if (nextEvent != null)
                        {
                            bool success = false;
                            int tries = 0;

                            while (!success && tries < _retries)
                            {
                                success = PostEvent(nextEvent);
                                if (!success)
                                {
                                    Logger.Info(Name, $"Write failed, try number: {tries}.");
                                }
                                tries++;
                            }
                            if (!success)
                            {
                                Logger.Error(Name, $"Write failed after {tries} tries.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        Logger.Warn(Name, $"Exception processing reporter queue:{ex}");

                        throw;
                    }

                }
                _finished = true;
            });
        }

        /// <summary>
        /// This is the correct way to cleanly shutdown.
        /// Once called this method *WILL BLOCK for for blockSeconds seconds* until the queue has been drained.
        /// </summary>
        /// <param name="blockSeconds"></param>
        public void Shutdown(int blockSeconds)
        {
            Logger.Debug(this.Name, $"shutting down event queue from {new StackTrace()}.");
            _eventQueue.CompleteAdding();
            var sw = new Stopwatch();
            sw.Start();
            while (!_finished)
            {
                if (sw.ElapsedMilliseconds > blockSeconds * 1000)
                {
                    return;
                }
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// This is the correct way to cleanly shutdown.
        /// Once called this method* WILL BLOCK for for 60 seconds* until the queue has been drained.
        /// </summary>
        public void Shutdown()
        {
            Shutdown(60);
        }

        /// <summary>
        /// report event queues it.  Post event actually writes it.
        /// </summary>
        /// <param name="formattedData"></param>
        /// <returns></returns>
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

        protected abstract bool PostEvent(object formattedData);


        private bool disposedValue = false;

        private int _shutdownTimeout = 60;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Shutdown(_shutdownTimeout);
                }
            }

            disposedValue = true;
        }

#pragma warning disable CC0029 // Disposables Should Call Suppress Finalize
        public void Dispose()
#pragma warning restore CC0029 // Disposables Should Call Suppress Finalize
        {
            Dispose(60);
        }

        public void Dispose(int shutdownTimeout = 60)
        {
            _shutdownTimeout = shutdownTimeout;
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}