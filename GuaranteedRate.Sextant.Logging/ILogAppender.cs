using System.Collections.Generic;
using GuaranteedRate.Sextant.Config;

namespace GuaranteedRate.Sextant.Logging
{
    /// <summary>
    /// describes a class for appending log information to an arbitrary source
    /// </summary>
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
