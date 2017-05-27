using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.Loggers;
using Nest;

namespace GuaranteedRate.Sextant.Logging.Elasticsearch
{
    public class ElasticsearchLogger:ILogReporter
    {
        private Uri node = null;
        private ConnectionSettings settings = null;
        private ElasticClient client = null;

      
       

        public void SetUp(IEncompassConfig config)
        {
            node = new Uri(config.GetValue("ElasticSearch.Url"));
            settings = new ConnectionSettings(node);
            client = new ElasticClient(settings);

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
    }

}
