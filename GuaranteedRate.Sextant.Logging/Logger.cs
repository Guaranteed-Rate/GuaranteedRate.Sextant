using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using GuaranteedRate.Sextant.Loggers;

namespace GuaranteedRate.Sextant.Logging
{
    public class Logger
    {
        private static volatile IList<ILogReporter> _reporters;
        private static readonly object syncRoot = new Object();
        private const string ERROR = "ERROR";
        private const string WARN = "WARN";
        private const string INFO = "INFO";
        private const string DEBUG = "DEBUG";
        private const string FATAL = "FATAL";

        public static void Init(ILogReporter reporter)
        {
            AddReporter(reporter);
        }

        public static void AddReporter(ILogReporter reporter)
        {
            lock (syncRoot)
            {

                if (_reporters == null)
                {
                    _reporters = new List<ILogReporter>();
                }
                _reporters.Add(reporter);
            }
        }

       

        private static  IDictionary<string, string> PopulateEvent(IDictionary<string, string> fields, string loggerName, string level)
        {
            fields.Add("level", level);
            fields.Add("timestamp", DateTime.Now.ToString("MM/dd/yyyyTHH:mm:ss.fffzzz"));
            fields.Add("hostname", System.Environment.MachineName);
            fields.Add("process", Process.GetCurrentProcess().ProcessName);
            fields.Add("loggerName", loggerName);
            return fields;
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
            Log(PopulateEvent(logger, message, DEBUG));
        }

        public static void Error(string logger, string message)
        {
            Log(PopulateEvent(logger, message, ERROR));
        }

        public static void Fatal(string logger, string message)
        {
            Log(PopulateEvent(logger, message, FATAL));
        }

        public static void Info(string logger, string message)
        {
            Log(PopulateEvent(logger, message, INFO));
        }

        public static void Warn(string logger, string message)
        {
            Log(PopulateEvent(logger, message, WARN));
        }

        void Log(IDictionary<string, string> fields, string loggerName, string level)
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

        static void  Log(IDictionary<string, string> fields)
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

