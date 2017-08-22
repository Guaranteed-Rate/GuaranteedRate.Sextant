using System;
using System.Collections.Generic;
using System.Linq;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.Metrics.Datadog;
using GuaranteedRate.Sextant.Metrics.Graphite;
using Metrics;
using Metrics.Graphite;
using Metrics.NET.Datadog;

namespace GuaranteedRate.Sextant.Metrics
{
    public class StatsDMetrics
    {
        private static MetricTags? _globalTags;
        private static string STATSD_METRICS_TAGS = "StatsDMetrics.Tags";

        public static void Setup(IEncompassConfig config)
        {
            var metricConfig = Metric.Config;
            
            var statsdTags = config.GetValue(STATSD_METRICS_TAGS, "");
            var globalTags = TrimTags(statsdTags);
            
            var datadogEnabled = config.GetValue(DatadogReporter.DATADOG_ENABLED, false);
            if (datadogEnabled)
            {
                var datadogTags = TrimTags(config.GetValue(DatadogReporter.DATADOG_TAGS));

                if (datadogTags.Any())
                {
                    globalTags.AddRange(datadogTags);
                }

                metricConfig.WithReporting(
                    report =>
                        report.WithDatadog(config.GetValue(DatadogReporter.DATADOG_APIKEY),
                            Environment.MachineName,
                            config.GetValue(DatadogReporter.DATADOG_ROOTNAMESPACE),
                            TimeSpan.FromSeconds(1)));
            }

            var graphiteEnabled = config.GetValue(GraphiteReporter.GRAPHITE_ENABLED, false);
            if (graphiteEnabled)
            {
                var grapiteTags = TrimTags(config.GetValue(GraphiteReporter.GRAPHITE_TAGS));

                if (grapiteTags.Any())
                {
                    globalTags.AddRange(grapiteTags);
                }

                var gs = new TcpGraphiteSender(config.GetValue(GraphiteReporter.GRAPHITE_HOST), config.GetValue(GraphiteReporter.GRAPHITE_PORT, 0));
                var gr = new StatsDGraphiteReport(gs, config.GetValue(GraphiteReporter.GRAPHITE_ROOT_NAMESPACE, string.Empty));
                
                metricConfig.WithReporting(report => report.WithReport(gr, TimeSpan.FromSeconds(1)));
            }

            if (globalTags.Any())
            {
                _globalTags = new MetricTags(globalTags);
            }
        }

        private static List<string> TrimTags(string tags)
        {
            if (string.IsNullOrEmpty(tags))
            {
                return new List<string>();
            }

            var trimmedTags = tags
                                .Split(',')
                                .Select(s => s.Trim())
                                .Where(s => !string.IsNullOrEmpty(s))
                                .ToList();

            return trimmedTags;
        }

        //
        // Summary:
        //     A counter is a simple incrementing and decrementing 64-bit integer. Ex number
        //     of active requests.
        //
        // Parameters:
        //   name:
        //     Name of the metric. Must be unique across all counters in this context.
        //
        //   unit:
        //     Description of what the is being measured ( Unit.Requests , Unit.Items etc )
        //     .
        //
        //   tags:
        //     Optional set of tags that can be associated with the metric. Tags can be string
        //     array or comma separated values in a string. ex: tags: "tag1,tag2" or tags: new[]
        //     {"tag1", "tag2"}
        //
        // Returns:
        //     Reference to the metric
        public static Counter Counter(string name, Unit unit, MetricTags tags = default(MetricTags))
        {
            var tagList = new List<string>();
            var metricTags = tags;

            if (_globalTags.HasValue)
            {
                tagList.AddRange(_globalTags.Value.Tags);
            }

            if (tags.Tags.Any())
            {
                tagList.AddRange(tags.Tags);
            }

            if (tagList.Any())
            {
                metricTags = new MetricTags(tagList);
            }

            return Metric.Counter(name, unit, metricTags);
        }

        //
        // Summary:
        //     A gauge is the simplest metric type. It just returns a value. This metric is
        //     suitable for instantaneous values.
        //
        // Parameters:
        //   name:
        //     Name of this gauge metric. Must be unique across all gauges in this context.
        //
        //   valueProvider:
        //     Function that returns the value for the gauge.
        //
        //   unit:
        //     Description of want the value represents ( Unit.Requests , Unit.Items etc ) .
        //
        //   tags:
        //     Optional set of tags that can be associated with the metric.
        //
        // Returns:
        //     Reference to the gauge
        public static void Gauge(string name, Func<double> valueProvider, Unit unit,
            MetricTags tags = default(MetricTags))
        {
            var tagList = new List<string>();
            var metricTags = tags;

            if (_globalTags.HasValue)
            {
                tagList.AddRange(_globalTags.Value.Tags);
            }

            if (tags.Tags.Any())
            {
                tagList.AddRange(tags.Tags);
            }

            if (tagList.Any())
            {
                metricTags = new MetricTags(tagList);
            }

            Metric.Gauge(name, valueProvider, unit, metricTags);
        }

