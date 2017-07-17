using System.Collections.Generic;
using GuaranteedRate.Sextant.Config;

namespace GuaranteedRate.Sextant.Logging.Datadog
{
    public interface IDatadogLogAppender
    {
        bool ErrorEnabled { get; set; }
        bool WarnEnabled { get; set; }
        bool InfoEnabled { get; set; }
        bool DebugEnabled { get; set; }
        bool FatalEnabled { get; set; }
        string host { get; set; }
        void Setup(IEncompassConfig config);
        void Log(IDictionary<string, string> fields);
        void AddTag(string tag, string value);
        void AddCounter(string metric, long value);

        /// <summary>
        /// Send a metric to DataDog
        /// </summary>
        /// <param name="metric">name of metric e.g. HogsHeadsPerMile</param>
        /// <param name="value">value to send to DataDog  e.g. 67</param>
        void AddGauge(string metric, long value);

        /// <summary>
        /// deprecataed because misspelled.  Use AddGauge instead.
        /// </summary>
        /// <param name="metric">name of metric e.g. HogsHeadsPerMile</param>
        /// <param name="value">value to send to DataDog  e.g. 67</param>
        void AddGuage(string metric, long value);

        /// <summary>
        /// Send a meter to DataDog
        /// </summary>
        /// <param name="metric">name of metric e.g. HogsHeadsPerMile</param>
        /// <param name="value">value to send to DataDog  e.g. 67</param>
        void AddMeter(string metric, long value);

        void Shutdown();
        bool ReportEvent(object formattedData);
    }
}