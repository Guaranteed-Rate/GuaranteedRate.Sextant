using GuaranteedRate.Sextant.WebClients;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.Metrics
{
    public class DatadogReporter : IDataDogReporter
    {

        private readonly IEventReporter datadogPoster;
        public string host { get; set; }

        private IList<string> jsonTags;
        private readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static volatile DatadogReporter INSTANCE;
        private static object syncRoot = new Object();

        private static int QUEUE_SIZE = -1;
        private static string API_KEY;
        private static string ENDPOINT;

        public static void SetQueueSize(int queueSize)
        {
            QUEUE_SIZE = queueSize;
        }

        public static void SetApiKey(string apiKey)
        {
            API_KEY = apiKey;
        }

        public static void SetEndPoint(string endPoint)
        {
            ENDPOINT = endPoint;
        }

        /**
         * Double-check locking to ensure the singleton is only created once.
         * Note the dogReporter is also volatile which is requried to make the double-check correct.
         */
        public static DatadogReporter Instance
        {
            get
            {
                if (INSTANCE == null)
                {
                    lock (syncRoot)
                    {
                        if (INSTANCE == null)
                        {
                            if (QUEUE_SIZE > 0)
                            {
                                INSTANCE = new DatadogReporter(ENDPOINT, API_KEY, QUEUE_SIZE);
                            }
                            else
                            {
                                INSTANCE = new DatadogReporter(ENDPOINT, API_KEY);
                            }
                        }
                    }
                }
                return INSTANCE;
            }
        }

        public DatadogReporter(string datadogEndpoint, string apiKey, int queueSize = -1)
        {
            string url = datadogEndpoint + "?api_key=" + apiKey;
            host = Environment.MachineName;
            jsonTags = new List<string>();

            if (queueSize > 0) {
                datadogPoster = new AsyncEventReporter(url, queueSize);
            }
            else
            {
                datadogPoster = new AsyncEventReporter(url);

            }
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
            Event e = new Event();
            e.metric = metric;
            e.type = type;
            e.host = host;
            e.tags = jsonTags;
            IList<long> point = new List<long>();
            point.Add(GetEpochTime());
            point.Add(value);
            IList<IList<long>> points = new List<IList<long>>();
            points.Add(point);

            e.points = points;

            Series s = new Series();
            s.series = new List<Event>();
            s.series.Add(e);

            string json = JsonConvert.SerializeObject(s); 
            datadogPoster.ReportEvent(json);
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