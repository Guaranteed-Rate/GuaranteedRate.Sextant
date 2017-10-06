using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EllieMae.EMLite.AsyncTaskManager;
using GuaranteedRate.Sextant.WebClients;

namespace GuaranteedRate.Sextant.Logging
{
    public abstract class SimpleLogAppender : AsyncEventReporter
    {
        public bool AllEnabled { get; set; }
        public bool DebugEnabled { get; set; }
        public bool InfoEnabled { get; set; }
        public bool WarnEnabled { get; set; }
        public bool ErrorEnabled { get; set; }
        public bool FatalEnabled { get; set; }
        protected static ISet<string> _tags;


        protected SimpleLogAppender(int queueLength, int retries) : base(queueLength, retries)
        {

        }

        public void Log(IDictionary<string, string> fields)
        {
            if (AllEnabled)
            {
                ReportEvent(fields);
            }
            else if (DebugEnabled &&
                     string.Equals(fields[Logger.LEVEL], Logger.DEBUG_LEVEL, StringComparison.CurrentCultureIgnoreCase))
            {
                ReportEvent(fields);
            }
            else if (InfoEnabled &&
                     string.Equals(fields[Logger.LEVEL], Logger.INFO_LEVEL, StringComparison.CurrentCultureIgnoreCase))
            {
                ReportEvent(fields);
            }
            else if (WarnEnabled &&
                     string.Equals(fields[Logger.LEVEL], Logger.WARN_LEVEL,
                         StringComparison.CurrentCultureIgnoreCase))
            {
                ReportEvent(fields);
            }
            else if (ErrorEnabled &&
                     string.Equals(fields[Logger.LEVEL], Logger.ERROR_LEVEL,
                         StringComparison.CurrentCultureIgnoreCase))
            {
                ReportEvent(fields);
            }
            else if (FatalEnabled &&
                     string.Equals(fields[Logger.LEVEL], Logger.FATAL_LEVEL,
                         StringComparison.CurrentCultureIgnoreCase))
            {
                ReportEvent(fields);
            }
        }


    }
}