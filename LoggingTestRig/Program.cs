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
using Metrics;

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

            #region Simple Metrics 

            var datadog = new DatadogReporter(config);
            var graphite = new GraphiteReporter(config);

            SimpleMetrics.AddReporter(datadog);
            SimpleMetrics.AddReporter(graphite);

            SimpleMetrics.AddGauge("SextantTestRig.Gauge", 10);
            SimpleMetrics.AddCounter("SextantTestRig.Counter", 10);
            SimpleMetrics.AddMeter("SextantTestRig.Meter", 10);

            #endregion

            #region StatsD Metrics

            StatsDMetrics.Setup(config);

            var timer = StatsDMetrics.Timer("sextant-statd-tests-timer", Unit.Calls);
            Random r = new Random();
            
            timer.StartRecording();

            var counter = StatsDMetrics.Counter("sextant-statd-tests-counter", Unit.Events, MetricTags.None);
            counter.Increment(r.Next(0, 100));
            counter.Increment(r.Next(0, 10));
            counter.Increment(r.Next(0, 10));
            counter.Increment(r.Next(0, 10));
            counter.Increment(r.Next(0, 10));
            counter.Increment(r.Next(0, 10));
            counter.Increment(r.Next(0, 10));
            counter.Increment(r.Next(0, 10));
            
            timer.EndRecording();

            #endregion

            Console.WriteLine("press enter to quit.");
            Console.ReadLine();
        }
    }
}
