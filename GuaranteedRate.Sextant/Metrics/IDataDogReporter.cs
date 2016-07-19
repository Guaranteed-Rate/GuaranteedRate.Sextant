using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.Metrics
{
    public interface IDatadogReporter
    {
        void AddCounter(string metric, long value);
         
        void AddGauge(string metric, long value);

        void AddMeter(string metric, long value);

        void AddTag(string tag, string value);
    }
}