using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuaranteedRate.Sextant.Config;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.Loggly;
using Serilog.Sinks.RollingFile;

namespace GuaranteedRate.Sextant.Logging
{
    internal static class SerilogHelpers
    {

        internal static LogEventLevel GetMinLevel(IEncompassConfig config, string prefix)
        {
            string all_key = "";
            string debug_key = "";
            string info_key = "";
            string warn_key = "";
            string error_key = "";
            string fatal_key = "";

            switch (prefix)
            {
                case "FILE":
                    all_key = Logger.FILE_ALL;
                    debug_key = Logger.FILE_DEBUG;
                    info_key = Logger.FILE_INFO;
                    warn_key = Logger.FILE_WARN;
                    error_key = Logger.FILE_ERROR;
                    fatal_key = Logger.FILE_FATAL;
                    break;
                case "ELASTICSEARCH":
                    all_key = Logger.ELASTICSEARCH_ALL;
                    debug_key = Logger.ELASTICSEARCH_DEBUG;
                    info_key = Logger.ELASTICSEARCH_INFO;
                    warn_key = Logger.ELASTICSEARCH_WARN;
                    error_key = Logger.ELASTICSEARCH_ERROR;
                    fatal_key = Logger.ELASTICSEARCH_FATAL;
                    break;
                case "CONSOLE":
                    all_key = Logger.CONSOLE_ALL;
                    debug_key = Logger.CONSOLE_DEBUG;
                    info_key = Logger.CONSOLE_INFO;
                    warn_key = Logger.CONSOLE_WARN;
                    error_key = Logger.CONSOLE_ERROR;
                    fatal_key = Logger.CONSOLE_FATAL;
                    break;
                default:
                    all_key = Logger.LOGGLY_ALL;
                    debug_key = Logger.LOGGLY_DEBUG;
                    info_key = Logger.LOGGLY_INFO;
                    warn_key = Logger.LOGGLY_WARN;
                    error_key = Logger.LOGGLY_ERROR;
                    fatal_key = Logger.CONSOLE_FATAL;
                    break;
            }

            if (config.GetValue(all_key, false))
            {
                return LogEventLevel.Debug;
            }
            if (config.GetValue(debug_key, false))
            {
                return LogEventLevel.Debug;
            }
            if (config.GetValue(info_key, false))
            {
                return LogEventLevel.Information;
            }
            if (config.GetValue(warn_key, false))
            {
                return LogEventLevel.Warning;
            }
            if (config.GetValue(error_key, false))
            {
                return LogEventLevel.Error;
            }
            if (config.GetValue(fatal_key, false))
            {
                return LogEventLevel.Fatal;
            }
            return LogEventLevel.Error;
        }
        

        internal static ElasticsearchSinkOptions GetElasticOptions( IEncompassConfig config)
        {
          
                var elasticOptions = new ElasticsearchSinkOptions(
                    new Uri(config.GetValue(Logger.ELASTICSEARCH_URL, "NOT PROVIDED")));

            var indexFormat = config.GetValue(Logger.ELASTICSEARCH_INDEX_NAME, "sextant-serilog-");
            if (!indexFormat.Contains("{"))
            {
                indexFormat = indexFormat + "{0:yyyy.MM.dd}";
            }

            elasticOptions.IndexFormat = indexFormat;
            elasticOptions.MinimumLogEventLevel = GetMinLevel(config, "ELASTICSEARCH");
                elasticOptions.EmitEventFailure = EmitEventFailureHandling.ThrowException;
            return elasticOptions;


        }

        internal static LogglyConfiguration GetLogglyConfig( IEncompassConfig config)
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

            return lc;
        }

      
    }
}
