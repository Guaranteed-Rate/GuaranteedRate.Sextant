using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using GuaranteedRate.Sextant.Integration.Core;
using GuaranteedRate.Sextant.WebClients;

namespace GuaranteedRate.Sextant.Logging.Loggly.Integration.Tests
{
    [TestFixture]
    public class LogglyLogAppenderIntegrationTests
    {
        public LogglyLogAppender _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new LogglyLogAppender(new IntegrationEncompassConfig());    
        }

        [Test, Category("Integration")]
        public void WhenLog_ThenSuccess()
        {
            
            var fields = new Dictionary<string, string>
            {
                {"Application", "Encompass"},
                {"Company", "Guaranteed Rate"}

            };

            _sut.Log(fields);

            Thread.Sleep(10000);
        }
    }
}
