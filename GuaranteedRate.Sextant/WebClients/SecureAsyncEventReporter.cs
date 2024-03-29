﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GuaranteedRate.Sextant.WebClients
{
    public class SecureAsyncEventReporter : AsyncWebEventReporter
    {
        protected override string Name { get; } = typeof(SecureAsyncEventReporter).Name;

        private Func<string> _authorization { get; set; }

        public SecureAsyncEventReporter(Func<string> url, Func<string> authorization, int queueSize = DEFAULT_QUEUE_SIZE,
            int retries = DEFAULT_RETRIES)
            : base(url, queueSize, retries)
        {
            if (authorization != null)
            {
                _authorization = authorization;
            }
        }

        protected override void ExtraSetup(WebRequest webRequest)
        {
            webRequest.Headers.Add("Authorization:" + _authorization());
        }       
    }
}
