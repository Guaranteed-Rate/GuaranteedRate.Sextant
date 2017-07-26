using System;
using System.Collections.Generic;
using System.Configuration;
using GuaranteedRate.Sextant.Config;

namespace GuaranteedRate.Sextant.Integration.Core
{
    public class IntegrationEncompassConfig : IEncompassConfig
    {
        public bool Init(EllieMae.Encompass.Client.Session session)
        {
            throw new NotImplementedException();
        }

        public bool Init(string configAsString)
        {
            throw new NotImplementedException();
        }

        public bool Reload(EllieMae.Encompass.Client.Session session)
        {
            throw new NotImplementedException();
        }

        public string GetValue(string key, string defaultVal = null)
        {
            var value = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(defaultVal)) return defaultVal;

            return value;
        }

        public bool GetValue(string key, bool defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];

            bool retVal;

            if (bool.TryParse(value, out retVal))
            {
                return retVal;
            }

            return defaultValue;
        }

        public int GetValue(string key, int defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];

            int retVal;

            if (int.TryParse(value, out retVal))
            {
                return retVal;
            }

            return defaultValue;
        }

        public ICollection<string> GetKeys()
        {
            throw new NotImplementedException();
        }

        public IEncompassConfig GetConfigGroup(string key)
        {
            throw new NotImplementedException();
        }
        public static T SafeGetValue<T>(string key, T defaultValue, bool errorOnWrongType = false)
        {
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
        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            return SafeGetValue(key, defaultValue);
        }
    }
}
