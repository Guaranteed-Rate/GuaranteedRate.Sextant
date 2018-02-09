using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.Logging.Console
{
   public  class Config
    {

        #region config mappings

        public static string CONSOLE_ENABLED = "ConsoleLogAppender.Enabled";
        public static string CONSOLE_ALL = "ConsoleLogAppender.All.Enabled";
        public static string CONSOLE_ERROR = "ConsoleLogAppender.Error.Enabled";
        public static string CONSOLE_WARN = "ConsoleLogAppender.Warn.Enabled";
        public static string CONSOLE_INFO = "ConsoleLogAppender.Info.Enabled";
        public static string CONSOLE_DEBUG = "ConsoleLogAppender.Debug.Enabled";
        public static string CONSOLE_FATAL = "ConsoleLogAppender.Fatal.Enabled";

        #endregion
    }
}
