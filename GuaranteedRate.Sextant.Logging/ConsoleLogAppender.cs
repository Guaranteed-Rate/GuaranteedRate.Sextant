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

        public void Setup(IEncompassConfig config)
        {
            DebugEnabled = config.GetValue("ConsoleLogAppender.Debug.Enabled", false);
            InfoEnabled = config.GetValue("ConsoleLogAppender.Info.Enabled", false);
            WarnEnabled = config.GetValue("ConsoleLogAppender.Warn.Enabled", false);
            ErrorEnabled = config.GetValue("ConsoleLogAppender.Error.Enabled", true);
            FatalEnabled = config.GetValue("ConsoleLogAppender.Fatal.Enabled", true);
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
