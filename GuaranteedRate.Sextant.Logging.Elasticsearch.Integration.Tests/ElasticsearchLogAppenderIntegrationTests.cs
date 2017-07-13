using System;
using System.Collections.Generic;
using System.Threading;
using GuaranteedRate.Sextant.Integration.Core;
using NUnit.Framework;

namespace GuaranteedRate.Sextant.Logging.Elasticsearch.Integration.Tests
{
    [TestFixture]
    public class ElasticsearchLogAppenderIntegrationTests
    {
        public ElasticsearchLogAppender _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new ElasticsearchLogAppender(new IntegrationEncompassConfig());
        }

        [Test, Category("Integration")]
        public void WhenLog_ThenSuccess()
        {
            var fields = new Dictionary<string, string>
            {
                {"Application", "Encompass"},
                {"Company", "Guaranteed Rate"},
                {"loggerName", "Guaranteed Rate Encompass Logger" }
            };
            _sut.Log(fields);

            Thread.Sleep(10000);
        }
    }
}
