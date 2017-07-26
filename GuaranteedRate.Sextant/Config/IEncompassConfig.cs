using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuaranteedRate.Sextant.Config
{
    public interface IEncompassConfig
    {
        bool Init(Session session);

        bool Init(string configAsString);
        bool Reload(Session session);

        /// <summary>
        /// Get value of key, return default if key does not exist
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="defaultValue">The default if the key does not exist</param>
        /// <returns>The value of the key or the default.</returns>
        string GetValue(string key, string defaultValue = null);
        bool GetValue(string key, bool defaultValue);
        int GetValue(string key, int defaultValue);

        ICollection<string> GetKeys();
        IEncompassConfig GetConfigGroup(string key);
    }
}