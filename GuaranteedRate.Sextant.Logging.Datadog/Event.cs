using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.Logging.Datadog
{
    class Event
    {
        //Using lowercase to help with json
        public string metric { get; set; }
        public IList<IList<long>> points { get; set; }
        public string type { get; set; }
        public string host { get; set; }
        public ISet<string> tags { get; set; }
    }
}
