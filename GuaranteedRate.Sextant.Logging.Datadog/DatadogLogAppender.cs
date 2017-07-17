using System;
using System.Collections.Generic;
using System.Linq;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.WebClients;
using Newtonsoft.Json;

namespace GuaranteedRate.Sextant.Logging.Datadog
{
    public class DatadogLogAppender : AsyncEventReporter, ILogAppender, IDatadogLogAppender
    {
        public bool ErrorEnabled { get; set; }
        public bool WarnEnabled { get; set; }
        public bool InfoEnabled { get; set; }
        public bool DebugEnabled { get; set; }
        public bool FatalEnabled { get; set; }

        public string host { get; set; }

        private ISet<string> _tags;
        private readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #region config mappings

        public static string DATADOG_URL = "DatadogLogAppender.Url";
        public static string DATADOG_QUEUE_SIZE = "DatadogLogAppender.QueueSize";
        public static string DATADOG_RETRY_LIMIT = "DatadogLogAppender.RetryLimit";
        public static string DATADOG_ALL = "DatadogLogAppender.All.Enabled";
        public static string DATADOG_ERROR = "DatadogLogAppender.Error.Enabled";
        public static string DATADOG_WARN = "DatadogLogAppender.Warn.Enabled";
        public static string DATADOG_INFO = "DatadogLogAppender.Info.Enabled";
        public static string DATADOG_DEBUG = "DatadogLogAppender.Debug.Enabled";
        public static string DATADOG_FATAL = "DatadogLogAppender.Fatal.Enabled";
        public static string DATADOG_TAGS = "Logger.Tags";

        #endregion

        public DatadogLogAppender(string baseEndpoint, string apiKey, int queueSize = 1000, int retries = 3) 
            : base(baseEndpoint + "?api_key=" + apiKey, queueSize, retries)
        {
            host = Environment.MachineName;
            _tags = new HashSet<string>();
        }

        public DatadogLogAppender(IEncompassConfig config)
            : base(config.GetValue(DATADOG_URL),
                config.GetValue(DATADOG_QUEUE_SIZE, 1000),
                config.GetValue(DATADOG_RETRY_LIMIT, 3))
        {
            _tags = new HashSet<string>();
            Setup(config);
        }


        public void Setup(IEncompassConfig config)
        {
            var allEnabled = config.GetValue(DATADOG_ALL, false);
            var errorEnabled = config.GetValue(DATADOG_ERROR, false);
            var warnEnabled = config.GetValue(DATADOG_WARN, false);
            var infoEnabled = config.GetValue(DATADOG_INFO, false);
            var debugEnabled = config.GetValue(DATADOG_DEBUG, false);
            var fatalEnabled = config.GetValue(DATADOG_FATAL, false);

            ErrorEnabled = allEnabled || errorEnabled;
            WarnEnabled = allEnabled || warnEnabled;
            InfoEnabled = allEnabled || infoEnabled;
            DebugEnabled = allEnabled || debugEnabled;
            FatalEnabled = allEnabled || fatalEnabled;

            if (_tags == null) _tags = new HashSet<string>();

            var configTags = config.GetValue(DATADOG_TAGS, string.Empty);

            if (!string.IsNullOrEmpty(configTags))
            {
                foreach (var tag in configTags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var tagParts = tag.Split(new char[] { ':'}, 2);
                    if (tagParts.Length == 2)
                    {
                        AddTag(tagParts[0], tagParts[1]);
                    }
                }
            }
        }

        protected override bool PostEvent(object data)
        {
            try
            {
                Client.Post("", data);
            }
            catch (ApiException ex)
            {
                var error =
                    $"The following request returned a {ex.StackTrace} status code, resource endpoint: {Client.BaseAddress} model: {ex.Message}. Response {ex.Response}";
                Logger.Warn(Name, error);
                return false;
            }

            return true;
        }

        public void Log(IDictionary<string, string> fields)
        {
            throw new NotImplementedException();
        }

        public void AddTag(string tag, string value)
        {
            string formattedTag = tag + ":" + value;
            _tags.Add(formattedTag);
        }

        private long GetEpochTime()
        {
            return Convert.ToInt64((DateTime.Now.ToUniversalTime() - epoch).TotalSeconds);
        }

        public void AddCounter(string metric, long value)
        {
            AddMetric(metric, value, "counter");
        }


        /// <summary>
        /// Send a metric to DataDog
        /// </summary>
        /// <param name="metric">name of metric e.g. HogsHeadsPerMile</param>
        /// <param name="value">value to send to DataDog  e.g. 67</param>
        public void AddGauge(string metric, long value)
        {
            AddMetric(metric, value, "gauge");
        }

        /// <summary>
        /// deprecataed because misspelled.  Use AddGauge instead.
        /// </summary>
        /// <param name="metric">name of metric e.g. HogsHeadsPerMile</param>
        /// <param name="value">value to send to DataDog  e.g. 67</param>
        [Obsolete("Use AddGauge instead.")]
        public void AddGuage(string metric, long value)
        {
            AddGauge(metric, value);
        }

        /// <summary>
        /// Send a meter to DataDog
        /// </summary>
        /// <param name="metric">name of metric e.g. HogsHeadsPerMile</param>
        /// <param name="value">value to send to DataDog  e.g. 67</param>
        public void AddMeter(string metric, long value)
        {
            AddMetric(metric, value, "meter");
        }

        private void AddMetric(string metric, long value, string type)
        {
            Event e = new Event
            {
                metric = metric,
                type = type,
                host = host,
                tags = _tags
            };
            IList<long> point = new List<long>();
            point.Add(GetEpochTime());
            point.Add(value);
            IList<IList<long>> points = new List<IList<long>>();
            points.Add(point);

            e.points = points;

            Series s = new Series { series = new List<Event> { e } };

            string json = JsonConvert.SerializeObject(s);
            ReportEvent(json);
        }
    }
}
