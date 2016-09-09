using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EllieMae.Encompass.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GuaranteedRate.Sextant.Config
{
    /// <summary>
    /// provides a simple config fild parser for JSON files.  Defaults to ASCII encoding
    /// </summary>
    public class JsonEncompassConfig : IEncompassConfig
    {
        private JObject _jsonObject = null;
        private Encoding _encoding = Encoding.ASCII;

        /// <summary>
        /// Returns all keys in the json config.  
        /// </summary>
        /// <returns>String collection of keys</returns>
        public ICollection<string> GetKeys()
        {
            return _jsonObject.Descendants().Select(aa => aa.Path).Distinct().ToList();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public JsonEncompassConfig()
        {
        }

        /// <summary>
        /// Initializes the config from a fixed string.  Useful for testing.
        /// </summary>
        /// <param name="configText">The json text.</param>
        /// <returns>true for success, throws exception otherwise</returns>
        public JsonEncompassConfig(string configText)
        {
            _jsonObject = JObject.Parse(configText);
        }

        /// <summary>
        /// Returns the value of the given key
        /// </summary>
        /// <param name="key">Full path to the key(e.g. "MyKey" or "Keys[0].MySubKey")</param>
        /// <param name="defaultVal">Default value if the key is null</param>
        /// <returns></returns>
        public bool GetValue(string key, bool defaultVal)
        {
            return Boolean.Parse(GetValue(key, defaultVal.ToString()));
        }

        /// <summary>
        /// Returns the value of the given key
        /// </summary>
        /// <param name="key">Full path to the key(e.g. "MyKey" or "Keys[0].MySubKey")</param>
        /// <param name="defaultVal">Default value if the key is null</param>
        /// <returns></returns>
        public string GetValue(string key, string defaultVal = null)
        {
            var val = _jsonObject.SelectToken(key);

            if (val == null)
            {
                return defaultVal;
            }
            return val.ToString();
        }


        /// <summary>
        /// Initializes the config from a default "sextant.json" file.
        /// </summary>
        /// <param name="session">Current Encompass session</param>
        /// <returns>true for success, throws exception otherwise</returns>
        public bool Init(Session session)
        {
            return Init(session, "Sextant.json");
        }


        /// <summary>
        /// Reloads the config from a default "sextant.json" file.
        /// </summary>
        /// <param name="session">Current Encompass session</param>
        /// <returns>true for success, throws exception otherwise</returns>
        public bool Reload(Session session)
        {
            return Reload(session, "Sextant.json");
        }


        /// <summary>
        /// Initializes the config from this file.
        /// </summary>
        /// <param name="session">Current Encompass session</param>
        /// <param name="configPath">Name of the config file (e.g. "myconfig.json")</param>
        public bool Init(Session session, string configPath)
        {
            return Init(session, configPath, Encoding.ASCII);
        }

        /// <summary>
        /// Initializes the config from this file.
        /// </summary>
        /// <param name="session">Current Encompass session</param>
        /// <param name="configPath">Name of the config file (e.g. "myconfig.json")</param>
        /// <param name="encoding">Encoding of the file.</param>
        /// <returns>true for success, throws exception otherwise</returns>
        public bool Init(Session session, string configPath, Encoding encoding)
        {
            try
            {
                _encoding = encoding;
                var configText = session.DataExchange.GetCustomDataObject(configPath);
                _jsonObject = JObject.Parse(configText.ToString(_encoding));
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not load config file {configPath}", ex);
            }
        }

        /// <summary>
        /// Reloads the config file
        /// </summary>
        /// <param name="session">Current Encompass session</param>
        /// <param name="configPath">Name of the config file e.g. "foo.json"</param>
        /// <returns>true for success, throws exception otherwise</returns>
        public bool Reload(Session session, string configPath)
        {
            return Init(session, configPath);
        }


        /// <summary>
        /// Returns a sub-section of the config.
        /// </summary>
        /// <param name="key">Json key of the section you want.</param>
        /// <returns>A JsonEncompassConfig created from the subsection.</returns>
        public IEncompassConfig GetConfigGroup(string key)
        {
            var val = _jsonObject.SelectToken(key);

            if (val == null)
            {
                return null;
            }

            return new JsonEncompassConfig(val.ToString());
        }
    }
}