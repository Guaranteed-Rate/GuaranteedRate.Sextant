using System;
using System.Collections.Generic;
using System.Linq;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.WebClients;
using Nest;

namespace GuaranteedRate.Sextant.Logging.Elasticsearch
{
    public class ElasticsearchLogAppender : AsyncEventReporter, ILogAppender
    {
        protected override string Name { get; } = typeof(ElasticsearchLogAppender).Name;
        private Uri _node;
        private ConnectionSettings _settings;
        private ElasticClient _client;
        public bool DebugEnabled { get; private set; }
        public bool InfoEnabled { get; private set; }
        public bool WarnEnabled { get; private set; }
        public bool ErrorEnabled { get; private set; }
        public bool FatalEnabled { get; private set; }

        #region config mappings

        public static string ELASTICSEARCH_URL = "ElasticsearchLogAppender.Url";
        public static string ELASTICSEARCH_QUEUE_SIZE = "ElasticsearchLogAppender.QueueSize";
        public static string ELASTICSEARCH_RETRY_LIMIT = "ElasticsearchLogAppender.RetryLimit";
        public static string ELASTICSEARCH_ALL = "ElasticsearchLogAppender.All.Enabled";
        public static string ELASTICSEARCH_ERROR = "ElasticsearchLogAppender.Error.Enabled";
        public static string ELASTICSEARCH_WARN = "ElasticsearchLogAppender.Warn.Enabled";
        public static string ELASTICSEARCH_INFO = "ElasticsearchLogAppender.Info.Enabled";
        public static string ELASTICSEARCH_DEBUG = "ElasticsearchLogAppender.Debug.Enabled";
        public static string ELASTICSEARCH_FATAL = "ElasticsearchLogAppender.Fatal.Enabled";

        #endregion

        public ElasticsearchLogAppender(IEncompassConfig config)
            : base(config.GetValue(ELASTICSEARCH_URL),
                config.GetValue(ELASTICSEARCH_QUEUE_SIZE, 1000),
                config.GetValue(ELASTICSEARCH_RETRY_LIMIT, 3))
        {
            Setup(config);
        }

        public void Setup(IEncompassConfig config)
        {
            var url = config.GetValue(ELASTICSEARCH_URL);
            _node = new Uri(url);
            _settings = new ConnectionSettings(_node);
            _client = new ElasticClient(_settings);

            var allEnabled = config.GetValue(ELASTICSEARCH_ALL, true);
            var errorEnabled = config.GetValue(ELASTICSEARCH_ERROR, true);
            var warnEnabled = config.GetValue(ELASTICSEARCH_WARN, true);
            var infoEnabled = config.GetValue(ELASTICSEARCH_INFO, true);
            var debugEnabled = config.GetValue(ELASTICSEARCH_DEBUG, true);
            var fatalEnabled = config.GetValue(ELASTICSEARCH_FATAL, true);

            ErrorEnabled = allEnabled || errorEnabled;
            WarnEnabled = allEnabled || warnEnabled;
            InfoEnabled = allEnabled || infoEnabled;
            DebugEnabled = allEnabled || debugEnabled;
            FatalEnabled = allEnabled || fatalEnabled;
        }

        public void Log(IDictionary<string, string> fields)
        {
            ReportEvent(fields);
        }

        protected override bool PostEvent(object data)
        {
            var fields = data as IDictionary<string, string>;

            if (fields == null) return true;

            var loggerName = "undefined";
            if (fields.ContainsKey("logger"))
            {
                loggerName = fields["logger"];
            }

            try
            {
                _client.Index(fields, idx => idx.Index($"{loggerName}-{DateTime.UtcNow.ToString("yyyy-MM-dd")}"));
            }
            catch (Exception ex)
            {
                Logger.Warn(Name, $"The Elasticsearch request returned an error {ex.Message}");
                return false;
            }

            return true;
        }
    }
}
