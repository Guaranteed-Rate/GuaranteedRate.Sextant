using System;
using System.Collections.Generic;
using NUnit.Framework;
using GuaranteedRate.Sextant.Integration.Core;

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

        [Test]
        public void WhenLog_ThenSuccess()
        {
            var fields = new Dictionary<string, string>
            {
                {"Application", "Encompass"},
                {"Company", "Guaranteed Rate"}

            };
            _sut.Log(fields);
        }
    }
}
