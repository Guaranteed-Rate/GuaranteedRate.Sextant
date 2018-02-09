using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuaranteedRate.Sextant.Config;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.Loggly;

namespace GuaranteedRate.Sextant.Logging
{
    internal static class SerilogHelpers
    {

        internal static LogEventLevel GetMinLevel(IEncompassConfig config, string prefix)
        {
            if (config.GetValue($"{prefix}_ALL", false))
            {
                return LogEventLevel.Debug;
            }
            if (config.GetValue($"{prefix}_DEBUG", false))
            {
                return LogEventLevel.Debug;
            }
            if (config.GetValue($"{prefix}_INFO", false))
            {
                return LogEventLevel.Information;
            }
            if (config.GetValue($"{prefix}_WARN", false))
            {
                return LogEventLevel.Warning;
            }
            if (config.GetValue($"{prefix}_ERROR", false))
            {
                return LogEventLevel.Error;
            }
            if (config.GetValue($"{prefix}_FATAL", false))
            {
                return LogEventLevel.Fatal;
            }
            return LogEventLevel.Error;
        }


        internal static LoggerConfiguration ConfigureFile(this LoggerConfiguration serilog,IEncompassConfig config )
        {
            if (config.GetValue(Logger.FILE_ENABLED, false))
            {
                return serilog.WriteTo.RollingFile(
                    new JsonFormatter(null, false, null), config.GetValue(Logger.LOG_FOLDER, "c:\\windows\\temp"),
                    GetMinLevel(config, "FILE"), config.GetValue(Logger.FILE_MAX_FILE_BYTES, 10000),
                    config.GetValue(Logger.FILE_MAX_FILES, 10));
            }
            return serilog;
        }

        internal static LoggerConfiguration ConfigureElasticSearch(this LoggerConfiguration serilog, IEncompassConfig config)
        {
            if (config.GetValue(Logger.ELASTICSEARCH_ENABLED, false))
            {
                var elasticOptions = new ElasticsearchSinkOptions(
                    new Uri(config.GetValue(Logger.ELASTICSEARCH_URL, "NOT PROVIDED")));
                elasticOptions.IndexFormat = config.GetValue(Logger.ELASTICSEARCH_INDEX_NAME, "sextant-serilog-");

                elasticOptions.MinimumLogEventLevel = GetMinLevel(config, "ELASTICSEARCH");
                elasticOptions.EmitEventFailure = EmitEventFailureHandling.ThrowException;
                return serilog.WriteTo.Elasticsearch(elasticOptions);
            }
            return serilog;

        }

        internal static LoggerConfiguration ConfigureLoggly(this LoggerConfiguration serilog, IEncompassConfig config)
        {
            if (config.GetValue(Logger.LOGGLY_ENABLED, false))
            {
                var lc = new LogglyConfiguration();
                lc.CustomerToken = config.GetValue(Logger.LOGGLY_APIKEY);
                lc.EndpointHostName = config.GetValue(Logger.LOGGLY_URL, "logs-01.loggly.com").Replace("https://", "");
                lc.EndpointPort = 443;
                lc.ThrowExceptions = true;
                lc.ApplicationName = config.GetValue(Logger.LOGGLY_APPLICATION_NAME, "Unnamed Sextant App");
                var split = new[] { '|', ',' };
                lc.Tags = new List<string>();
                lc.Tags.AddRange(config.GetValue(Logger.LOGGLY_TAGS, "")
                    .Split(split, StringSplitOptions.RemoveEmptyEntries));
                return serilog.WriteTo.Loggly(
                    logglyConfig: lc,
                    restrictedToMinimumLevel: GetMinLevel(config, "LOGGLY"));
            }
            return serilog;

        }

        internal static LoggerConfiguration ConfigureConsole(this LoggerConfiguration serilog, IEncompassConfig config)
        {

            if (config.GetValue(Logger.CONSOLE_ENABLED, false))
            {
                return serilog.WriteTo.Console(
                     new JsonFormatter(null, false, null),
                     GetMinLevel(config, "CONSOLE"));
            }
            return serilog;
        }

    }
}
