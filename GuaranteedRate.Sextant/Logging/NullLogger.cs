using System;

namespace GuaranteedRate.Sextant.Logging
{
    /// <summary>
    /// Implementation of Logger to use when no configuration settings are available.
    /// Will disregard messages without raising exceptions.
    /// </summary>
    public class NullLogger : ILogger
    {
        public void Debug(string message) { }

        public void Debug(string message, Exception exception) { }

        public void Error(string message) { }

        public void Error(string message, Exception exception) { }

        public void Fatal(string message) { }

        public void Fatal(string message, Exception exception) { }

        public void Info(string message) { }

        public void Info(string message, Exception exception) { }

        public void Warning(string message) { }

        public void Warning(string message, Exception exception) { }

    }
}
