using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using EllieMae.Encompass.Client;
using Newtonsoft.Json.Linq;

namespace GuaranteedRate.Sextant.Config
{
    /// <summary>
    /// Provides a simple config field parser for JSON files.  Defaults to ASCII encoding
    /// We use a standard key-value style, with an option for an orgId override for multi-tenancy.
    /// </summary>
    /// <remarks>orgId1.key1 takes precedent over key1</remarks>
    /// <example>Expects the following JSON format:
    /// <code>
    /// {
    ///   "key1":"Default Value",
    ///   "orgId1": {
    ///     "key1":"Value Override for OrgId1"
    ///   },
    ///   "orgId2": {
    ///     "key1":"Value Override for OrgId2"
    ///   }
    /// }
    /// </code>
    /// </example>
    public class JsonEncompassConfig : IJsonEncompassConfig
    {
        private JObject _jsonObject = null;
        private string _orgId = null;
        private Encoding _encoding = Encoding.ASCII;
        private string _configPath = "Sextant.json";

        /// <summary>
        /// Returns all keys in the json config.  
        /// </summary>
        /// <returns>String collection of keys</returns>
        public ICollection<string> GetKeys() => _jsonObject.Descendants().Select(aa => aa.Path).Distinct().ToList();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public JsonEncompassConfig() { }

        /// <summary>
        /// Returns the value of the given key
        /// </summary>
        /// <param name="key">Full path to the key(e.g. "MyKey" or "Keys[0].MySubKey")</param>
        /// <param name="defaultVal">Default value if the key is null</param>
        /// <returns></returns>
        public bool GetValue(string key, bool defaultVal) => GetValue<bool>(key, defaultVal);

        /// <inheritdoc/>
        public bool GetValue(string key, bool defaultVal, string orgId) => GetValue<bool>(key, defaultVal, orgId);

        /// <inheritdoc/>
        public int GetValue(string key, int defaultValue) => GetValue<int>(key, defaultValue);

        /// <inheritdoc/>
        public int GetValue(string key, int defaultValue, string orgId) => GetValue<int>(key, defaultValue, orgId);

        /// <summary>
        /// Returns the value of the given key
        /// </summary>
        /// <param name="key">Full path to the key(e.g. "MyKey" or "Keys[0].MySubKey")</param>
        /// <param name="defaultVal">Default value if the key is null</param>
        /// <returns></returns>
        public string GetValue(string key, string defaultVal = null) => GetValue<string>(key, defaultVal);

        /// <summary>
        /// Returns the value of the given key
        /// </summary>
        /// <param name="key">Full path to the key(e.g. "MyKey" or "Keys[0].MySubKey")</param>
        /// <param name="defaultVal">Default value if the key is null</param>
        /// <param name="orgId">The organization ID to pull from, if passed</param>
        /// <returns>The string value</returns>
        public string GetValue(string key, string defaultVal, string orgId) => GetValue<string>(key, defaultVal, orgId);

        /// <summary>
        /// This takes a type of T, a key and an optional default value.
        /// If the json value assigned to the key is an empty string it 
        /// will return a null object or an empty string depending on the 
        /// type of T
        /// </summary>
        /// <typeparam name="T">Object type to be returned</typeparam>
        /// <param name="key">Full path to the key(e.g. "Shape.Square.Color")</param>
        /// <param name="defaultValue">The value to be returned if the requested value is not found</param>
        /// <param name="orgId">The organization ID to pull from, if passed</param>
        /// <returns>A value of the requested type</returns>
        public T GetValue<T>(string key, T defaultValue, string orgId)
        {
            var token = _jsonObject.SelectToken(Keyname(key, orgId), false) ?? _jsonObject.SelectToken(key, false);
            try
            {
                if (ReferenceEquals(token, null))
                {
                    return defaultValue;
                }
                return token.ToObject<T>();
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// This takes a type of T, a key and an optional default value.
        /// If the json value assigned to the key is an empty string it 
        /// will return a null object or an empty string depending on the 
        /// type of T
        /// </summary>
        /// <typeparam name="T">Object type to be returned</typeparam>
        /// <param name="key">Full path to the key(e.g. "Shape.Square.Color")</param>
        /// <param name="defaultValue">The value to be returned if the requested value is not found</param>
        /// <returns>A value of the requested type</returns>
        public T GetValue<T>(string key, T defaultValue = default(T)) => GetValue<T>(key, defaultValue, null);

        /// <summary>
        /// Initializes the config from a default "sextant.json" file.
        /// </summary>
        /// <param name="session">Current Encompass session</param>
        /// <returns>true for success, throws exception otherwise</returns>
        public bool Init(Session session) => Init(session, _configPath);

        /// <summary>
        /// Reloads the config from a default "sextant.json" file.
        /// </summary>
        /// <param name="session">Current Encompass session</param>
        /// <returns>true for success, throws exception otherwise</returns>
        public bool Reload(Session session) => Reload(session, _configPath);

        /// <summary>
        /// Initializes the config from this file.
        /// </summary>
        /// <param name="session">Current Encompass session</param>
        /// <param name="configPath">Name of the config file (e.g. "myconfig.json")</param>
        public bool Init(Session session, string configPath) => Init(session, configPath, Encoding.ASCII);

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
                _configPath = configPath;
                var configText = session.DataExchange.GetCustomDataObject(_configPath);
                _jsonObject = JObject.Parse(configText.ToString(_encoding));
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Could not load config file {0}", configPath), ex);
            }
        }

        /// <summary>
        /// Reloads the config file
        /// </summary>
        /// <param name="session">Current Encompass session</param>
        /// <param name="configPath">Name of the config file e.g. "foo.json"</param>
        /// <returns>true for success, throws exception otherwise</returns>
        public bool Reload(Session session, string configPath) => Init(session, configPath);


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

            var config = new JsonEncompassConfig();
            config.Init(val.ToString());

            return config;
        }

        /// <summary>
        /// Loads the config from the given string.  Useful for testing.
        /// </summary>
        /// <param name="configAsString">The json string</param>
        /// <returns></returns>
        public bool Init(string configAsString)
        {
            try
            {
                _jsonObject = JObject.Parse(configAsString);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot parse config: {ex}");
            }
        }

        /// <inheritdoc/>
		public bool Init(string orgId, Session session) => Init(session) && SwitchToOrgId(orgId);

        /// <inheritdoc/>
		public bool Init(string orgId, string configAsString) => Init(configAsString) && SwitchToOrgId(orgId);

        /// <inheritdoc/>
		public bool Reload(string orgId, Session session) => Reload(session) && SwitchToOrgId(orgId);

        /// <inheritdoc/>
		public bool SwitchToOrgId(string orgId)
        {
            _orgId = orgId;
            return _jsonObject != null;
        }

        /// <summary>
        /// Returns the key name with an orgId if it's passed, or if it's currently initialized through one of the switching functions.
        /// </summary>
        /// <param name="key">The config key</param>
        /// <param name="orgId">An optional orgId override.</param>
        /// <returns>The key name with an orgId prefix if needed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string Keyname(string key, string orgId = null)
        {
            var prefix = orgId ?? _orgId;
            return $"{prefix}{(prefix == null ? "" : ".")}{key}";
        }
    }
}