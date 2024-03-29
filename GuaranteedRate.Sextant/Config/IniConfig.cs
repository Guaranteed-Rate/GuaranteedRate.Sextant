﻿using EllieMae.Encompass.BusinessObjects;
using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GuaranteedRate.Sextant.Config
{
    /// <summary>
    /// IniConfig is a simple config class that expects data in the old .INI format:
    /// [key]=[value]\n
    /// 
    /// Lines starting with # will be treated as comments
    /// Empty lines will be ignored.
    /// 
    /// Org1.Key1 will override Key1 if an Org Id is passed in, or activated through a switching function.
    ///
    /// </summary>
    /// <remarks>
    /// This class is considered obsolete, and you should be using the JsonEncompassConfig where possible.
    /// </remarks>
    /// <example>
    /// Expects the code in the form of:
    /// Key1 = Default value
    /// Org1.Key1 = Org1 value override
    /// Org2.Key1 = Org2 value override
    /// </example>
    public class IniConfig : IEncompassConfig
    {
        private volatile IDictionary<string, string> _config;
        private readonly string _fileName;
        private string _orgId = null;

        public IniConfig(string filename)
        {
            this._fileName = filename;
        }

        public bool Init(Session session) => DoLoad(session);

        public bool Reload(Session session) => DoLoad(session);

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

        public string GetValue(string key, string defaultValue = null) => GetValue(key, defaultValue, null);

        public int GetValue(string key, int defaultValue) => GetValue(key, defaultValue, null);

        public bool GetValue(string key, bool defaultValue) => GetValue(key, defaultValue, null);

        public ICollection<string> GetKeys()
        {
            return _config?.Keys;
        }

        /// <summary>
        /// Do not use. Always returns null in the iniConfig implementation
        /// </summary>
        /// <param name="key"></param>
        /// <returns>null.  Always.</returns>
        public IEncompassConfig GetConfigGroup(string key) => null;

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
            return _config != null;
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
            return $"{prefix}{(prefix == null ? "" : ".")}{key.ToLower()}";
        }

        public string GetValue(string key, string defaultValue, string orgId)
        {
            if (_config == null || string.IsNullOrWhiteSpace(key))
            {
                return defaultValue;
            }

            string orgRetVal;
            string retVal;
            var specific = _config.TryGetValue(Keyname(key, orgId), out orgRetVal)
                ? orgRetVal
                : null;
            var general = _config.TryGetValue(key.ToLower(), out retVal)
                ? retVal
                : defaultValue;
            return specific ?? general;
        }

        /// <inheritdoc/>
		public bool GetValue(string key, bool defaultValue, string orgId)
        {
            if (_config == null || string.IsNullOrWhiteSpace(key))
            {
                return defaultValue;
            }
            try
            {
                string stringVal, orgStringVal;
                bool retVal, orgRetVal;

                if (_config.TryGetValue(Keyname(key, orgId), out orgStringVal) && bool.TryParse(orgStringVal, out orgRetVal))
                {
                    return orgRetVal;
                }

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

		public int GetValue(string key, int defaultValue, string orgId)
        {
            if (_config == null || string.IsNullOrWhiteSpace(key))
            {
                return defaultValue;
            }
            try
            {

                string stringVal, orgStringVal;
                int retVal, orgRetVal;

                if (_config.TryGetValue(Keyname(key, orgId), out orgStringVal) && int.TryParse(orgStringVal, out orgRetVal))
                {
                    return orgRetVal;
                }

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

        /// <inheritdoc/>
        public Func<string> GetValueFunction(string key, string defaultValue = null)
            => () => GetValue(key, defaultValue);

        /// <inheritdoc/>
        public Func<string> GetValueFunction(string key, string defaultValue, string orgId)
            => () => GetValue(key, defaultValue, orgId);

        /// <inheritdoc/>
        public Func<bool> GetValueFunction(string key, bool defaultValue)
            => () => GetValue(key, defaultValue);

        /// <inheritdoc/>
        public Func<bool> GetValueFunction(string key, bool defaultValue, string orgId)
            => () => GetValue(key, defaultValue, orgId);

        /// <inheritdoc/>
        public Func<int> GetValueFunction(string key, int defaultValue)
            => () => GetValue(key, defaultValue);

        /// <inheritdoc/>
        public Func<int> GetValueFunction(string key, int defaultValue, string orgId)
            => () => GetValue(key, defaultValue, orgId);
    }
}