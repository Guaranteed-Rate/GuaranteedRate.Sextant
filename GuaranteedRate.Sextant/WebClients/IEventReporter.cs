using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.WebClients
{
    public interface IEventReporter
    {
        bool ReportEvent(string formattedData);
        void Shutdown();
    }
}