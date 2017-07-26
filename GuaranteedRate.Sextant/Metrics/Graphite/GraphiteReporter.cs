using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.WebClients;

namespace GuaranteedRate.Sextant.Metrics.Graphite
{
    public class GraphiteReporter : AsyncEventReporter, IReporter
    {
        private static string _prefix;
        private TcpClient _client;
        private readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #region config mappings

        public static string GRAPHITE_HOST = "GraphiteReporter.Host";
        public static string GRAPHITE_PORT = "GraphiteReporter.Port";
        public static string GRAPHITE_QUEUE_SIZE = "GraphiteReporter.QueueSize";
        public static string GRAPHITE_RETRY_LIMIT = "GraphiteReporter.RetryLimit";
        public static string GRAPHITE_ROOT_NAMESPACE = "GraphiteReporter.RootNamespace";
        public static string GRAPHITE_TRACK_HOSTMACHINE = "GraphiteReporter.TrackHostmachine";
        
        #endregion

        public GraphiteReporter(IEncompassConfig config)
            : base(config.GetValue(GRAPHITE_QUEUE_SIZE, 1000),
                config.GetValue(GRAPHITE_RETRY_LIMIT, 3))
        {
            Setup(config);
        }

        private void Setup(IEncompassConfig config)
        {
            _prefix = config.GetValue(GRAPHITE_ROOT_NAMESPACE, "");

            if (config.GetValue(GRAPHITE_TRACK_HOSTMACHINE, true))
            {
                _prefix = $"{_prefix}.{Environment.MachineName}";
            }

            var host = config.GetValue(GRAPHITE_HOST);
            if(string.IsNullOrEmpty(host)) throw new InvalidOperationException($"{GRAPHITE_HOST} is a required configuration");

            var port = config.GetValue(GRAPHITE_PORT, -1);
            if(port < 0) throw new InvalidOperationException($"{GRAPHITE_PORT} is a required configuration");

            _client = new TcpClient(host, port);
        }

        private long GetEpochTime()
        {
            return Convert.ToInt64((DateTime.Now.ToUniversalTime() - epoch).TotalSeconds);
        }

        public void AddGauge(string name, long value)
        {
            AddEvent(name, value);
        }

        public void AddMeter(string name, long value)
        {
            AddEvent(name, value);
        }

        public void AddCounter(string name, long value)
        {
            AddEvent(name, value);
        }

        private void AddEvent(string name, long value)
        {
            var gaugeEvent = $"{_prefix}.{name} {value} {GetEpochTime()}{Environment.NewLine}";
            ReportEvent(gaugeEvent);
        }

        protected override bool PostEvent(object data)
        {
            try
            {
                var stream = _client.GetStream();
                var bytes = System.Text.Encoding.ASCII.GetBytes(data as string);

                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        private bool _disposedValue = false;

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Shutdown();
                    _client.Close();
                }
            }

            Client = null;
            _client = null;

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
