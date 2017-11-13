using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GuaranteedRate.Sextant.Logging
{
    public class SimpleLogEvent
    {
        public string loggerName { get; set; }
        public string process { get; set; }
        public string level { get; set; }
        public DateTime timestamp { get; set; }
        public string message { get; set; }
        public string hostname { get; set; }

        public string tags { get; set; }

        public static SimpleLogEvent Create(object data, ISet<string> tags)
        {
            var fields = data as IDictionary<string, string>;

            if (fields == null) return null;

            var loggerName = "undefined";
            if (fields.ContainsKey("loggerName"))
            {
                loggerName = fields["loggerName"];
                loggerName = loggerName.ToLower();
            }

            //fields["timestamp"] = GetEpochTime().ToString();

            fields["tags"] = JsonConvert.SerializeObject(tags);

            //tolower all the keys
            var lcfields = new Dictionary<string, string>();
            foreach (var key in fields.Keys)
            {
                if (!lcfields.ContainsKey(key.ToLower()))
                {
                    lcfields.Add(key.ToLower(), fields[key]);
                        //if you have a key like "Message" and another like "message" you get only the contents of "message."  Don't do that.
                }
            }

            var logEvent = new SimpleLogEvent()
            {
                loggerName =
                    lcfields.ContainsKey("loggername")
                        ? lcfields["loggername"]
                        : Assembly.GetExecutingAssembly().GetName().Name,
                hostname = lcfields.ContainsKey("hostname") ? lcfields["hostname"] : System.Environment.MachineName,
                timestamp = lcfields.ContainsKey("timestamp") ? DateTime.Parse(lcfields["timestamp"]) : DateTime.UtcNow,
                level = fields.ContainsKey(Logger.LEVEL) ? fields[Logger.LEVEL] : "INFO",
                message = lcfields.ContainsKey("message") ? lcfields["message"] : "no message",
                process =
                    lcfields.ContainsKey("process")
                        ? lcfields["process"]
                        : Assembly.GetExecutingAssembly().GetName().Name,
                tags = lcfields.ContainsKey("tags") ? lcfields["tags"] : ""
            };
            return logEvent;
        }
    }
}