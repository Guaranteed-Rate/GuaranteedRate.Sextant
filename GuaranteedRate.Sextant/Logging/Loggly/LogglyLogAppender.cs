using System;
using System.Collections.Generic;
using System.Text;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.WebClients;
using Newtonsoft.Json;

namespace GuaranteedRate.Sextant.Logging.Loggly
{
    public class LogglyLogAppender : AsyncEventReporter, ILogAppender
    {
        protected override string Name { get; } = typeof(LogglyLogAppender).Name;
        private static ISet<string> _tags;
        public bool ErrorEnabled { get; set; }
        public bool WarnEnabled { get; set; }
        public bool InfoEnabled { get; set; }
        public bool DebugEnabled { get; set; }
        public bool FatalEnabled { get; set; }

        #region config mappings

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
            _tags = new HashSet<string>();
        }

        public LogglyLogAppender(IEncompassConfig config)
            : base(CreateUrl(config.GetValue(LOGGLY_URL), config.GetValue(LOGGLY_APIKEY)),
                config.GetValue(LOGGLY_QUEUE_SIZE, 1000),
                config.GetValue(LOGGLY_RETRY_LIMIT, 3))
        {
            _tags = new HashSet<string>();
            Setup(config);
        }

        protected static string CreateUrl(string url, string apiKey)
        {
            return $"{url}/inputs/{apiKey}/tag";
        }

        protected void Setup(IEncompassConfig config)
        {
            CreateClient(config.GetValue(LOGGLY_URL));
            
            var allEnabled = config.GetValue(LOGGLY_ALL, false);
            var errorEnabled = config.GetValue(LOGGLY_ERROR, false);
            var warnEnabled = config.GetValue(LOGGLY_WARN, false);
            var infoEnabled = config.GetValue(LOGGLY_INFO, false);
            var debugEnabled = config.GetValue(LOGGLY_DEBUG, false);
            var fatalEnabled = config.GetValue(LOGGLY_FATAL, false);

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
            }
        }

        public void Log(IDictionary<string, string> fields)
        {
            //Having Indented formatting makes the data format better in the Loggly
            //Search screen
            var json = JsonConvert.SerializeObject(fields, Formatting.Indented);
            ReportEvent(json);
        }

        private static string MakeTagCsv()
        {
            StringBuilder builder = new StringBuilder();
            foreach (string tag in _tags)
            {
                if (!string.IsNullOrWhiteSpace(tag))
                {
                    builder.Append(tag).Append(",");
                }
            }
            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }
    }
}
