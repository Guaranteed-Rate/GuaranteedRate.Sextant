using System.Collections.Generic;

namespace GuaranteedRate.Sextant.Logging
{
    /// <summary>
    /// describes a class for appending log information to an arbitrary source
    /// </summary>
    public interface ILogAppender
    {
        void Log(IDictionary<string, string> fields);
        void AddTag(string tag);
        bool AllEnabled { get; }
        bool DebugEnabled { get; }
        bool InfoEnabled { get; }
        bool WarnEnabled { get; }
        bool ErrorEnabled { get; }
        bool FatalEnabled { get; } 
    }
}
