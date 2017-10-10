using System;
using System.Collections.Generic;
using System.Linq;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.WebClients;
using Newtonsoft.Json;

namespace GuaranteedRate.Sextant.Metrics.Datadog
{
    public class DatadogReporter : AsyncWebEventReporter, IReporter
    {
        private readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private string _host;
        private IList<string> jsonTags;

        #region config mappings

        public static string DATADOG_ROOTNAMESPACE = "DatadogReporter.RootNamespace";
        public static string DATADOG_ENABLED = "DatadogReporter.Enabled";
        public static string DATADOG_URL = "DatadogReporter.Url";
        public static string DATADOG_APIKEY = "DatadogReporter.ApiKey";
        public static string DATADOG_QUEUE_SIZE = "DatadogReporter.QueueSize";
        public static string DATADOG_RETRY_LIMIT = "DatadogReporter.RetryLimit";
        public static string DATADOG_TAGS = "DatadogReporter.Tags";

        #endregion

        public DatadogReporter(IEncompassConfig config)
            : base(CreateUrl(config.GetValue(DATADOG_URL),config.GetValue(DATADOG_APIKEY)),
                config.GetValue(DATADOG_QUEUE_SIZE, 1000),
                config.GetValue(DATADOG_RETRY_LIMIT, 3))
        {
            Setup();
            var tags = config.GetValue(DATADOG_TAGS, "");
            if (!string.IsNullOrEmpty(tags))
            {
                var trimmedTags = tags.Split(',')
                                      .Select(s => s.Trim())
                                      .Where(s => !string.IsNullOrEmpty(s))
                                      .ToList();

                trimmedTags.ForEach(t => jsonTags.Add(t));
            }
        }

        public DatadogReporter(string endpoint, string apiKey, int queueSize = 1000, int retries = 3)
            :base(CreateUrl(endpoint, apiKey), queueSize, retries)
        {
            Setup();
        }

        protected static string CreateUrl(string url, string apiKey)
        {
            return $"{url}?api_key={apiKey}";
        }

        private void Setup()
        {
            _host = Environment.MachineName;
            jsonTags = new List<string>();
        }

        public void AddTag(string tag, string value)
        {
            string formattedTag = tag + ":" + value;
            jsonTags.Add(formattedTag);
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
            var e = new Event
            {
                metric = metric,
                type = type,
                host = _host,
                tags = jsonTags
            };
            IList<long> point = new List<long>
            {
                GetEpochTime(),
                value
            };

            IList<IList<long>> points = new List<IList<long>>();
            points.Add(point);

            e.points = points;

            var s = new Series {series = new List<Event> {e}};

            ReportEvent(s);
        }

        private class Event
        {
            //Using lowercase to help with json
            public string metric { get; set; }
            public IList<IList<long>> points { get; set; }
            public string type { get; set; }
            public string host { get; set; }
            public IList<string> tags { get; set; }
        }

        private class Series
        {
            public IList<Event> series { get; set; }
        }
    }
}