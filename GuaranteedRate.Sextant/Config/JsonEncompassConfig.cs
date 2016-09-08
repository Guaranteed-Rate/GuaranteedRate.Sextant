using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EllieMae.Encompass.Client;
using Newtonsoft.Json.Linq;

namespace GuaranteedRate.Sextant.Config
{
    public class JsonEncompassConfig:IEncompassConfig
    {
        private JObject _jsonObject = null;

        public ICollection<string> GetKeys()
        {
          return   _jsonObject.Properties().Select(a => a.Name).ToList();
        }

        public bool GetValue(string key, bool defaultValue)
        {
            return Boolean.Parse(GetValue(key,defaultValue.ToString()));
        }

        public string GetValue(string key, string defaultVal = null)
           {
            var val =  _jsonObject.Properties().FirstOrDefault(aa => aa.Name == key);

            if (val == null)
            {
                return defaultVal;
            }
            return val.ToString();
        }

        public bool Init(Session session)
        {
            return Init(session,"Sextant.json");
        }

        public bool Reload(Session session)
        {
            return Reload(session, "Sextant.json");
        }

        public bool Init(Session session, string configPath)
        {
            try
            {
                var configText = session.DataExchange.GetCustomDataObject(configPath);
                _jsonObject= new JObject(configText);
                return false;
            }
            catch (Exception)
            {
                    
                throw new Exception($"Could not load config file {configPath}");
            }

        }

        public bool Reload(Session session, string configPath)
        {
            return Init(session, configPath);

        }
    }
}
