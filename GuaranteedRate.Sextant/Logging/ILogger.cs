using System;
using System.Collections.Generic;

namespace GuaranteedRate.Sextant.Logging
{
    /// <summary>
    /// Interface for standard logging implementations that also enables proper unit testing and future dependency injection, when required
    /// </summary>
    public interface ILogger
    {

        /// <summary>
        /// Informations the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        void Info(string message, IDictionary<string, string> tags = null);

        /// <summary>
        /// Informations the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        void Info(string message, Exception exception, IDictionary<string, string> tags = null);

        /// <summary>
        /// Debugs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        void Debug(string message, IDictionary<string, string> tags = null);

        /// <summary>
        /// Debugs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        void Debug(string message, Exception exception, IDictionary<string, string> tags = null);

        /// <summary>
        /// Warnings the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        void Warning(string message, IDictionary<string, string> tags = null);

        /// <summary>
        /// Warnings the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        void Warning(string message, Exception exception, IDictionary<string, string> tags = null);

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        void Error(string message, IDictionary<string, string> tags = null);

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        void Error(string message, Exception exception, IDictionary<string, string> tags = null);

        /// <summary>
        /// Fatals the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        void Fatal(string message, IDictionary<string, string> tags = null);

        /// <summary>
        /// Fatals the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="tags">Optional tags as name/value pairs to be included in this log entry</param>
        void Fatal(string message, Exception exception, IDictionary<string, string> tags = null);
    }
}
