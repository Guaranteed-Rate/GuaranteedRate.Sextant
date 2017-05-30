using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.Logging;
using GuaranteedRate.Sextant.WebClients;
using Newtonsoft.Json;

namespace GuaranteedRate.Sextant
{
    public class LogglyLogAppender: AsyncEventReporter, ILogAppender
    {
        private static ISet<string> tags = new HashSet<string>();
        private static int QUEUE_SIZE = DEFAULT_QUEUE_SIZE;
        private static string POST_URL;
        private static readonly int DEFAULT_QUEUE_SIZE = 1000;
        public bool ErrorEnabled { get; set; }
        public bool WarnEnabled { get; set; }
        public bool InfoEnabled { get; set; }
        public bool DebugEnabled { get; set; }
        public bool FatalEnabled { get; set; }
        
        /// <summary>
        /// Tags must be added BEFORE anything is logged.
        /// Once the first event is logged, the tags are locked
        /// </summary>
        /// <param name="tag"></param>
        public void AddTag(string tag)
        {
            if (!tags.Contains(tag.Trim()))
            {
                tags.Add(tag.Trim());
            }
        }

        public void SetSize(int queueSize)
        {
            QUEUE_SIZE = queueSize;
        }

        public static void SetPostUrl(string url)
        {
            POST_URL = url;
        }

        public void Setup(IEncompassConfig config, ICollection<string> tags = null)
        {
            Setup(config);
            if (tags != null)
            {
                foreach (string tag in tags)
                {
                    AddTag(tag);
                }
            }

            foreach (var tag in config.GetValue(LOGGLY_TAGS, string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                AddTag(tag);
            }


        }

        /// <summary>
        /// Initializes the loggly log reporter
        /// </summary>
        /// <param name="config">Config file to load.  Will re-initialize when this is used.</param>
        public void Setup(IEncompassConfig config)
        {
            POST_URL = config.GetValue(LOGGLY_URL);

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
        }

        #region config mappings
        public static string LOGGLY_URL = "LogglyLogAppender.Url";
        public static string LOGGLY_ALL = "LogglyLogAppender.All.Enabled";
        public static string LOGGLY_ERROR = "LogglyLogAppender.Error.Enabled";
        public static string LOGGLY_WARN = "LogglyLogAppender.Warn.Enabled";
        public static string LOGGLY_INFO = "LogglyLogAppender.Info.Enabled";
        public static string LOGGLY_DEBUG = "LogglyLogAppender.Debug.Enabled";
        public static string LOGGLY_FATAL = "LogglyLogAppender.Fatal.Enabled";
        public static string LOGGLY_TAGS = "Logger.Tags";
        #endregion

        public LogglyLogAppender(string url, int queueSize = 1000, int retries = 3) : base(url, queueSize, retries)
        {
            POST_URL = url;
            QUEUE_SIZE = queueSize;

        }

        public void Log(IDictionary<string, string> fields)
        {
            //Having Indented formatting makes the data format better in the Loggly
            //Search screen
            string json = JsonConvert.SerializeObject(fields, Formatting.Indented);
            ReportEvent(json);
        }


        private static string MakeTagCsv()
        {
            StringBuilder builder = new StringBuilder();
            foreach (string tag in tags)
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
