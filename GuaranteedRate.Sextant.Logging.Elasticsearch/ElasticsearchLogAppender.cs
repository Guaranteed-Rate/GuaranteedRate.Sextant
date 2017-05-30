using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuaranteedRate.Sextant.Config;
using Nest;

namespace GuaranteedRate.Sextant.Logging.Elasticsearch
{
    public class ElasticsearchLogAppender:ILogAppender
    {
        private Uri node = null;
        private ConnectionSettings settings = null;
        private ElasticClient client = null;

        public void Setup(IEncompassConfig config)
        {
            node = new Uri(config.GetValue("ElasticSearchLogAppender.Url"));
            settings = new ConnectionSettings(node);
            client = new ElasticClient(settings);
            DebugEnabled = config.GetValue("ElasticSearchLogAppender.Debug.Enabled", true);
            InfoEnabled = config.GetValue("ElasticSearchLogAppender.Info.Enabled", true);
            WarnEnabled = config.GetValue("ElasticSearchLogAppender.Warn.Enabled", true);
            ErrorEnabled = config.GetValue("ElasticSearchLogAppender.Error.Enabled", true);
            FatalEnabled = config.GetValue("ElasticSearchLogAppender.Fatal.Enabled", true);

        }

        public void Log(IDictionary<string, string> fields)
        {
            var loggerName = "undefined";
            if (fields.ContainsKey("logger"))
            {
                loggerName = fields["logger"];
            }
              client.Index(fields,
                idx =>
                    idx.Index(
                        $"{loggerName}-{DateTime.UtcNow.ToString("yyyy-MM-dd")}"));
        }
        public bool DebugEnabled { get; private set; }
        public bool InfoEnabled { get; private set; }
        public bool WarnEnabled { get; private set; }
        public bool ErrorEnabled { get; private set; }
        public bool FatalEnabled { get; private set; }
    }

}
