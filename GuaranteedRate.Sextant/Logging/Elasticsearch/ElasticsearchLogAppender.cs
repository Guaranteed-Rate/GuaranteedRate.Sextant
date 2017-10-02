using System;
using System.Collections.Generic;
using System.Linq;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.WebClients;
using Nest;
using Newtonsoft.Json;

namespace GuaranteedRate.Sextant.Logging.Elasticsearch
{
    public class ElasticsearchLogAppender : AsyncEventReporter, ILogAppender
    {
        //private readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        protected override string Name { get; } = typeof(ElasticsearchLogAppender).Name;
        private Uri _node;
        private ConnectionSettings _settings;
        private string _indexName;
        private ElasticClient _client;
        private static ISet<string> _tags;
        public bool AllEnabled { get; set; }
        public bool DebugEnabled { get; private set; }
        public bool InfoEnabled { get; private set; }
        public bool WarnEnabled { get; private set; }
        public bool ErrorEnabled { get; private set; }
        public bool FatalEnabled { get; private set; }

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
        public static string ELASTICSEARCH_FATAL = "ElasticsearchLogAppender.Fatal.Enabled";
        public static string ELASTICSEARCH_TAGS = "ElasticsearchLogAppender.Tags";
        public static string ELASTICSEARCH_INDEX_NAME = "ElasticsearchLogAppender.IndexName";

        #endregion

        public ElasticsearchLogAppender(IEncompassConfig config)
            : base(config.GetValue(ELASTICSEARCH_QUEUE_SIZE, 1000),
                config.GetValue(ELASTICSEARCH_RETRY_LIMIT, 3))
        {
            _tags = new HashSet<string>();
            Setup(config);
        }

        protected void Setup(IEncompassConfig config)
        {
            var url = config.GetValue(ELASTICSEARCH_URL);
            _node = new Uri(url);
            _settings = new ConnectionSettings(_node);
            _client = new ElasticClient(_settings);
            _indexName = config.GetValue(ELASTICSEARCH_INDEX_NAME, "SextantLogger").ToLower();
            var allEnabled = config.GetValue(ELASTICSEARCH_ALL, true);
            var errorEnabled = config.GetValue(ELASTICSEARCH_ERROR, true);
            var warnEnabled = config.GetValue(ELASTICSEARCH_WARN, true);
            var infoEnabled = config.GetValue(ELASTICSEARCH_INFO, true);
            var debugEnabled = config.GetValue(ELASTICSEARCH_DEBUG, true);
            var fatalEnabled = config.GetValue(ELASTICSEARCH_FATAL, true);

            AllEnabled = allEnabled;
            ErrorEnabled = allEnabled || errorEnabled;
            WarnEnabled = allEnabled || warnEnabled;
            InfoEnabled = allEnabled || infoEnabled;
            DebugEnabled = allEnabled || debugEnabled;
            FatalEnabled = allEnabled || fatalEnabled;

            if (_tags == null) _tags = new HashSet<string>();

            var configTags = config.GetValue(ELASTICSEARCH_TAGS, string.Empty);

            if (!string.IsNullOrEmpty(configTags))
            {
                foreach (var tag in configTags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    AddTag(tag);
                }
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

        protected override bool PostEvent(object data)
        {
            var fields = data as IDictionary<string, string>;

            if (fields == null) return true;

            var loggerName = "undefined";
            if (fields.ContainsKey("loggerName"))
            {
                loggerName = fields["loggerName"];
                loggerName = loggerName.ToLower();
            }

            //fields["timestamp"] = GetEpochTime().ToString();

            fields["tags"] = JsonConvert.SerializeObject(_tags);

            var logEvent = new ElasticLogEvent
            {
                loggerName = fields["loggerName"],
                hostname = fields.ContainsKey("hostname") ? fields["hostname"] : System.Environment.MachineName,
                timestamp = fields.ContainsKey("timestamp") ? DateTime.Parse(fields["timestamp"]) :DateTime.UtcNow,
                level = fields.ContainsKey(Logger.LEVEL) ? fields[Logger.LEVEL] : "INFO",
                message = fields.ContainsKey("message") ? fields["message"] : "no message",
                process = fields.ContainsKey("process") ? fields["process"] : "process not defined"
            };

            try
            {
                var response = _client.Index(logEvent, idx => idx.Index($"{_indexName}-{DateTime.UtcNow.ToString("yyyy.MM.dd")}"));

                if (!response.IsValid)
                {
                    Logger.Warn(Name, $"The Elasticsearch request returned an error {response.DebugInformation}");
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(Name, $"The Elasticsearch request returned an error {ex.Message}");
                return false;
            }

            return true;
        }

        public void AddTag(string tag)
        {
            if (!_tags.Contains(tag.Trim()))
            {
                _tags.Add(tag.Trim());
            }
        }

        //private long GetEpochTime()
        //{
        //    return Convert.ToInt64((DateTime.Now.ToUniversalTime() - epoch).TotalSeconds);
        //}
    }
}
