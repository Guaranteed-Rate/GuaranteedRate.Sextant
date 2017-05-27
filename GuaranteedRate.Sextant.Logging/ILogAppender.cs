using System.Collections.Generic;
using GuaranteedRate.Sextant.Config;

namespace GuaranteedRate.Sextant.Logging
{
    public interface ILogAppender
    {
        void Setup(IEncompassConfig config);
        void Log(IDictionary<string, string> fields);
        bool DebugEnabled { get; }
        bool InfoEnabled { get; }
        bool WarnEnabled { get; }
        bool ErrorEnabled { get; }
        bool FatalEnabled { get; } 

    }
}
