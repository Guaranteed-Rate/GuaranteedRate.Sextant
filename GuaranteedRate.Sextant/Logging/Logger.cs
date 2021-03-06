﻿using GuaranteedRate.Sextant.Config;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.Logging
{
    public class Logger : IDisposable
    {
        private static readonly object syncRoot = new Object();
        private static bool configured = false;
        public const string LEVEL = "level";
        public const string ERROR_LEVEL = "ERROR";
        public const string VERBOSE_LEVEL = "VERBOSE";
        public const string WARN_LEVEL = "WARN";
        public const string INFO_LEVEL = "INFO";
        public const string DEBUG_LEVEL = "DEBUG";
        public const string FATAL_LEVEL = "FATAL";
        private const string MESSAGE_KEY = "message";
        private const string TIMESTAMP_KEY = "timestamp";
        private const string LOGGERNAME_KEY = "loggerName";
        private static ConcurrentDictionary<string, string> _additionalTags;

        #region config mappings

        public static string ELASTICSEARCH_ENABLED = "ElasticsearchLogAppender.Enabled";
        public static string ELASTICSEARCH_URL = "ElasticsearchLogAppender.Url";
        public static string ELASTICSEARCH_QUEUE_SIZE = "ElasticsearchLogAppender.QueueSize";
        public static string ELASTICSEARCH_RETRY_LIMIT = "ElasticsearchLogAppender.RetryLimit";
        public static string ELASTICSEARCH_ALL = "ElasticsearchLogAppender.All.Enabled";
        public static string ELASTICSEARCH_ERROR = "ElasticsearchLogAppender.Error.Enabled";
        public static string ELASTICSEARCH_WARN = "ElasticsearchLogAppender.Warn.Enabled";
        public static string ELASTICSEARCH_INFO = "ElasticsearchLogAppender.Info.Enabled";
        public static string ELASTICSEARCH_DEBUG = "ElasticsearchLogAppender.Debug.Enabled";
        public static string ELASTICSEARCH_MIN_LEVEL = "ElasticsearchLogAppender.Debug.Enabled";
        public static string ELASTICSEARCH_FATAL = "ElasticsearchLogAppender.Fatal.Enabled";
        public static string ELASTICSEARCH_TAGS = "ElasticsearchLogAppender.Tags";
        public static string ELASTICSEARCH_INDEX_NAME = "ElasticsearchLogAppender.IndexName";
        public static string ELASTICSEARCH_LOG_RECURSIVELY = "ElasticsearchLogAppender.LogRecursively";
        public static string ELASTICSEARCH_APPNAME = "ElasticsearchLogAppender.AppName";
        public static string ELASTICSEARCH_ENVIRONMENT = "ElasticsearchLogAppender.Environment";

        public static string FILE_ENABLED = "FileLogAppender.Enabled";
        public static string FILE_FOLDER = "FileLogAppender.Folder";
        public static string FILE_NAME = "FileLogAppender.LogName";
        public static string FILE_QUEUE_SIZE = "FileLogAppender.QueueSize";
        public static string FILE_RETRY_LIMIT = "FileLogAppender.RetryLimit";
        public static string FILE_ALL = "FileLogAppender.All.Enabled";
        public static string FILE_ERROR = "FileLogAppender.Error.Enabled";
        public static string FILE_WARN = "FileLogAppender.Warn.Enabled";
        public static string FILE_INFO = "FileLogAppender.Info.Enabled";
        public static string FILE_DEBUG = "FileLogAppender.Debug.Enabled";
        public static string FILE_FATAL = "FileLogAppender.Fatal.Enabled";
        public static string FILE_TAGS = "FileLogAppender.Tags";
        public static string FILE_MAX_FILE_BYTES = "FileLogAppender.MaxFileBytes";
        public static string FILE_MAX_FILES = "10";
        public static string FILE_MESSAGE_FORMAT = "FileLogAppender.MessageFormat";

        public static string CONSOLE_ENABLED = "ConsoleLogAppender.Enabled";
        public static string CONSOLE_ALL = "ConsoleLogAppender.All.Enabled";
        public static string CONSOLE_ERROR = "ConsoleLogAppender.Error.Enabled";
        public static string CONSOLE_WARN = "ConsoleLogAppender.Warn.Enabled";
        public static string CONSOLE_INFO = "ConsoleLogAppender.Info.Enabled";
        public static string CONSOLE_DEBUG = "ConsoleLogAppender.Debug.Enabled";
        public static string CONSOLE_VERBOSE = "ConsoleLogAppender.Verbose.Enabled";
        public static string CONSOLE_FATAL = "ConsoleLogAppender.Fatal.Enabled";

        public static string LOGGLY_ENABLED = "LogglyLogAppender.Enabled";
        public static string LOGGLY_URL = "LogglyLogAppender.Url";
        public static string LOGGLY_APPLICATION_NAME = "LogglyLogAppender.ApplicationName";
        public static string LOGGLY_APIKEY = "LogglyLogAppender.ApiKey";
        public static string LOGGLY_QUEUE_SIZE = "LogglyLogAppender.QueueSize";
        public static string LOGGLY_RETRY_LIMIT = "LogglyLogAppender.RetryLimit";
        public static string LOGGLY_ALL = "LogglyLogAppender.All.Enabled";
        public static string LOGGLY_ERROR = "LogglyLogAppender.Error.Enabled";
        public static string LOGGLY_WARN = "LogglyLogAppender.Warn.Enabled";
        public static string LOGGLY_INFO = "LogglyLogAppender.Info.Enabled";
        public static string LOGGLY_DEBUG = "LogglyLogAppender.Debug.Enabled";
        public static string LOGGLY_VERBOSE = "LogglyLogAppender.Verbose.Enabled";
        public static string LOGGLY_FATAL = "LogglyLogAppender.Fatal.Enabled";
        public static string LOGGLY_TAGS = "LogglyLogAppender.Tags";
        public static string LOGGLY_LOG_RECURSIVELY = "LogglyLogAppender.LogRecursively";

        #endregion

        public static void Setup(IEncompassConfig config, Dictionary<string, string> additionalTags = null)
        {
            LoggerConfiguration baseLogger = null;
            lock (syncRoot)
            {
                try
                {
                    //Serilog has a clever "enricher" approach, but it is created at setup time and wouldn't be easily compatible with how we add tags to logs now.  We could make this a future enhancement.  For now, just add these to each log event when we log.
                    _additionalTags = new ConcurrentDictionary<string, string>();
                    if (!ReferenceEquals(additionalTags, null))
                    {
                        foreach (var kv in additionalTags)
                        {
                            _additionalTags.TryAdd(kv.Key, kv.Value);
                        }
                    }

                    baseLogger = new LoggerConfiguration()
                        .WriteTo.Logger(aa => aa.MinimumLevel.Verbose())
                        .Enrich.FromLogContext()
                        .Enrich.WithExceptionDetails()
                        .Enrich.WithMachineName()
                        .Enrich.WithProcessId()
                        .Enrich.WithEnvironmentUserName()
                        .Enrich.WithProcessName();

                    baseLogger.WriteTo.Logger(aa => aa.Destructure.ToMaximumDepth(20));

                    if (config.GetValue(Logger.ELASTICSEARCH_ENABLED, false))
                    {
                        baseLogger.WriteTo.Elasticsearch(SerilogHelpers.GetElasticOptions(config));
                    }

                    if (config.GetValue(Logger.FILE_ENABLED, false))
                    {
                        baseLogger.WriteTo.RollingFile(pathFormat: config.GetValue(
                            Logger.FILE_FOLDER, config.GetValue(Logger.FILE_NAME)),
                            formatter: new JsonFormatter(null, false, null),
                            fileSizeLimitBytes: config.GetValue(Logger.FILE_MAX_FILE_BYTES, 10000),
                            retainedFileCountLimit: config.GetValue(Logger.FILE_MAX_FILES, 10));
                    }

                    if (config.GetValue(Logger.LOGGLY_ENABLED, false))
                    {
                        Serilog.Events.LogEventLevel eventLevel;
                        eventLevel = SetLogglyEventLevelFromConfig(config);
                        baseLogger.WriteTo.Loggly(logglyConfig: SerilogHelpers.GetLogglyConfig(config), formatProvider: CultureInfo.CurrentCulture,
                                                  restrictedToMinimumLevel: eventLevel);
                    }

                    if (config.GetValue(Logger.CONSOLE_ENABLED, false))
                    {
                        baseLogger.WriteTo.Console(new JsonFormatter(null, false, null));
                    }

                    Serilog.Log.Logger = baseLogger.CreateLogger();

                    configured = true;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"ERROR CONFIGURING LOGGING: {ex}");
                }
            }
        }

        private static LogEventLevel SetLogglyEventLevelFromConfig(IEncompassConfig config)
        {
            if (Convert.ToBoolean(config.GetValue(LOGGLY_ALL)))
                return LogEventLevel.Verbose;
            else if (Convert.ToBoolean(config.GetValue(LOGGLY_DEBUG)))
                return LogEventLevel.Debug;
            else if (Convert.ToBoolean(config.GetValue(LOGGLY_VERBOSE)))
                return LogEventLevel.Verbose;
            else if (Convert.ToBoolean(config.GetValue(LOGGLY_INFO)))
                return LogEventLevel.Information;
            else if (Convert.ToBoolean(config.GetValue(LOGGLY_WARN)))
                return LogEventLevel.Warning;
            else if (Convert.ToBoolean(config.GetValue(LOGGLY_ERROR)))
                return LogEventLevel.Error;
            else if (Convert.ToBoolean(config.GetValue(LOGGLY_FATAL)))
                return LogEventLevel.Fatal;
            else
                return LogEventLevel.Warning;
        }

        private static IDictionary<string, string> PopulateEvent(string loggerName, string message, IDictionary<string, string> fields = null)
        {
            if (fields == null)
            {
                fields = new ConcurrentDictionary<string, string>();
            }

            if (fields.ContainsKey(MESSAGE_KEY) && string.IsNullOrEmpty(message))
            {
                message = fields[MESSAGE_KEY];
            }

            fields.Remove(MESSAGE_KEY);
            fields.Remove(TIMESTAMP_KEY);
            fields.Remove(LOGGERNAME_KEY);

            fields.Add(MESSAGE_KEY, message);
            fields.Add(TIMESTAMP_KEY, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            fields.Add(LOGGERNAME_KEY, loggerName);

            foreach (var tt in _additionalTags)
            {
                if (fields.ContainsKey(tt.Key)) continue;
                fields.Add(tt.Key, tt.Value);
            }

            return fields;
        }

        /// <summary>
        /// writes a log entry as "debug"
        /// </summary>
        /// <param name="logger">e.g. 'my app'</param>
        /// <param name="message">the log message</param>   
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        public static void Debug(string logger, string message, IDictionary<string, string> tags = null)
        {
            if (configured)
            {
                var lv = PrepLogValues(logger, message, tags);
                Serilog.Log.Logger.Debug(lv.Item1, lv.Item2);
            }
        }

        [Obsolete("exlcuded reporters are no longer a thing.")]
        public static void Info(string logger, string message, Type[] excludedReporters)
        {
            Info(logger, message);
        }

        [Obsolete("exlcuded reporters are no longer a thing.")]
        public static void Error(string logger, string message, Type[] excludedReporters)
        {
            Error(logger, message);
        }

        [Obsolete("exlcuded reporters are no longer a thing.")]
        public static void Warn(string logger, string message, Type[] excludedReporters)
        {
            Warn(logger, message);
        }

        [Obsolete("exlcuded reporters are no longer a thing.")]
        public static void Debug(string logger, string message, Type[] excludedReporters)
        {
            Debug(logger, message);
        }

        [Obsolete("exlcuded reporters are no longer a thing.")]
        public static void Fatal(string logger, string message, Type[] excludedReporters)
        {
            Fatal(logger, message);
        }

        /// <summary>
        /// writes a log entry as "error"
        /// </summary>
        /// <param name="logger">e.g. 'my app'</param>
        /// <param name="message">the log message</param>   
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        public static void Error(string logger, string message, IDictionary<string, string> tags = null)
        {
            if (configured)
            {
                var lv = PrepLogValues(logger, message, tags);
                Serilog.Log.Logger.Error(lv.Item1, lv.Item2);
            }

        }

        private static Tuple<string, object[]> PrepLogValues(string logger, string message, IDictionary<string, string> fields = null)
        {
            var data = PopulateEvent(logger, message, fields);
            var sb = new System.Text.StringBuilder("{Message}");
            var log = new List<object>();
            log.Add(message);
            foreach (var d in data)
            {
                sb.Append("{@");
                sb.Append(d.Key);
                sb.Append("}");
                log.Add(d.Value);

            }
            return new Tuple<string, object[]>(sb.ToString(), log.ToArray());
        }

        /// <summary>
        /// Writes a log entry as "fatal"
        /// </summary>
        /// <param name="logger">e.g. 'my app'</param>
        /// <param name="message">the log message</param>   
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        public static void Fatal(string logger, string message, IDictionary<string, string> tags = null)
        {
            if (configured)
            {
                var lv = PrepLogValues(logger, message, tags);
                Serilog.Log.Logger.Fatal(lv.Item1, lv.Item2);
            }
        }

        /// <summary>
        /// Writes a log entry as "info"
        /// </summary>
        /// <param name="logger">e.g. 'my app'</param>
        /// <param name="message">the log message</param>   
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        public static void Info(string logger, string message, IDictionary<string, string> tags = null)
        {
            if (configured)
            {
                var lv = PrepLogValues(logger, message, tags);
                Serilog.Log.Logger.Information(lv.Item1, lv.Item2);
            }
        }

        /// <summary>
        /// Writes a log entry as "warn"
        /// </summary>
        /// <param name="logger">e.g. 'my app'</param>
        /// <param name="message">the log message</param>   
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        public static void Warn(string logger, string message, IDictionary<string, string> tags = null)
        {
            if (configured)
            {
                var lv = PrepLogValues(logger, message, tags);
                Serilog.Log.Logger.Warning(lv.Item1, lv.Item2);
            }
        }

        public static void Log(IDictionary<string, string> fields, string loggerName, string level)
        {
            if (configured)
            {
                var lv = PrepLogValues(loggerName, "", fields);
                switch (level.ToUpperInvariant())
                {
                    case FATAL_LEVEL:
                        Serilog.Log.Logger.Fatal(lv.Item1, lv.Item2);
                        break;
                    case ERROR_LEVEL:
                        Serilog.Log.Logger.Error(lv.Item1, lv.Item2);
                        break;
                    case WARN_LEVEL:
                        Serilog.Log.Logger.Warning(lv.Item1, lv.Item2);
                        break;
                    case INFO_LEVEL:
                        Serilog.Log.Logger.Information(lv.Item1, lv.Item2);
                        break;
                    default:
                        Serilog.Log.Logger.Debug(lv.Item1, lv.Item2);
                        break;
                }
            }
        }

        private bool disposedValue;
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
        /// flushes all logs. 
        /// </summary>
        /// <param name="timeout">wait this many seconds attempting to flush logs.</param>
        /// <param name="throwOnTimeout">throw an exception if we time out flushing logs.</param>
        public static void Shutdown(int timeout = 30, bool throwOnTimeout = false)
        {
            var task = Task.Run(() => Serilog.Log.CloseAndFlush());
            if (task.Wait(TimeSpan.FromSeconds(timeout)))
            {
                return;
            }
            if (throwOnTimeout)
            {
                throw new Exception("Timed out");
            }

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static void AddTag(string key, string value)
        {
            lock (syncRoot)
            {
                if (_additionalTags == null)
                {
                    _additionalTags = new ConcurrentDictionary<string, string>();
                }
                _additionalTags.TryAdd(key, value);
            }
        }

    }
}