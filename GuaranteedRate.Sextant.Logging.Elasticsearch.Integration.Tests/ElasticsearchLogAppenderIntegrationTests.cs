using System;
using System.Collections.Generic;
using GuaranteedRate.Sextant.Integration.Core;
using NUnit.Framework;

namespace GuaranteedRate.Sextant.Logging.Elasticsearch.Integration.Tests
{
    [TestFixture]
    public class ElasticsearchLogAppenderIntegrationTests
    {
        [Test, Category("Integration")]
        public void WhenLog_ThenSuccess()
        {
            using (var sut = new ElasticsearchLogAppender(new IntegrationEncompassConfig()))
            {
                var fields = new Dictionary<string, string>
                {
                    {"Application", "Encompass"},
                    {"Company", "Guaranteed Rate"},
                    {"loggerName", "Guaranteed Rate Encompass Logger"}
                };
                sut.Log(fields);
            }
        }
    }
}
