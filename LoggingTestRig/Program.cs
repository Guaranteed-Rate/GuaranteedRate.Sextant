using System;
using System.Threading.Tasks;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.Logging;
using GuaranteedRate.Sextant.Metrics;
using GuaranteedRate.Sextant.Metrics.Datadog;
using GuaranteedRate.Sextant.Metrics.Graphite;
using Metrics;

namespace LoggingTestRig
{
    /// <summary>
    /// this is essentially as scratch pad for testing logging.
    /// </summary>
    class Program
    {

        private static void Debug()
        {
            Logger.Debug("SextantTestRig", $"Test debug message from thread {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }


        static void Main(string[] args)
        {

            int count = 0;
            var config = new JsonEncompassConfig();
            config.Init(System.IO.File.ReadAllText("../../SextantConfigTest.json"));

            //manually set appenders    
            // var console = new ConsoleLogAppender(config);
            //var loggly = new LogglyLogAppender(config);
            //var elasticSearch = new ElasticsearchLogAppender(config);
            //Logger.AddAppender(console);
            //Logger.AddAppender(loggly);
            //Logger.AddAppender(elasticSearch);

            //automatically set appenders
            Logger.Setup(config);

            Logger.AddTag("runtime-tag");
            Logger.Debug("SextantTestRig", "Test debug message.");

            Logger.Info("SextantTestRig", "Test info message");
            Logger.Warn("SextantTestRig", "Test warn message");
            Logger.Error("SextantTestRig", "Test error message");
            Logger.Fatal("SextantTestRig", "Test fatal message");

            Console.WriteLine("press Q to quit or any other key to log another debug event.");

            while (Console.ReadKey().Key != ConsoleKey.Q)
            {
                var increment = 19;
                Parallel.For(0, increment, async => { Debug(); });
                count = count + increment;
            }
            Console.WriteLine($"total queued: {count}");
            Console.WriteLine("Shutting down.");
            Logger.Shutdown(30);
            return;
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
