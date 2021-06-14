using System;
using System.Collections.Generic;
using System.Configuration;
using EllieMae.Encompass.Client;
using GuaranteedRate.Sextant.Config;

namespace GuaranteedRate.Sextant.Integration.Core
{
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

        public static T SafeGetValue<T>(string key, T defaultValue, bool errorOnWrongType = false)
        {
            // HARRY - CHANGE THIS TO DEFAULT TO OLD STUFF!
            var result = System.Configuration.ConfigurationManager.AppSettings.Get(key);
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

        public T GetValue<T>(string key, T defaultValue = default(T)) => GetValue<T>(key, defaultValue, null);

        public bool Init(string orgId, Session session) => throw new NotImplementedException();

		public bool Init(string orgId, string configAsString) => throw new NotImplementedException();

		public bool Reload(string orgId, Session session) => throw new NotImplementedException();

		public string GetValue(string key, string defaultValue, string orgId = null)
        {
            // HARRY - CHANGE THIS TO DEFAULT TO OLD STUFF!
            var value = ConfigurationManager.AppSettings[Keyname(key, orgId)];

            if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(defaultValue)) return defaultValue;

            return value;
        }

		public bool GetValue(string key, bool defaultValue, string orgId = null)
        {
            // HARRY - CHANGE THIS TO DEFAULT TO OLD STUFF!
            var value = ConfigurationManager.AppSettings[Keyname(key, orgId)];

            bool retVal;

            if (bool.TryParse(value, out retVal))
            {
                return retVal;
            }

            return defaultValue;
        }

		public int GetValue(string key, int defaultValue, string orgId = null)
        {
            // HARRY - CHANGE THIS TO DEFAULT TO OLD STUFF!
            var value = ConfigurationManager.AppSettings[Keyname(key,orgId)];

            int retVal;

            if (int.TryParse(value, out retVal))
            {
                return retVal;
            }

            return defaultValue;
        }

		public bool SwitchToOrgId(string orgId)
		{
            _orgId = orgId;
            return true;
		}

        private string Keyname(string key, string orgId = null) => $"{orgId ?? _orgId}{((orgId ?? _orgId) == null ? "" : ".")}{key}";

		public T GetValue<T>(string key, T defaultValue = default, string orgId = null)
        {
            return SafeGetValue(key, defaultValue);
        }
	}
}
