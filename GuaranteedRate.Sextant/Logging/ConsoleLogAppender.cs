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
        public bool AllEnabled { get; private set; }
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
            var allEnabled = config.GetValue(CONSOLE_ALL, true);
            var errorEnabled = config.GetValue(CONSOLE_ERROR, true);
            var warnEnabled = config.GetValue(CONSOLE_WARN, true);
            var infoEnabled = config.GetValue(CONSOLE_INFO, true);
            var debugEnabled = config.GetValue(CONSOLE_DEBUG, true);
            var fatalEnabled = config.GetValue(CONSOLE_FATAL, true);

            AllEnabled = allEnabled;
            ErrorEnabled = allEnabled || errorEnabled;
            WarnEnabled = allEnabled || warnEnabled;
            InfoEnabled = allEnabled || infoEnabled;
            DebugEnabled = allEnabled || debugEnabled;
            FatalEnabled = allEnabled || fatalEnabled;
        }

        public void Log(IDictionary<string, string> fields)
        {
            if (AllEnabled)
            {
                Console.WriteLine(fields.Values.Aggregate((a, b) => $"{a}, {b}"));
            }
            else if (DebugEnabled && string.Equals(fields["level"], Logger.DEBUG, StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine(fields.Values.Aggregate((a, b) => $"{a}, {b}"));
            }
            else if (InfoEnabled && string.Equals(fields["level"], Logger.INFO, StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine(fields.Values.Aggregate((a, b) => $"{a}, {b}"));
            }
            else if (WarnEnabled && string.Equals(fields["level"], Logger.WARN, StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine(fields.Values.Aggregate((a, b) => $"{a}, {b}"));
            }
            else if (ErrorEnabled && string.Equals(fields["level"], Logger.ERROR, StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine(fields.Values.Aggregate((a, b) => $"{a}, {b}"));
            }
            else if (FatalEnabled && string.Equals(fields["level"], Logger.FATAL, StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine(fields.Values.Aggregate((a, b) => $"{a}, {b}"));
            }
        }

        public void AddTag(string tag)
        {
            //tagging not supported for console log appender
        }
    }
}
