using System;
using System.Collections.Generic;
using System.Linq;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.WebClients;
using Infragistics.Win.UltraWinTabs;

namespace GuaranteedRate.Sextant.Logging.Console
{
    /// <summary>
    /// trivial console appender for logs
    /// </summary>
    public class ConsoleLogAppender : SimpleLogAppender, ILogAppender
    {


        #region config mappings

        public static string CONSOLE_ENABLED = "ConsoleLogAppender.Enabled";
        public static string CONSOLE_ALL = "ConsoleLogAppender.All.Enabled";
        public static string CONSOLE_ERROR = "ConsoleLogAppender.Error.Enabled";
        public static string CONSOLE_WARN = "ConsoleLogAppender.Warn.Enabled";
        public static string CONSOLE_INFO = "ConsoleLogAppender.Info.Enabled";
        public static string CONSOLE_DEBUG = "ConsoleLogAppender.Debug.Enabled";
        public static string CONSOLE_FATAL = "ConsoleLogAppender.Fatal.Enabled";
        public static readonly int QUEUE_LENGTH = 10;
        public static readonly int RETRIES = 10;  //how would it fail?

        #endregion

        public ConsoleLogAppender(IEncompassConfig config):base(QUEUE_LENGTH,RETRIES)
        {
            Setup(config);
        }

        public ConsoleLogAppender(IEncompassConfig config, int queueLength, int retries):base(queueLength,retries)
        {
            Setup(config);
        }

        protected void Setup(IEncompassConfig config)
        {
            AllEnabled = config.GetValue(CONSOLE_ALL, true);
            ErrorEnabled = config.GetValue(CONSOLE_ERROR, true);
            WarnEnabled = config.GetValue(CONSOLE_WARN, true);
            InfoEnabled = config.GetValue(CONSOLE_INFO, true);
            DebugEnabled = config.GetValue(CONSOLE_DEBUG, true);
            FatalEnabled = config.GetValue(CONSOLE_FATAL, true);
            _tags = new HashSet<string>();
        }


        public void AddTag(string tag)
        {
            _tags.Add(tag);
        }

        /// <summary>
        /// Post event actually writes it.
        /// </summary>
        /// <param name="formattedData"></param>
        /// <returns></returns>
        protected override bool PostEvent(object formattedData)
        {
            try
            {


                var le = SimpleLogEvent.Create(formattedData, _tags);
                System.Console.WriteLine(
                    $"{le.level}: {le.timestamp} - {le.message} - {string.Join(",", _tags)} {le.hostname}.");
                return true;
            }
            catch (Exception ex)
            {
                    
                throw;
            }
        }
    }
}
