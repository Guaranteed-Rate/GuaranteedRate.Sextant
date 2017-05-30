using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuaranteedRate.Sextant.Config;

namespace GuaranteedRate.Sextant.Logging
{
    /// <summary>
    /// trivial console appender for logs
    /// </summary>
    public class ConsoleLogAppender:ILogAppender
    {
        public void Setup(IEncompassConfig config)
        {
            this.DebugEnabled = config.GetValue("ConsoleLogAppender.Debug.Enabled", false);
            this.InfoEnabled = config.GetValue("ConsoleLogAppender.Info.Enabled", false);
            this.WarnEnabled = config.GetValue("ConsoleLogAppender.Warn.Enabled", false);
            this.ErrorEnabled = config.GetValue("ConsoleLogAppender.Error.Enabled", true);
            this.FatalEnabled = config.GetValue("ConsoleLogAppender.Fatal.Enabled", true);
        }

        public void Log(IDictionary<string, string> fields)
        {
            if (String.Equals(fields["level"], "debug", StringComparison.CurrentCultureIgnoreCase) && this.DebugEnabled)
            {
                Console.WriteLine(fields.Values.Aggregate((a,b)=> $"{a}, {b}"));
            }
            if (String.Equals(fields["level"], "info", StringComparison.CurrentCultureIgnoreCase) && this.InfoEnabled)
            {
                Console.WriteLine(fields.Values.Aggregate((a, b) => $"{a}, {b}"));
            }
            if (String.Equals(fields["level"], "warn", StringComparison.CurrentCultureIgnoreCase) && this.WarnEnabled)
            {
                Console.WriteLine(fields.Values.Aggregate((a, b) => $"{a}, {b}"));
            }
            if (String.Equals(fields["level"], "error", StringComparison.CurrentCultureIgnoreCase) && this.ErrorEnabled)
            {
                Console.WriteLine(fields.Values.Aggregate((a, b) => $"{a}, {b}"));
            }
            if (String.Equals(fields["level"], "fatal", StringComparison.CurrentCultureIgnoreCase) && this.FatalEnabled)
            {
                Console.WriteLine(fields.Values.Aggregate((a, b) => $"{a}, {b}"));
            }
        }

        public bool DebugEnabled { get; private set; }
        public bool InfoEnabled { get; private set; }
        public bool WarnEnabled { get; private set; }
        public bool ErrorEnabled { get; private set; }
        public bool FatalEnabled { get; private set; }
    }
}
