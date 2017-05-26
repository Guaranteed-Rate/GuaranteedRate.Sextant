using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GuaranteedRate.Sextant.Loggers;
using NUnit.Framework;

namespace GuaranteedRate.SextantTests
{
    class LoggingTest1
    {
        [Test()]
        public async void foo()
        {
            var logger = new GuaranteedRate.Sextant.Logging.Elasticsearch.ElasticsearchLogger();
            var dict = new Dictionary<string, string>();
            dict.Add("foo", "bar");

            for (int i = 0; i < 40; i++)
            {
               var result=   await logger.Log(dict, "begytest", "error");
                Assert.That(result);
            }

        }

    }
}
