using System;
using System.Collections.Generic;
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
