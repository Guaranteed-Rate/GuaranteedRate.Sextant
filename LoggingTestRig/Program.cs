using System;
using System.Collections.Generic;
using System.Linq;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.Logging;
using GuaranteedRate.Sextant.Logging.Elasticsearch;
using GuaranteedRate.Sextant.Logging.Loggly;
using GuaranteedRate.Sextant.Metrics;
using GuaranteedRate.Sextant.Metrics.Datadog;
using GuaranteedRate.Sextant.Metrics.Graphite;

namespace LoggingTestRig
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new JsonEncompassConfig();
            config.Init(System.IO.File.ReadAllText("../../SextantConfigTest.json"));
                
            var console = new ConsoleLogAppender(config);

            var loggly = new LogglyLogAppender(config);
            var elasticSearch = new ElasticsearchLogAppender(config);

            Logger.AddAppender(console);
            Logger.AddAppender(loggly);
            Logger.AddAppender(elasticSearch);

            Logger.Debug("SextantTestRig", "Test debug message");
            Logger.Info("SextantTestRig", "Test info message");
            Logger.Warn("SextantTestRig", "Test warn message");
            Logger.Error("SextantTestRig", "Test error message");
            Logger.Fatal("SextantTestRig", "Test fatal message");

            var datadog = new DatadogReporter(config);
            var graphite = new GraphiteReporter(config);

            Metrics.AddReporter(datadog);
            Metrics.AddReporter(graphite);

            Metrics.AddGauge("SextantTestRig.Gauge", 10);
            Metrics.AddCounter("SextantTestRig.Counter", 10);
            Metrics.AddMeter("SextantTestRig.Meter", 10);

            Console.WriteLine("press enter to quit.");
            Console.ReadLine();
        }
    }
}
