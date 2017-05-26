using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace GuaranteedRate.Sextant.Logging.Elasticsearch
{
    public class ElasticsearchLogger
    {
        private Uri node = null;
        private ConnectionSettings settings = null;
        private ElasticClient client = null;

        public ElasticsearchLogger()
        {
            node = new Uri("https://elasticsearch.gr-dev.com:9200");
            settings = new ConnectionSettings(node);
            client = new ElasticClient(settings);
             
        }
        public async Task<bool> Log(IDictionary<string, string> fields, string loggerName, string level)
        {

            var response = await client.IndexAsync(fields,
                idx =>
                    idx.Index(
                        $"{loggerName}-{DateTime.UtcNow.ToString("yyyy-MM-dd")}"));
            return response.Created;
        }
    }

}
