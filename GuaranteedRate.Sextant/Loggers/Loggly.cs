using GuaranteedRate.Sextant.WebClients;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.Loggers
{
    /**
     * This is a minimal class to aysnchronously push data to loggly.
     * It has a basic set of log4net style methods.
     * We can expand if find them to be useful
     * 
     * NOTE: Because of how Loggly does tagging, you must set the url and tags BEFORE you start logging events
     */ 
    public class Loggly : AsyncEventReporter
    {
        private static volatile Loggly reporter;
        private static object syncRoot = new Object();

        public string Hostname { get; private set; }
        public string ProcessName { get; private set; }

        private const string ERROR = "ERROR";
        private const string WARN = "WARN";
        private const string INFO = "INFO";
        private const string DEBUG = "DEBUG";

        private static ISet<string> tags = new HashSet<string>();
        private static int QUEUE_SIZE = DEFAULT_QUEUE_SIZE;
        private static string POST_URL;

        /**
         * Tags must be added BEFORE anything is logged.
         * Once the first event is logged, the tags are locked
         */
        public static void AddTag(string tag) {
            tags.Add(tag);
        }

        public static void SetSize(int queueSize)
        {
            Loggly.QUEUE_SIZE = queueSize;
        }

        public static void SetPostUrl(string url)
        {
            Loggly.POST_URL = url;
        }

        /**
         * Double-check locking to ensure the singleton is only created once.
         * Note the reporter is also volatile which is requried to make the double-check correct.
         */
        public static Loggly Instance
        {
            get
            {
                if (reporter == null)
                {
                    lock (syncRoot)
                    {
                        if (reporter == null)
                        {
                            tags.Add("http");
                            string tagString = MakeTagCsv();
                            reporter = new Loggly(POST_URL + tagString + "/", QUEUE_SIZE);
                        }
                    }
                }
                return reporter;
            }
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


        private Loggly(string url, int queueSize = DEFAULT_QUEUE_SIZE) : base (url, queueSize)
        {
            ContentType = "text/plain";
            try
            {
                Hostname = Environment.MachineName;
                ProcessName = Process.GetCurrentProcess().ProcessName;
            }
            catch
            {
                Hostname = "UNKNOWN";
                ProcessName = "UNKNOWN";
            }
        }

        public static void Error(string loggerName, string message) 
        {
            IDictionary<string, string> fields = new Dictionary<string, string>();
            fields.Add("message", message);
            Log(fields, loggerName, ERROR);
        }

        public static void Error(string loggerName, IDictionary<string, string> fields)
        {
            Log(fields, loggerName, ERROR);
        }

        public static void Warn(string loggerName, string message)
        {
            IDictionary<string, string> fields = new Dictionary<string, string>();
            fields.Add("message", message);
            Log(fields, loggerName, WARN);
        }

        public static void Warn(string loggerName, IDictionary<string, string> fields)
        {
            Log(fields, loggerName, WARN);
        }

        public static void Info(string loggerName, string message)
        {
            IDictionary<string, string> fields = new Dictionary<string, string>();
            fields.Add("message", message);
            Log(fields, loggerName, INFO);
        }

        public static void Info(string loggerName, IDictionary<string, string> fields)
        {
            Log(fields, loggerName, INFO);
        }

        public static void Debug(string loggerName, string message)
        {
            IDictionary<string, string> fields = new Dictionary<string, string>();
            fields.Add("message", message);
            Log(fields, loggerName, DEBUG);
        }

        public static void Debug(string loggerName, IDictionary<string, string> fields)
        {
            Log(fields, loggerName, DEBUG);
        }

        public static void Log(IDictionary<string, string> fields, string loggerName, string level)
        {
            PopulateEvent(fields, loggerName, level);
            //Having Indented formatting makes the data format better in the Loggly
            //Search screen
            string json = JsonConvert.SerializeObject(fields, Formatting.Indented);
            Instance.ReportEvent(json);
        }

        private static void PopulateEvent(IDictionary<string, string> fields, string loggerName, string level)
        {
            fields.Add("level", level);
            fields.Add("timestamp", DateTime.Now.ToString("MM/dd/yyyyTHH:mm:ss.fffzzz"));
            fields.Add("hostname", Instance.Hostname);
            fields.Add("process", Instance.ProcessName);
            fields.Add("loggerName", loggerName);
        }
    }
}
