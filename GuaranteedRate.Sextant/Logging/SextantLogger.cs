using GuaranteedRate.Sextant.Config;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GuaranteedRate.Sextant.Logging
{
    /// <summary>
    /// Sextant implementation for ILogger that wraps around the existing Logger class methods
    /// </summary>
    public class SextantLogger : ILogger
    {
        private readonly string _logglyName;

        /// <summary>
        /// Creates instance of SextantLogger without client context or additional tags
        /// </summary>
        /// <param name="config">Encompass configuration to use for Logger setup</param>
        /// <param name="loggerName">Logger name for this instance</param>
        public SextantLogger(IEncompassConfig config, string loggerName)
        {
            _logglyName = loggerName;

            Setup(config, string.Empty, string.Empty);
        }

        /// <summary>
        /// Creates instance of SextantLogger with the specified client and context
        /// </summary>
        /// <param name="config">Encompass configuration to use for Logger setup</param>
        /// <param name="loggerName">Logger name for this instance</param>
        public SextantLogger(IEncompassConfig config, string loggerName, string clientId, string context)
        {
            _logglyName = loggerName;

            Setup(config, clientId, context);
        }

        /// <summary>
        /// Creates instance of SextantLogger with the tags provided
        /// </summary>
        /// <param name="config">Encompass configuration to use for Logger setup</param>
        /// <param name="loggerName">Logger name for this instance</param>
        public SextantLogger(IEncompassConfig config, string loggerName, Dictionary<string, string> additionalTags)
        {
            _logglyName = loggerName;

            Setup(config, additionalTags);
        }

        private void Setup(IEncompassConfig config, string clientId, string context)
        {
            var tags = new Dictionary<string, string>();

            tags.Add("assembly", GetAssemblyNameSafely());
            tags.Add("version", GetVersionSafely());
            tags.Add("environment", config.GetValue("Encompass.Environment"));

            if (!string.IsNullOrEmpty(context)) tags.Add("context", context);
            if (!string.IsNullOrEmpty(clientId)) tags.Add("clientid", clientId);

            Setup(config, tags);
        }

        private void Setup(IEncompassConfig config, Dictionary<string, string> additionalTags)
        {
            Logger.Setup(config, additionalTags);
        }

        /// <summary>
        /// Debugs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        public void Debug(string message, IDictionary<string, string> tags = null)
        {
            Logger.Debug(_logglyName, message, tags);
        }

        /// <summary>
        /// Debugs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        public void Debug(string message, Exception exception, IDictionary<string, string> tags = null)
        {
            Debug(Combine(message, exception), tags);
        }

        /// <summary>
        /// Informations the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        public void Info(string message, IDictionary<string, string> tags = null)
        {
            Logger.Info(_logglyName, message, tags);
        }

        /// <summary>
        /// Informations the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        public void Info(string message, Exception exception, IDictionary<string, string> tags = null)
        {
            Info(Combine(message, exception), tags);
        }

        /// <summary>
        /// Warnings the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        public void Warning(string message, IDictionary<string, string> tags = null)
        {
            Logger.Warn(_logglyName, message, tags);
        }

        /// <summary>
        /// Warnings the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        public void Warning(string message, Exception exception, IDictionary<string, string> tags = null)
        {
            Warning(Combine(message, exception), tags);

        }

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        public void Error(string message, IDictionary<string, string> tags = null)
        {
            Logger.Error(_logglyName, message, tags);
        }

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        public void Error(string message, Exception exception, IDictionary<string, string> tags = null)
        {
            Error(Combine(message, exception), tags);
        }

        /// <summary>
        /// Fatals the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        public void Fatal(string message, IDictionary<string, string> tags = null)
        {
            Logger.Fatal(_logglyName, message, tags);
        }

        /// <summary>
        /// Fatals the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        public void Fatal(string message, Exception exception, IDictionary<string, string> tags = null)
        {
            Fatal(Combine(message, exception), tags);
        }

        /// <summary>
        /// Sextant doesn't support supplying the exception with the message, so we combine them into one string
        /// to submit to the Sextant logger.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        private string Combine(string message, Exception exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine(message);
            sb.AppendLine($"Exception: {exception.GetType().Name}");
            sb.AppendLine(exception.Message);
            sb.AppendLine(exception.StackTrace);
            return sb.ToString();
        }

        private string GetVersionSafely()
        {
            string version;
            try
            {
                version = Assembly.GetExecutingAssembly().FullName.Split(',')[1].Split('=')[1];
            }
            catch
            {
                version = "Unable to retrieve version";
            }
            return version;
        }

        private string GetAssemblyNameSafely()
        {
            string version;
            try
            {
                version = Assembly.GetExecutingAssembly().FullName.Split(',')[0];
            }
            catch
            {
                version = "Unable to retrieve assembly name";
            }
            return version;
        }

    }
}
