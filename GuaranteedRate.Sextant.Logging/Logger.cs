using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace GuaranteedRate.Sextant.Logging
{
    public class Logger
    {
        private static volatile IList<ILogAppender> _reporters;
        private static readonly object syncRoot = new Object();
        private const string ERROR = "ERROR";
        private const string WARN = "WARN";
        private const string INFO = "INFO";
        private const string DEBUG = "DEBUG";
        private const string FATAL = "FATAL";

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

        private static IDictionary<string, string> PopulateEvent(string loggerName, string level, string message)
        {
            IDictionary<string, string> fields = new ConcurrentDictionary<string, string>();
            fields.Add("level", level);
            fields.Add("timestamp", DateTime.Now.ToString("MM/dd/yyyyTHH:mm:ss.fffzzz"));
            fields.Add("hostname", System.Environment.MachineName);
            fields.Add("process", Process.GetCurrentProcess().ProcessName);
            fields.Add("loggerName", loggerName);
            return fields;
        }

        public static void Debug(string logger, string message)
        {
            Log(PopulateEvent(logger, DEBUG, message));
        }

        public static void Error(string logger, string message)
        {
            Log(PopulateEvent(logger, ERROR, message));
        }

        public static void Fatal(string logger, string message)
        {
            Log(PopulateEvent(logger, FATAL, message));
        }

        public static void Info(string logger, string message)
        {
            Log(PopulateEvent(logger, INFO, message));
        }

        public static void Warn(string logger, string message)
        {
            Log(PopulateEvent(logger, WARN, message));
        }

        public static void Log(IDictionary<string, string> fields, string loggerName, string level)
        {
            if (!fields.ContainsKey("logger"))
            {
                fields.Add("logger", loggerName);
            }
            if (!fields.ContainsKey("level"))
            {
                fields.Add("level", level);
            }
            Log(fields);
        }

        private static void Log(IDictionary<string, string> fields)
        {
            lock (syncRoot)
            {
                foreach (var r in _reporters)
                {
                    r.Log(fields);
                }
            }
        }
    }
}
