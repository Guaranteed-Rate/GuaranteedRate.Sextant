using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.WebClients
{
    public class SecureAsyncEventReporter : AsyncEventReporter
    {
        public string authorization { get; set; }

        protected override void ExtraSetup(WebRequest webRequest)
        {
            webRequest.Headers.Add("Authorization:" + authorization);
        }

        public SecureAsyncEventReporter(string url, int queueSize = DEFAULT_QUEUE_SIZE, int retries = DEFAULT_RETRIES)
            : base(url, queueSize, retries)
        {
        }
    }
}