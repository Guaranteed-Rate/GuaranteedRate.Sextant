using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuaranteedRate.Sextant.Integration.Core;
using GuaranteedRate.Sextant.Metrics.Datadog;
using NUnit.Framework;

namespace GuaranteedRate.Sextant.Integration.Tests.Metrics.Datadog
{
    [TestFixture]
    public class DatadogReporterIntegrationTests
    {
        [Test, Category("Integration")]
        public void WhenAddCounter_ThenSuccess()
        {
            using (var sut = new DatadogReporter(new IntegrationEncompassConfig()))
            {
                sut.AddCounter("my-metric", 10);
            }
        }
    }
}
