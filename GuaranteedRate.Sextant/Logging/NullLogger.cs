using System;
using System.Collections.Generic;

namespace GuaranteedRate.Sextant.Logging
{
    /// <summary>
    /// Implementation of Logger to use when no configuration settings are available.
    /// Will disregard messages without raising exceptions.
    /// </summary>
    public class NullLogger : ILogger
    {
        public void Debug(string message, IDictionary<string, string> tags = null) { }

        public void Debug(string message, Exception exception, IDictionary<string, string> tags = null) { }

        public void Error(string message, IDictionary<string, string> tags = null) { }

        public void Error(string message, Exception exception, IDictionary<string, string> tags = null) { }

        public void Fatal(string message, IDictionary<string, string> tags = null) { }

        public void Fatal(string message, Exception exception, IDictionary<string, string> tags = null) { }

        public void Info(string message, IDictionary<string, string> tags = null) { }

        public void Info(string message, Exception exception, IDictionary<string, string> tags = null) { }

        public void Warning(string message, IDictionary<string, string> tags = null) { }

        public void Warning(string message, Exception exception, IDictionary<string, string> tags = null) { }

    }
}
