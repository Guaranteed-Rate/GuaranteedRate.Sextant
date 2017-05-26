using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuaranteedRate.Sextant.Config;

namespace GuaranteedRate.Sextant.Loggers
{
    public interface ILogReporter
    {
        void SetUp(IEncompassConfig config);
        void Log(IDictionary<string, string> fields);
    }
}
