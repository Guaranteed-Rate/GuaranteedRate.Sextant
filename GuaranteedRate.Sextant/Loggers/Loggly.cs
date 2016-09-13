using EllieMae.Encompass.Client;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.WebClients;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GuaranteedRate.Sextant.Loggers
{
    /// <summary>
    /// This is a minimal class to aysnchronously push data to loggly.
    /// It has a basic set of log4net style methods.
    /// We can expand if find them to be useful
    /// 
    /// NOTE: Because of how Loggly does tagging, you must set the url and tags BEFORE you start logging events
    /// </summary>
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
        private const string FATAL = "FATAL";

        private static ISet<string> tags = new HashSet<string>();
        private static int QUEUE_SIZE = DEFAULT_QUEUE_SIZE;
        private static string POST_URL;

        private static bool active = false;

        public bool ErrorEnabled { get; set; }
        public bool WarnEnabled { get; set; }
        public bool InfoEnabled { get; set; }
        public bool DebugEnabled { get; set; }
        public bool FatalEnabled { get; set; }

        public static string LOGGLY_URL = "Loggly.Url";
        public static string LOGGLY_ALL = "Loggly.All.Enabled";
        public static string LOGGLY_ERROR = "Loggly.Error.Enabled";
        public static string LOGGLY_WARN = "Loggly.Warn.Enabled";
        public static string LOGGLY_INFO = "Loggly.Info.Enabled";
        public static string LOGGLY_DEBUG = "Loggly.Debug.Enabled";
        public static string LOGGLY_FATAL = "Loggly.Fatal.Enabled";
        public static string LOGGLY_TAGS = "Loggly.Tags";

        /// <summary>
        /// Tags must be added BEFORE anything is logged.
        /// Once the first event is logged, the tags are locked
        /// </summary>
        /// <param name="tag"></param>
        public static void AddTag(string tag)
        {
            if (!tags.Contains(tag.Trim()))
            {
                tags.Add(tag.Trim());
            }
        }

        public static void SetSize(int queueSize)
        {
            Loggly.QUEUE_SIZE = queueSize;
        }

        public static void SetPostUrl(string url)
        {
            Loggly.POST_URL = url;
            Loggly.active = !String.IsNullOrWhiteSpace(url);
        }

         
        /// <summary>
        /// Initializes the loggly logger
        /// </summary>
        /// <param name="session">Current encompass session, used to load the config file</param>
        /// <param name="config">Config file to load.  Will re-initialize when this is used.</param>
        /// <param name="tags">Collection of loggly tags.  If null, we will load the Loggly.Tags element from the config.</param>
        public static void Init(Session session, IEncompassConfig config, ICollection<string> tags = null)
        {
            config.Init(session);
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

            string configLogglyUrl = config.GetValue(LOGGLY_URL);

            if (configLogglyUrl != null)
            {
                SetPostUrl(configLogglyUrl);
            }
            bool allEnabled = config.GetValue(LOGGLY_ALL, false);
            bool errorEnabled = config.GetValue(LOGGLY_ERROR, false);
            bool warnEnabled = config.GetValue(LOGGLY_WARN, false);
            bool infoEnabled = config.GetValue(LOGGLY_INFO, false);
            bool debugEnabled = config.GetValue(LOGGLY_DEBUG, false);
            bool fatalEnabled = config.GetValue(LOGGLY_FATAL, false);

            Instance.ErrorEnabled = allEnabled || errorEnabled;
            Instance.WarnEnabled = allEnabled || warnEnabled;
            Instance.InfoEnabled = allEnabled || infoEnabled;
            Instance.DebugEnabled = allEnabled || debugEnabled;
            Instance.FatalEnabled = allEnabled || fatalEnabled;
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

        private Loggly(string url, int queueSize = DEFAULT_QUEUE_SIZE) : base(url, queueSize)
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
            ErrorEnabled = true;
            WarnEnabled = true;
            InfoEnabled = true;
            DebugEnabled = true;
            FatalEnabled = true;
        }

        private static bool LogError()
        {
            return (active && Instance.ErrorEnabled);
        }

        private static bool LogWarn()
        {
            return (active && Instance.WarnEnabled);
        }

        private static bool LogInfo()
        {
            return (active && Instance.InfoEnabled);
        }

        private static bool LogDebug()
        {
            return (active && Instance.DebugEnabled);
        }
        private static bool LogFatal()
        {
            return (active && Instance.FatalEnabled);
        }

        public static void Fatal(string loggerName, string message)
        {
            if (LogError())
            {
                IDictionary<string, string> fields = new Dictionary<string, string>();
                fields.Add("message", message);
                Log(fields, loggerName, FATAL);
            }
        }


        public static void ErrFatalor(string loggerName, IDictionary<string, string> fields)
        {
            if (LogError())
            {
                Log(fields, loggerName, FATAL);
            }
        }
 
        public static void Error(string loggerName, string message)
        {
            if (LogError())
            {
                IDictionary<string, string> fields = new Dictionary<string, string>();
                fields.Add("message", message);
                Log(fields, loggerName, ERROR);
            }
        }

        public static void Error(string loggerName, IDictionary<string, string> fields)
        {
            if (LogError())
            {
                Log(fields, loggerName, ERROR);
            }
        }

        public static void Warn(string loggerName, string message)
        {
            if (LogWarn())
            {
                IDictionary<string, string> fields = new Dictionary<string, string>();
                fields.Add("message", message);
                Log(fields, loggerName, WARN);
            }
        }

        public static void Warn(string loggerName, IDictionary<string, string> fields)
        {
            if (LogWarn())
            {
                Log(fields, loggerName, WARN);
            }
        }

        public static void Info(string loggerName, string message)
        {
            if (LogInfo())
            {
                IDictionary<string, string> fields = new Dictionary<string, string>();
                fields.Add("message", message);
                Log(fields, loggerName, INFO);
            }
        }

        public static void Info(string loggerName, IDictionary<string, string> fields)
        {
            if (LogInfo())
            {
                Log(fields, loggerName, INFO);
            }
        }

        public static void Debug(string loggerName, string message)
        {
            if (LogDebug())
            {
                IDictionary<string, string> fields = new Dictionary<string, string>();
                fields.Add("message", message);
                Log(fields, loggerName, DEBUG);
            }
        }

        public static void Debug(string loggerName, IDictionary<string, string> fields)
        {
            if (LogDebug())
            {
                Log(fields, loggerName, DEBUG);
            }
        }

        public static void Log(IDictionary<string, string> fields, string loggerName, string level)
        {
            if (active)
            {
                PopulateEvent(fields, loggerName, level);
                //Having Indented formatting makes the data format better in the Loggly
                //Search screen
                string json = JsonConvert.SerializeObject(fields, Formatting.Indented);
                Instance.ReportEvent(json);
            }
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