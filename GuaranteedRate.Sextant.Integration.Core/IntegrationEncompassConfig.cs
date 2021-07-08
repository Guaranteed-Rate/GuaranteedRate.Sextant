using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;
using EllieMae.Encompass.Client;
using GuaranteedRate.Sextant.Config;

namespace GuaranteedRate.Sextant.Integration.Core
{
    /// <summary>
    /// Supports the standard app.config values for integration testing.
    /// Allows an override by passing an OrgId for multitenancy.
    /// </summary>
    /// <example>
    /// Expects a standard key value pair in app.config
    /// <code>
    /// <appSettings>
    ///    <add key="key1" value="3" />
    ///    <add key="orgId1.key1" value="4" />
    /// <appSettings>
    /// </code>
    /// </example>
    public class IntegrationEncompassConfig : IJsonEncompassConfig
    {
        private string _orgId = null;

        public IntegrationEncompassConfig() { }

        public bool Init(EllieMae.Encompass.Client.Session session) => throw new NotImplementedException();

        public bool Init(string configAsString) => throw new NotImplementedException();

        public bool Reload(EllieMae.Encompass.Client.Session session) => throw new NotImplementedException();

        public string GetValue(string key, string defaultValue = null) => GetValue(key, defaultValue, null);

        public bool GetValue(string key, bool defaultValue) => GetValue(key, defaultValue, null);

        public int GetValue(string key, int defaultValue) => GetValue(key, defaultValue, null);

        public ICollection<string> GetKeys() => throw new NotImplementedException();

        public IEncompassConfig GetConfigGroup(string key) => throw new NotImplementedException();

        /// <inheritdoc/>
        public T SafeGetValue<T>(string key, T defaultValue, bool errorOnWrongType = false, string orgId = null)
        {
            var result = ConfigurationManager.AppSettings.Get(Keyname(key, orgId)) ?? ConfigurationManager.AppSettings.Get(key);
            if (string.IsNullOrEmpty(result))
            {
                return defaultValue;
            }

            if (result is T)
            {
                return (T)Convert.ChangeType(result, typeof(T));
            }
            //try casting anyway...
            try
            {
                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (Exception)
            {
                if (errorOnWrongType)
                {
                    throw new Exception(string.Format("Problem fetching {0} as {1}.  It's a {2}", key, typeof(T).Name,
                        result.GetType().Name));
                }
                return defaultValue;
            }
        }

        /// <inheritdoc/>
        public T GetValue<T>(string key, T defaultValue = default(T)) => GetValue<T>(key, defaultValue, null);

        /// <inheritdoc/>
        public bool Init(string orgId, Session session) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool Init(string orgId, string configAsString) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool Reload(string orgId, Session session) => throw new NotImplementedException();

        /// <inheritdoc/>
        public string GetValue(string key, string defaultValue, string orgId)
        {
            var value = ConfigurationManager.AppSettings[Keyname(key, orgId)] ?? ConfigurationManager.AppSettings[key];

            if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(defaultValue)) return defaultValue;

            return value;
        }

		public bool GetValue(string key, bool defaultValue, string orgId)
        {
            var value = ConfigurationManager.AppSettings[Keyname(key, orgId)] ?? ConfigurationManager.AppSettings[key];

            bool retVal;

            if (bool.TryParse(value, out retVal))
            {
                return retVal;
            }

            return defaultValue;
        }

        /// <inheritdoc/>
        public int GetValue(string key, int defaultValue, string orgId)
        {
            var value = ConfigurationManager.AppSettings[Keyname(key, orgId)] ?? ConfigurationManager.AppSettings[key];

            int retVal;

            if (int.TryParse(value, out retVal))
            {
                return retVal;
            }

            return defaultValue;
        }

        /// <inheritdoc/>
		public bool SwitchToOrgId(string orgId)
		{
            _orgId = orgId;
            return true;
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

        /// <inheritdoc/>
        public T GetValue<T>(string key, T defaultValue, string orgId) =>
            SafeGetValue(key, defaultValue, false, orgId);

        /// <inheritdoc/>
        public Func<T> GetValueFunction<T>(string key, T defaultValue = default)
            => () => GetValue(key, defaultValue);

        /// <inheritdoc/>
        public Func<T> GetValueFunction<T>(string key, T defaultValue, string orgId)
            => () => GetValue(key, defaultValue, orgId);

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
