using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuaranteedRate.Sextant.Integration.Core;
using GuaranteedRate.Sextant.Metrics.Graphite;
using NUnit.Framework;

namespace GuaranteedRate.Sextant.Integration.Tests.Metrics.Graphite
{
    [TestFixture]
    public class GraphiteReporterIntegrationTests
    {
        [Test, Category("Integration")]
        public void WhenAddCounter_ThenSuccess()
        {
            using (var sut = new GraphiteReporter(new IntegrationEncompassConfig()))
            {
                sut.AddCounter("my-metric", 10);
            }
        }
    }
}
