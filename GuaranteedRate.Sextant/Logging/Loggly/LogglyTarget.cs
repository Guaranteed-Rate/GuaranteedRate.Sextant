//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using GuaranteedRate.Sextant.Config;
//using Loggly;
//using Loggly.Config;
//using NLog;

//namespace GuaranteedRate.Sextant.Logging.Loggly
//{
//    public class LogglyTarget : NLog.Targets.Target
//    {
//        private bool enabled = false;
//        public static string LOGGLY_ENABLED = "LogglyLogAppender.Enabled";
//        public static string LOGGLY_URL = "LogglyLogAppender.Url";
//        public static string LOGGLY_APIKEY = "LogglyLogAppender.ApiKey";
//        public static string LOGGLY_QUEUE_SIZE = "LogglyLogAppender.QueueSize";
//        public static string LOGGLY_RETRY_LIMIT = "LogglyLogAppender.RetryLimit";
//        public static string LOGGLY_ALL = "LogglyLogAppender.All.Enabled";
//        public static string LOGGLY_ERROR = "LogglyLogAppender.Error.Enabled";
//        public static string LOGGLY_WARN = "LogglyLogAppender.Warn.Enabled";
//        public static string LOGGLY_INFO = "LogglyLogAppender.Info.Enabled";
//        public static string LOGGLY_DEBUG = "LogglyLogAppender.Debug.Enabled";
//        public static string LOGGLY_FATAL = "LogglyLogAppender.Fatal.Enabled";
//        public static string LOGGLY_TAGS = "LogglyLogAppender.Tags";
//        public static string LOGGLY_LOG_RECURSIVELY = "LogglyLogAppender.LogRecursively";
//        public void Setup(IEncompassConfig config)
//        {
//            LogglyConfig.Instance.CustomerToken = config.GetValue(LOGGLY_APIKEY, "NOT SPECIFIED");
//            LogglyConfig.Instance.Transport.EndpointHostname = config.GetValue(LOGGLY_URL, "NOT SPECIFIED");
//            foreach (var t in config.GetValue(LOGGLY_TAGS, "").Split(new[] { ",", "|" }, StringSplitOptions.RemoveEmptyEntries))
//            {
//                var tag = new SimpleTag { Value = t };
//                LogglyConfig.Instance.TagConfig.Tags.Add(tag);

//            }
//        }

//        public Dictionary<string, string> Parameters { get; set; }
//        protected override void Write(LogEventInfo logEvent)
//        {
//            if (!enabled)
//            {
//                return;
//            }
//            var loggly = new LogglyClient();
//            var logglyEvt = new LogglyEvent();
//            logglyEvt.Data = new MessageData();
//            logglyEvt.Data.Add("message", logEvent.Message);
//            foreach (var param in Parameters)
//            {
//                logglyEvt.Data.Add(param.Key, param.Value);
//            }
//var res  =             loggly.Log(logglyEvt).Result;

//        }
//    }
//}
