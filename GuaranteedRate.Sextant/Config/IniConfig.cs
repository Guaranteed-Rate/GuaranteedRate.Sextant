using EllieMae.Encompass.BusinessObjects;
using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.Config
{
    /**
     * IniConfig is a simple config class that expects data in the old .INI format:
     * [key]=[value]\n
     * 
     * Lines starting with # will be treated as comments
     * Empty lines will be ignored.
     */
    public class IniConfig : IEncompassConfig
    {
        private volatile IDictionary<string, string> _config;
        private readonly string _fileName;

        public IniConfig(string filename)
        {
            this._fileName = filename;
        }

        public bool Init(Session session)
        {
            return DoLoad(session);
        }

        public bool Reload(Session session)
        {
            return DoLoad(session);
        }

        private bool DoLoad(Session session)
        {
            IDictionary<string, string> config = new Dictionary<string, string>();
            DataObject data = session.DataExchange.GetCustomDataObject(_fileName);
            if (data == null)
            {
                return false;
            }
            string allAsString = ASCIIEncoding.ASCII.GetString(data.Data);
            if (allAsString == null || allAsString.Length <= 0)
            {
                return false;
            }
            string[] lines = allAsString.Split('\n');
            foreach (string line in lines)
            {
                if (line != null && !String.IsNullOrWhiteSpace(line) && line.Substring(0, 1) != "#")
                {
                    string[] keyVal = line.Split('=');
                    if (keyVal != null && keyVal.Length == 2)
                    {
                        config.Add(keyVal[0].ToLower().Trim(), keyVal[1].ToLower().Trim());
                    }
                }
            }

            _config = config;
            return true;
        }

        public string GetValue(string key, string defaultVal = null)
        {
            if (_config == null || String.IsNullOrWhiteSpace(key))
            {
                return defaultVal;
            }
            try
            {
                string retVal = null;
                if (_config.TryGetValue(key.ToLower(), out retVal))
                {
                    return retVal;
                } 
                else 
                {
                    return defaultVal;
                }
            }
            catch
            {
                return defaultVal;
            }
        }

        public ICollection<string> GetKeys()
        {
            if (_config == null)
            {
                return null;
            }
            return _config.Keys;
        }
    }
}
