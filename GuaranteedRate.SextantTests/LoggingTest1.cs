using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GuaranteedRate.SextantTests
{
    class LoggingTest1
    {
        [Test()]
        public async void foo()
        {


            var logger = new Sextant.Logging.Elasticsearch.ElasticsearchLogger();

            GuaranteedRate.Sextant.Logging.Logger.AddAppender(logger);
        }

    }
}
