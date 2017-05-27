using System.Collections.Generic;
using GuaranteedRate.Sextant.Config;

namespace GuaranteedRate.Sextant.Loggers
{
    public interface ILogReporter
    {
        void SetUp(IEncompassConfig config);
        void Log(IDictionary<string, string> fields);
    }
}
