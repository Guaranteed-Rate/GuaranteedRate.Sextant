using System;
using System.Collections.Generic;
using NUnit.Framework;
using GuaranteedRate.Sextant.Integration.Core;

namespace GuaranteedRate.Sextant.Logging.Loggly.Integration.Tests
{
    [TestFixture]
    public class LogglyLogAppenderIntegrationTests
    {
        [Test, Category("Integration")]
        public void WhenLog_ThenSuccess()
        {
            using (var sut = new LogglyLogAppender(new IntegrationEncompassConfig()))
            {
                var fields = new Dictionary<string, string>
                {
                    {"Application", "Encompass"},
                    {"Company", "Guaranteed Rate"}

                };

                sut.Log(fields);
            }
        }
    }
}
