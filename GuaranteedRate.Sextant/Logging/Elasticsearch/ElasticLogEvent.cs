using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.Logging.Elasticsearch
{
    public class ElasticLogEvent
    {
        public string loggerName { get; set; }
        public string process { get; set; }
        public string level { get; set; }
        public DateTime timestamp { get; set; }
        public string message { get; set; }
        public string hostname { get; set; }
    }
}