        //
        // Summary:
        //     A Histogram measures the distribution of values in a stream of data: e.g., the
        //     number of results returned by a search.
        //
        // Parameters:
        //   name:
        //     Name of the metric. Must be unique across all histograms in this context.
        //
        //   unit:
        //     Description of what the is being measured ( Unit.Requests , Unit.Items etc )
        //     .
        //
        //   samplingType:
        //     Type of the sampling to use (see SamplingType for details ).
        //
        //   tags:
        //     Optional set of tags that can be associated with the metric.
        //
        // Returns:
        //     Reference to the metric
        public static Histogram Histogram(string name, Unit unit, SamplingType samplingType = SamplingType.Default,
            MetricTags tags = default(MetricTags))
        {
            var tagList = new List<string>();
            var metricTags = tags;

            if (_globalTags.HasValue)
            {
                tagList.AddRange(_globalTags.Value.Tags);
            }

            if (tags.Tags.Any())
            {
                tagList.AddRange(tags.Tags);
            }

            if (tagList.Any())
            {
                metricTags = new MetricTags(tagList);
            }

            return Metric.Histogram(name, unit, samplingType, metricTags);
        }

        //
        // Summary:
        //     A meter measures the rate at which a set of events occur, in a few different
        //     ways. This metric is suitable for keeping a record of now often something happens
        //     ( error, request etc ).
        //
        // Parameters:
        //   name:
        //     Name of the metric. Must be unique across all meters in this context.
        //
        //   unit:
        //     Description of what the is being measured ( Unit.Requests , Unit.Items etc )
        //     .
        //
        //   rateUnit:
        //     Time unit for rates reporting. Defaults to Second ( occurrences / second ).
        //
        //   tags:
        //     Optional set of tags that can be associated with the metric.
        //
        // Returns:
        //     Reference to the metric
        //
        // Remarks:
        //     The mean rate is the average rate of events. It’s generally useful for trivia,
        //     but as it represents the total rate for your application’s entire lifetime (e.g.,
        //     the total number of requests handled, divided by the number of seconds the process
        //     has been running), it doesn’t offer a sense of recency. Luckily, meters also
        //     record three different exponentially-weighted moving average rates: the 1-, 5-,
        //     and 15-minute moving averages.
        public static Meter Meter(string name, Unit unit, TimeUnit rateUnit = TimeUnit.Seconds,
            MetricTags tags = default(MetricTags))
        {
            var tagList = new List<string>();
            var metricTags = tags;

            if (_globalTags.HasValue)
            {
                tagList.AddRange(_globalTags.Value.Tags);
            }

            if (tags.Tags.Any())
            {
                tagList.AddRange(tags.Tags);
            }

            if (tagList.Any())
            {
                metricTags = new MetricTags(tagList);
            }

            return Metric.Meter(name, unit, rateUnit, metricTags);
        }

        //
        // Summary:
        //     Register a performance counter as a Gauge metric.
        //
        // Parameters:
        //   name:
        //     Name of this gauge metric. Must be unique across all gauges in this context.
        //
        //   counterCategory:
        //     Category of the performance counter
        //
        //   counterName:
        //     Name of the performance counter
        //
        //   counterInstance:
        //     Instance of the performance counter
        //
        //   unit:
        //     Description of want the value represents ( Unit.Requests , Unit.Items etc ) .
        //
        //   tags:
        //     Optional set of tags that can be associated with the metric.
        //
        // Returns:
        //     Reference to the gauge
        public static void PerformanceCounter(string name, string counterCategory, string counterName,
            string counterInstance, Unit unit, MetricTags tags = default(MetricTags))
        {
            var tagList = new List<string>();
            var metricTags = tags;

            if (_globalTags.HasValue)
            {
                tagList.AddRange(_globalTags.Value.Tags);
            }

            if (tags.Tags.Any())
            {
                tagList.AddRange(tags.Tags);
            }

            if (tagList.Any())
            {
                metricTags = new MetricTags(tagList);
            }

            Metric.PerformanceCounter(name, counterCategory, counterName, counterInstance, unit, metricTags);
        }

        //
        // Summary:
        //     A timer is basically a histogram of the duration of a type of event and a meter
        //     of the rate of its occurrence. Metrics.Metric.Histogram(System.String,Metrics.Unit,Metrics.SamplingType,Metrics.MetricTags)
        //     and Metrics.Metric.Meter(System.String,Metrics.Unit,Metrics.TimeUnit,Metrics.MetricTags)
        //
        // Parameters:
        //   name:
        //     Name of the metric. Must be unique across all timers in this context.
        //
        //   unit:
        //     Description of what the is being measured ( Unit.Requests , Unit.Items etc )
        //     .
        //
        //   samplingType:
        //     Type of the sampling to use (see SamplingType for details ).
        //
        //   rateUnit:
        //     Time unit for rates reporting. Defaults to Second ( occurrences / second ).
        //
        //   durationUnit:
        //     Time unit for reporting durations. Defaults to Milliseconds.
        //
        //   tags:
        //     Optional set of tags that can be associated with the metric.
        //
        // Returns:
        //     Reference to the metric
        public static Timer Timer(string name, Unit unit, SamplingType samplingType = SamplingType.Default,
            TimeUnit rateUnit = TimeUnit.Seconds, TimeUnit durationUnit = TimeUnit.Milliseconds,
            MetricTags tags = default(MetricTags))
        {
            var tagList = new List<string>();
            var metricTags = tags;

            if (_globalTags.HasValue)
            {
                tagList.AddRange(_globalTags.Value.Tags);
            }

            if (tags.Tags.Any())
            {
                tagList.AddRange(tags.Tags);
            }

            if (tagList.Any())
            {
                metricTags = new MetricTags(tagList);
            }

            return Metric.Timer(name, unit, samplingType, rateUnit, durationUnit, metricTags);
        }
    }
}
