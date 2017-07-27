using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace GuaranteedRate.Sextant.WebClients
{
    public class SecureAsyncEventReporter : AsyncEventReporter
    {
        public SecureAsyncEventReporter(string url, string authorization, int queueSize = DEFAULT_QUEUE_SIZE,
            int retries = DEFAULT_RETRIES)
            : base(url, queueSize, retries)
        {
            if (!string.IsNullOrEmpty(authorization))
            {
                Client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(authorization);
            }
        }
    }
}
