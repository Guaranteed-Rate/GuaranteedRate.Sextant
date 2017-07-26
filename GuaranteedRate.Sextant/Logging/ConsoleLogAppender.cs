using System;
using System.Collections.Generic;
using System.Linq;
using GuaranteedRate.Sextant.Config;

namespace GuaranteedRate.Sextant.Logging
{
    /// <summary>
    /// trivial console appender for logs
    /// </summary>
    public class ConsoleLogAppender : ILogAppender
    {
        public bool DebugEnabled { get; private set; }
        public bool InfoEnabled { get; private set; }
        public bool WarnEnabled { get; private set; }
        public bool ErrorEnabled { get; private set; }
        public bool FatalEnabled { get; private set; }

        #region config mappings
        
        public static string CONSOLE_ALL = "ConsoleLogAppender.All.Enabled";
        public static string CONSOLE_ERROR = "ConsoleLogAppender.Error.Enabled";
        public static string CONSOLE_WARN = "ConsoleLogAppender.Warn.Enabled";
        public static string CONSOLE_INFO = "ConsoleLogAppender.Info.Enabled";
        public static string CONSOLE_DEBUG = "ConsoleLogAppender.Debug.Enabled";
        public static string CONSOLE_FATAL = "ConsoleLogAppender.Fatal.Enabled";

        #endregion

        public ConsoleLogAppender(IEncompassConfig config)
        {
            Setup(config);
        }

        protected void Setup(IEncompassConfig config)
        {
            DebugEnabled = config.GetValue(CONSOLE_DEBUG, false);
            InfoEnabled = config.GetValue(CONSOLE_INFO, false);
            WarnEnabled = config.GetValue(CONSOLE_WARN, false);
            ErrorEnabled = config.GetValue(CONSOLE_ERROR, true);
            FatalEnabled = config.GetValue(CONSOLE_FATAL, true);
        }

        public void Log(IDictionary<string, string> fields)
        {
            if (string.Equals(fields["level"], "debug", StringComparison.CurrentCultureIgnoreCase) && DebugEnabled)
            {
                Console.WriteLine(fields.Values.Aggregate((a, b) => $"{a}, {b}"));
            }
            if (string.Equals(fields["level"], "info", StringComparison.CurrentCultureIgnoreCase) && InfoEnabled)
            {
                Console.WriteLine(fields.Values.Aggregate((a, b) => $"{a}, {b}"));
            }
            if (string.Equals(fields["level"], "warn", StringComparison.CurrentCultureIgnoreCase) && WarnEnabled)
            {
                Console.WriteLine(fields.Values.Aggregate((a, b) => $"{a}, {b}"));
            }
            if (string.Equals(fields["level"], "error", StringComparison.CurrentCultureIgnoreCase) && ErrorEnabled)
            {
                Console.WriteLine(fields.Values.Aggregate((a, b) => $"{a}, {b}"));
            }
            if (string.Equals(fields["level"], "fatal", StringComparison.CurrentCultureIgnoreCase) && FatalEnabled)
            {
                Console.WriteLine(fields.Values.Aggregate((a, b) => $"{a}, {b}"));
            }
        }
    }
}
