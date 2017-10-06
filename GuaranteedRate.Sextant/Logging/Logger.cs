using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Belikov.GenuineChannels;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.Logging.Console;
using GuaranteedRate.Sextant.Logging.Elasticsearch;
using GuaranteedRate.Sextant.Logging.File;
using GuaranteedRate.Sextant.Logging.Loggly;

namespace GuaranteedRate.Sextant.Logging
{
    public class Logger : IDisposable
    {
        private static volatile IList<ILogAppender> _reporters = new List<ILogAppender>();
        private static readonly object syncRoot = new Object();

        public const string LEVEL = "level";
        public const string ERROR_LEVEL = "ERROR";
        public const string WARN_LEVEL = "WARN";
        public const string INFO_LEVEL = "INFO";
        public const string DEBUG_LEVEL = "DEBUG";
        public const string FATAL_LEVEL = "FATAL";

        public static void Setup(IEncompassConfig config)
        {

            var elasticSearchEnabled = config.GetValue(ElasticsearchLogAppender.ELASTICSEARCH_ENABLED, false);
            if (elasticSearchEnabled)
            {
                AddAppender(new ElasticsearchLogAppender(config));
            }

            var fileEnabled = config.GetValue(FileLogAppender.FILE_ENABLED, false);
            if (fileEnabled)
            {
                AddAppender(new FileLogAppender(config));
            }
            var consoleEnabled = config.GetValue(ConsoleLogAppender.CONSOLE_ENABLED, false);
            if (consoleEnabled)
            {
                AddAppender(new ConsoleLogAppender(config));
            }

            var logglyEnabled = config.GetValue(LogglyLogAppender.LOGGLY_ENABLED, false);
            if (logglyEnabled)
            {
                AddAppender(new LogglyLogAppender(config));
            }

        }

        /// <summary>
        /// Initializes the logger with a single appender
        /// </summary>
        /// <param name="appender"></param>
        public static void Init(ILogAppender appender)
        {
            AddAppender(appender);
        }

        /// <summary>
        /// Adds a log appender to the collection of appenders.  
        /// </summary>
        /// <param name="appender"></param>
        public static void AddAppender(ILogAppender appender)
        {
            lock (syncRoot)
            {
                if (_reporters == null)
                {
                    _reporters = new List<ILogAppender>();
                }
                _reporters.Add(appender);
            }
        }

        public static void AddTag(string tag)
        {
            lock (syncRoot)
            {
                if (_reporters != null)
                {
                    foreach (var reporter in _reporters)
                    {
                        reporter.AddTag(tag);
                    }
                }
            }
        }

        private static IDictionary<string, string> PopulateEvent(string loggerName, string level, string message)
        {
            IDictionary<string, string> fields = new ConcurrentDictionary<string, string>();
            fields.Add(LEVEL, level);
            fields.Add("timestamp", DateTime.UtcNow.ToString());
            fields.Add("hostname", System.Environment.MachineName);
            fields.Add("process", Process.GetCurrentProcess().ProcessName);
            fields.Add("loggerName", loggerName);
            fields.Add("message", message);
            return fields;
        }

        public static void Debug(string logger, string message)
        {
            Log(PopulateEvent(logger, DEBUG_LEVEL, message));
        }

        public static void Error(string logger, string message)
        {
            Log(PopulateEvent(logger, ERROR_LEVEL, message));
        }

        public static void Fatal(string logger, string message)
        {
            Log(PopulateEvent(logger, FATAL_LEVEL, message));
        }

        public static void Info(string logger, string message)
        {
            Log(PopulateEvent(logger, INFO_LEVEL, message));
        }

        public static void Warn(string logger, string message)
        {
            Log(PopulateEvent(logger, WARN_LEVEL, message));
        }

        public static void Log(IDictionary<string, string> fields, string loggerName, string level)
        {
            if (!fields.ContainsKey("logger"))
            {
                fields.Add("logger", loggerName);
            }
            if (!fields.ContainsKey(LEVEL))
            {
                fields.Add(LEVEL, level);
            }
            Log(fields);
        }

        private static void Log(IDictionary<string, string> fields)
        {
            foreach (var r in _reporters)
            {
                r.Log(fields);
            }
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

        /// <summary>
        /// flushes all reporters.  By default blocks for 60 seconds. 
        /// </summary>
        public static void Shutdown(int blockSeconds = 60)
        {
            if (_reporters != null && _reporters.Any())
            {
                var degPar = new ParallelOptions();
                degPar.MaxDegreeOfParallelism = _reporters.Count;
                lock (syncRoot)
                {
                    Parallel.ForEach(_reporters, r =>
                    {
                        System.Console.WriteLine($"disposing {r.GetType().Name}");
                        r.Shutdown(blockSeconds);
                        r.Dispose();
                        System.Console.WriteLine($"disposed {r.GetType().Name}");
                    });
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Logger()
        {
            System.Console.WriteLine("FINALIZED!");
        }
    }
}
