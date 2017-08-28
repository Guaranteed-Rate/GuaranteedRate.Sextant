using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.Logging.Elasticsearch;
using GuaranteedRate.Sextant.Logging.Loggly;

namespace GuaranteedRate.Sextant.Logging
{
    public class Logger
    {
        private static volatile IList<ILogAppender> _reporters = new List<ILogAppender>();
        private static readonly object syncRoot = new Object();

        public const string LEVEL = "level";
        public const string ERROR = "ERROR";
        public const string WARN = "WARN";
        public const string INFO = "INFO";
        public const string DEBUG = "DEBUG";
        public const string FATAL = "FATAL";

        public static void Setup(IEncompassConfig config)
        {
            var logglyEnabled = config.GetValue(LogglyLogAppender.LOGGLY_ENABLED, false);
            if (logglyEnabled)
            {
                AddAppender(new LogglyLogAppender(config));
            }

            var elasticSearchEnabled = config.GetValue(ElasticsearchLogAppender.ELASTICSEARCH_ENABLED, false);
            if (elasticSearchEnabled)
            {
                AddAppender(new ElasticsearchLogAppender(config));
            }

            var consoleEnabled = config.GetValue(ConsoleLogAppender.CONSOLE_ENABLED, false);
            if (consoleEnabled)
            {
                AddAppender(new ConsoleLogAppender(config));
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
            lock(syncRoot)
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
            fields.Add("timestamp", DateTime.Now.ToString("MM/dd/yyyyTHH:mm:ss.fffzzz"));
            fields.Add("hostname", System.Environment.MachineName);
            fields.Add("process", Process.GetCurrentProcess().ProcessName);
            fields.Add("loggerName", loggerName);
            fields.Add("message", message);
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
            if (!fields.ContainsKey(LEVEL))
            {
                fields.Add(LEVEL, level);
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
