using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuaranteedRate.Sextant;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.Logging;
using GuaranteedRate.Sextant.Logging.Elasticsearch;

namespace LoggingTestRig
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new JsonEncompassConfig();
            config.Init(System.IO.File.ReadAllText("../../../../LoggingTest.json"));
                
            var console = new ConsoleLogAppender();
            console.Setup(config);

            var loggly = new LogglyLogAppender(config.GetValue("LogglyLogAppender.LoggglyUrl"),1000);
            loggly.Setup(config);


            var elasticSearch = new ElasticsearchLogAppender();
            elasticSearch.Setup(config);

            Logger.AddAppender(console);
            Logger.AddAppender(loggly);
            Logger.AddAppender(elasticSearch);

            Logger.Debug("SextantTestRig", "Test debug message");
            Logger.Info("SextantTestRig", "Test info message");
            Logger.Warn("SextantTestRig", "Test warn message");
            Logger.Error("SextantTestRig", "Test error message");
            Logger.Fatal("SextantTestRig", "Test fatal message");

        }
    }
}
