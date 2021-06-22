using GuaranteedRate.Sextant.Config;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using System;
using System.Globalization;

namespace GuaranteedRate.Sextant.Logging
{
    /// <summary>
    /// This is a static Setup class for a Serilog Logger. It requires all using DLLs start referencing Serilog. 
    /// We could write a wrapper around Serilog, but this isn't exactly a great project when we have a solid logging system to just make use of.
    /// </summary>
    public static class SeriLogger
	{
		private static readonly object syncRoot = new object();
        private static bool configured = false;

        public static Serilog.ILogger Logger => _logger.Value;

        /// <summary>
        /// The behavior of Lazy is to return null each time afterwards if an exception was thrown.
        /// Otherwise, it will keep returning the same Log.Logger. This ensures we have a quick thread-safe access that also ensures we get no overhead from more than one Exception check.
        /// </summary>
        private static Lazy<Serilog.ILogger> _logger = new Lazy<Serilog.ILogger>(SafeLogGet, true);

        private static Serilog.ILogger SafeLogGet()
        {
            if (configured) return Log.Logger;
            throw new Exception("SeriLogger is not configured. Please fix the code.");
        }

        public static Serilog.ILogger Setup(IEncompassConfig config)
		{
            lock (syncRoot)
            {
                if (configured) throw new Exception("Already Setup for this AppDomain.");
                
                try
                {
                    var baseLogger = new LoggerConfiguration()
                        .WriteTo.Logger(aa => aa.MinimumLevel.Verbose())
                        .Enrich.FromLogContext()
                        .Enrich.WithExceptionDetails()
                        .Enrich.WithMachineName()
                        .Enrich.WithProcessId()
                        .Enrich.WithEnvironmentUserName()
                        .Enrich.WithProcessName()
                        .Enrich.WithAssemblyName()
                        .Enrich.WithAssemblyVersion();

                    baseLogger.WriteTo.Logger(aa => aa.Destructure.ToMaximumDepth(20));

                    if (config.GetValue(Logging.Logger.ELASTICSEARCH_ENABLED, false))
                    {
                        baseLogger.WriteTo.Elasticsearch(SerilogHelpers.GetElasticOptions(config));
                    }

                    if (config.GetValue(Logging.Logger.FILE_ENABLED, false))
                    {
                        baseLogger.WriteTo.RollingFile(pathFormat: config.GetValue(
                            Logging.Logger.FILE_FOLDER, config.GetValue(Logging.Logger.FILE_NAME)),
                            formatter: new JsonFormatter(renderMessage: false),
                            fileSizeLimitBytes: config.GetValue(Logging.Logger.FILE_MAX_FILE_BYTES, 10000),
                            retainedFileCountLimit: config.GetValue(Logging.Logger.FILE_MAX_FILES, 10));
                    }

                    if (config.GetValue(Logging.Logger.LOGGLY_ENABLED, false))
                    {
                        LogEventLevel eventLevel = SetLogglyEventLevelFromConfig(config);
                        baseLogger.WriteTo.Loggly(logglyConfig: SerilogHelpers.GetLogglyConfig(config), formatProvider: CultureInfo.CurrentCulture,
                                                  restrictedToMinimumLevel: eventLevel);
                    }

                    if (config.GetValue(Logging.Logger.CONSOLE_ENABLED, false))
                    {
                        baseLogger.WriteTo.Console(new JsonFormatter(null, false, null));
                    }

                    Log.Logger = baseLogger.CreateLogger();

                    configured = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR CONFIGURING LOGGING: {ex}");
                }
            }

            return Serilog.Log.Logger;
        }

        private static LogEventLevel SetLogglyEventLevelFromConfig(IEncompassConfig config)
        {
            if (Convert.ToBoolean(config.GetValue(Logging.Logger.LOGGLY_ALL)))
                return LogEventLevel.Verbose;
            else if (Convert.ToBoolean(config.GetValue(Logging.Logger.LOGGLY_DEBUG)))
                return LogEventLevel.Debug;
            else if (Convert.ToBoolean(config.GetValue(Logging.Logger.LOGGLY_VERBOSE)))
                return LogEventLevel.Verbose;
            else if (Convert.ToBoolean(config.GetValue(Logging.Logger.LOGGLY_INFO)))
                return LogEventLevel.Information;
            else if (Convert.ToBoolean(config.GetValue(Logging.Logger.LOGGLY_WARN)))
                return LogEventLevel.Warning;
            else if (Convert.ToBoolean(config.GetValue(Logging.Logger.LOGGLY_ERROR)))
                return LogEventLevel.Error;
            else if (Convert.ToBoolean(config.GetValue(Logging.Logger.LOGGLY_FATAL)))
                return LogEventLevel.Fatal;
            else
                return LogEventLevel.Warning;
        }
    }
}
