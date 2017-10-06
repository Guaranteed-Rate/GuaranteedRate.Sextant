using System;
using System.Collections.Generic;
using System.Linq;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.WebClients;

namespace GuaranteedRate.Sextant.Logging.Loggly
{
    public class LogglyLogAppender : AsyncWebEventReporter, ILogAppender
    {
        private readonly string _url = "";
        private readonly string _apiKey = "";

        protected override string Name { get; } = typeof(LogglyLogAppender).Name;
        private static ISet<string> _tags;
        public bool AllEnabled { get; set; }
        public bool ErrorEnabled { get; set; }
        public bool WarnEnabled { get; set; }
        public bool InfoEnabled { get; set; }
        public bool DebugEnabled { get; set; }
        public bool FatalEnabled { get; set; }

        #region config mappings

        public static string LOGGLY_ENABLED = "LogglyLogAppender.Enabled";
        public static string LOGGLY_URL = "LogglyLogAppender.Url";
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

        #endregion

        public LogglyLogAppender(string url, string apiKey, int queueSize = 1000, int retries = 3) : base(CreateUrl(url, apiKey), queueSize, retries)
        {
            _url = url;
            _apiKey = apiKey;
            _tags = new HashSet<string>();
        }

        public LogglyLogAppender(IEncompassConfig config)
            : base(CreateUrl(config.GetValue(LOGGLY_URL), config.GetValue(LOGGLY_APIKEY)),
                config.GetValue(LOGGLY_QUEUE_SIZE, 1000),
                config.GetValue(LOGGLY_RETRY_LIMIT, 3))
        {
            _url = config.GetValue(LOGGLY_URL);
            _apiKey = config.GetValue(LOGGLY_APIKEY);
            _tags = new HashSet<string>();
            Setup(config);
        }

        protected static string CreateUrl(string url, string apiKey)
        {
            var baseUrl = $"{url}/inputs/{apiKey}/tag";

            if (_tags != null && _tags.Any())
            {
                var tagCsv = string.Join(",", _tags);
                baseUrl += $"/{tagCsv}/";
            }

            return baseUrl;
        }

        protected void Setup(IEncompassConfig config)
        {
            var allEnabled = config.GetValue(LOGGLY_ALL, false);
            var errorEnabled = config.GetValue(LOGGLY_ERROR, false);
            var warnEnabled = config.GetValue(LOGGLY_WARN, false);
            var infoEnabled = config.GetValue(LOGGLY_INFO, false);
            var debugEnabled = config.GetValue(LOGGLY_DEBUG, false);
            var fatalEnabled = config.GetValue(LOGGLY_FATAL, false);

            AllEnabled = allEnabled;
            ErrorEnabled = allEnabled || errorEnabled;
            WarnEnabled = allEnabled || warnEnabled;
            InfoEnabled = allEnabled || infoEnabled;
            DebugEnabled = allEnabled || debugEnabled;
            FatalEnabled = allEnabled || fatalEnabled;

            if(_tags == null) _tags = new HashSet<string>();

            var configTags = config.GetValue(LOGGLY_TAGS, string.Empty);

            if (!string.IsNullOrEmpty(configTags))
            {
                foreach (var tag in configTags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    AddTag(tag);
                }
            }

            var url = CreateUrl(config.GetValue(LOGGLY_URL), config.GetValue(LOGGLY_APIKEY));

            CreateClient(url);
        }

        /// <summary>
        /// Tags must be added BEFORE anything is logged.
        /// Once the first event is logged, the tags are locked
        /// </summary>
        /// <param name="tag"></param>
        public void AddTag(string tag)
        {
            if (!_tags.Contains(tag.Trim()))
            {
                _tags.Add(tag.Trim());

                var url = CreateUrl(_url, _apiKey);
                CreateClient(url);
            }
        }

        public void Log(IDictionary<string, string> fields)
        {
            if (AllEnabled)
            {
                ReportEvent(fields);
            }
            else if (DebugEnabled && string.Equals(fields[Logger.LEVEL], Logger.DEBUG_LEVEL, StringComparison.CurrentCultureIgnoreCase))
            {
                ReportEvent(fields);
            }
            else if (InfoEnabled && string.Equals(fields[Logger.LEVEL], Logger.INFO_LEVEL, StringComparison.CurrentCultureIgnoreCase))
            {
                ReportEvent(fields);
            }
            else if (WarnEnabled && string.Equals(fields[Logger.LEVEL], Logger.WARN_LEVEL, StringComparison.CurrentCultureIgnoreCase))
            {
                ReportEvent(fields);
            }
            else if (ErrorEnabled && string.Equals(fields[Logger.LEVEL], Logger.ERROR_LEVEL, StringComparison.CurrentCultureIgnoreCase))
            {
                ReportEvent(fields);
            }
            else if (FatalEnabled && string.Equals(fields[Logger.LEVEL], Logger.FATAL_LEVEL, StringComparison.CurrentCultureIgnoreCase))
            {
                ReportEvent(fields);
            }
        }
    }
}
