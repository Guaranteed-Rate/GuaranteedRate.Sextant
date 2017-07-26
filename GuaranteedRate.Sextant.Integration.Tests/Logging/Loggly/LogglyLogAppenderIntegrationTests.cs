using System.Collections.Generic;
using GuaranteedRate.Sextant.Integration.Core;
using GuaranteedRate.Sextant.Logging.Loggly;
using NUnit.Framework;

namespace GuaranteedRate.Sextant.Integration.Tests.Logging.Loggly
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
