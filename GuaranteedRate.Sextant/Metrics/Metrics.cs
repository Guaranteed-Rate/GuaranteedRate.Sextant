using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.Metrics
{
    public class Metrics
    {
        private static volatile IList<IReporter> _reporters = new List<IReporter>();
        private static readonly object syncRoot = new Object();

        /// <summary>
        /// Initializes Metrics with a single reporter
        /// </summary>
        /// <param name="reporter"></param>
        public static void Init(IReporter reporter)
        {
            AddReporter(reporter);
        }

        /// <summary>
        /// Add a single reporter to the collection of reporters
        /// </summary>
        /// <param name="reporter"></param>
        public static void AddReporter(IReporter reporter)
        {
            lock (syncRoot)
            {
                if (_reporters == null)
                {
                    _reporters = new List<IReporter>();
                }
                _reporters.Add(reporter);
            }
        }


        public static void AddCounter(string name, long value)
        {
            if (_reporters == null) return;

            foreach (var reporter in _reporters)
            {
                reporter.AddCounter(name, value);
            }
        }

        public static void AddGauge(string name, long value)
        {
            if (_reporters == null) return;

            foreach (var reporter in _reporters)
            {
                reporter.AddGauge(name, value);
            }
        }

        public static void AddMeter(string name, long value)
        {
            if (_reporters == null) return;

            foreach (var reporter in _reporters)
            {
                reporter.AddMeter(name, value);
            }
        }
    }
}
