using EllieMae.Encompass.BusinessObjects;
using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GuaranteedRate.Sextant.Config
{
    /// <summary>
    ///      IniConfig is a simple config class that expects data in the old .INI format:
    /// [key]=[value]\n
    /// 
    /// Lines starting with # will be treated as comments
    /// Empty lines will be ignored.
    ///
    /// </summary>
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
            try
            {
                DataObject data = session.DataExchange.GetCustomDataObject(_fileName);
                if (data == null)
                {
                    return false;
                }
                string allAsString = ASCIIEncoding.ASCII.GetString(data.Data);
                return Init(allAsString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot parse config: {ex.ToString()}");
            }
        }

        public string GetValue(string key, string defaultValue = null)
        {
            if (_config == null || string.IsNullOrWhiteSpace(key))
            {
                return defaultValue;
            }
            
            string retVal;
            return _config.TryGetValue(key.ToLower(), out retVal)
                ? retVal
                : defaultValue;
        }

        public int GetValue(string key, int defaultValue)
        {
            if (_config == null || string.IsNullOrWhiteSpace(key))
            {
                return defaultValue;
            }
            try
            {
                string stringVal;
                int retVal;
                if (_config.TryGetValue(key.ToLower(), out stringVal) && int.TryParse(stringVal, out retVal))
                {
                    return retVal;
                }

                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public bool GetValue(string key, bool defaultValue)
        {
            if (_config == null || string.IsNullOrWhiteSpace(key))
            {
                return defaultValue;
            }
            try
            {
                string stringVal;
                bool retVal;
                if (_config.TryGetValue(key.ToLower(), out stringVal) && bool.TryParse(stringVal, out retVal))
                {
                    return retVal;
                }

                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public ICollection<string> GetKeys()
        {
            return _config?.Keys;
        }

        /// <summary>
        /// Do not use.  Always returns null in the iniConfig impllementation
        /// </summary>
        /// <param name="key"></param>
        /// <returns>null.  Always.</returns>
        public IEncompassConfig GetConfigGroup(string key)
        {
            return null;
        }

        /// <summary>
        /// Loads the config from teh given string
        /// </summary>
        /// <param name="configAsString">The ASCII-encoded string</param>
        /// <returns></returns>
        public bool Init(string configAsString)
        {
            try
            {
                IDictionary<string, string> config = new Dictionary<string, string>();

                if (configAsString == null || configAsString.Length <= 0)
                {
                    return false;
                }
                string[] lines = configAsString.Split('\n');
                foreach (string line in lines)
                {
                    if (line != null && !String.IsNullOrWhiteSpace(line) && line.Substring(0, 1) != "#")
                    {
                        var keyVal = line.Split(new[] { '=' }, 2);
                        if (keyVal != null && keyVal.Length == 2)
                        {
                            config.Add(keyVal[0].ToLower().Trim(), keyVal[1].ToLower().Trim());
                        }
                    }
                }

                _config = config;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot parse config: {ex.ToString()}");
            }
        }
    }
}