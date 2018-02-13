using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using GuaranteedRate.Sextant.Config;
using Serilog;
using Serilog.Formatting.Json;

namespace GuaranteedRate.Sextant.Logging
{
    public class Logger : IDisposable
    {
        private static readonly object syncRoot = new Object();
        private static bool configured = false;
        public const string LEVEL = "level";
        public const string ERROR_LEVEL = "ERROR";
        public const string WARN_LEVEL = "WARN";
        public const string INFO_LEVEL = "INFO";
        public const string DEBUG_LEVEL = "DEBUG";
        public const string FATAL_LEVEL = "FATAL";
        private static Dictionary<string, string> _additionalTags;

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
        public static string LOGGLY_FATAL = "LogglyLogAppender.Fatal.Enabled";
        public static string LOGGLY_TAGS = "LogglyLogAppender.Tags";
        public static string LOGGLY_LOG_RECURSIVELY = "LogglyLogAppender.LogRecursively";

        #endregion


        public static void Setup(IEncompassConfig config)
        {
            LoggerConfiguration baseLogger = null;
            lock (syncRoot)
            {
                try
                {
                    //Serilog has a clever "enricher" approach, but it is created at setup time and wouldn't be easily compatible with how we add tags to logs now.  We could make this a future enhancement.  For now, just add these to each log event when we log.
                    _additionalTags = new Dictionary<string, string>();
                    _additionalTags.Add("process", Process.GetCurrentProcess().ProcessName);
                    _additionalTags.Add("hostname", Environment.MachineName);
                    _additionalTags.Add("windowsuser", Environment.UserName);

                    baseLogger = new LoggerConfiguration()
                        .WriteTo.Logger(aa => aa.MinimumLevel.Verbose());

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
                        baseLogger.WriteTo.Loggly(logglyConfig: SerilogHelpers.GetLogglyConfig(config));
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
                    System.Console.WriteLine($"ERROR CONFIGURING LOGGING:{ex}");
                }
            }
        }

        private static IDictionary<string, string> PopulateEvent(string loggerName, string message)
        {
            IDictionary<string, string> fields = new ConcurrentDictionary<string, string>();
            fields.Add("timestamp", DateTime.UtcNow.ToString());
            fields.Add("loggerName", loggerName);

            foreach (var tt in _additionalTags)
            {
                fields.Add(tt.Key, tt.Value);
            }

            fields.Add("message", message);
            return fields;
        }

        public static void Debug(string logger, string message)
        {
            if (configured)
            {
                Serilog.Log.Logger.Debug($"{logger}: {message}", PopulateEvent(logger, message));
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
        /// <param name="excludedReporters">don't have these reporters process  this message.  useful so an error in a reporter doesn't log to itself and fail recursively.</param>
        public static void Error(string logger, string message)
        {
            if (configured)
            {
                Serilog.Log.Logger.Error($"{logger}: {message}", PopulateEvent(logger, message));
            }
        }

        /// <summary>
        /// Writes a log entry as "fatal"
        /// </summary>
        /// <param name="logger">e.g. 'my app'</param>
        /// <param name="message">the log message</param>   
        /// <param name="excludedReporters">don't have these reporters process  this message.  useful so an error in a reporter doesn't log to itself and fail recursively.</param>
        public static void Fatal(string logger, string message)
        {
            if (configured)
            {
                Serilog.Log.Logger.Fatal($"{logger}: {message}", PopulateEvent(logger, message));
            }
        }

        /// <summary>
        /// Writes a log entry as "info"
        /// </summary>
        /// <param name="logger">e.g. 'my app'</param>
        /// <param name="message">the log message</param>   
        /// <param name="excludedReporters">don't have these reporters process  this message.  useful so an error in a reporter doesn't log to itself and fail recursively.</param>
        public static void Info(string logger, string message)
        {
            if (configured)
            {

                Serilog.Log.Logger.Information($"{logger}: {message}", PopulateEvent(logger, message));
            }
        }

        /// <summary>
        /// Writes a log entry as "warn"
        /// </summary>
        /// <param name="logger">e.g. 'my app'</param>
        /// <param name="message">the log message</param>   
        /// <param name="excludedReporters">don't have these reporters process  this message.  useful so an error in a reporter doesn't log to itself and fail recursively.</param>
        public static void Warn(string logger, string message)
        {
            if (configured)
            {
                Serilog.Log.Logger.Warning($"{logger}: {message}", PopulateEvent(logger, message));
            }
        }

        public static void Log(IDictionary<string, string> fields, string loggerName, string level)
        {
            switch (level.ToLowerInvariant())
            {
                case "fatal":
                    Serilog.Log.Logger.Fatal($"{loggerName}: {fields}", fields);
                    break;
                case "error":
                    Serilog.Log.Logger.Error($"{loggerName}: {fields}", fields);
                    break;
                case "warn":
                    Serilog.Log.Logger.Warning($"{loggerName}: {fields}", fields);
                    break;
                case "info":
                    Serilog.Log.Logger.Information($"{loggerName}: {fields}", fields);
                    break;
                default:
                    Serilog.Log.Logger.Debug($"{loggerName}: {fields}", fields);
                    break;
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

        public void AddTag(string key, string value)
        {
            lock (syncRoot)
            {
                if (_additionalTags == null)
                {
                    _additionalTags = new Dictionary<string, string>();
                }
                _additionalTags.Add(key, value);
            }
        }

    }
}
