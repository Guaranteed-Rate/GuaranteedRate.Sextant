using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.Metrics
{
    interface IDataDogReporter
    {
        void AddCounter(string metric, long value);
         
        void AddGauge(string metric, long value);
    }
}