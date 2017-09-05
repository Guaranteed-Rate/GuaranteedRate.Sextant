using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GuaranteedRate.Sextant.WebClients
{
    public class SecureAsyncEventReporter : AsyncEventReporter
    {
        private string _authorization { get; set; }

        public SecureAsyncEventReporter(string url, string authorization, int queueSize = DEFAULT_QUEUE_SIZE,
            int retries = DEFAULT_RETRIES)
            : base(url, queueSize, retries)
        {
            if (!string.IsNullOrEmpty(authorization))
            {
                _authorization = authorization;
            }
        }

        protected override void ExtraSetup(WebRequest webRequest)
        {
            webRequest.Headers.Add("Authorization:" + _authorization);
        }       
    }
}
