using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.Metrics
{
    public interface IReporter
    {
        void AddCounter(string name, long value);
        void AddGauge(string name, long value);
        void AddMeter(string name, long value);
    }
}
