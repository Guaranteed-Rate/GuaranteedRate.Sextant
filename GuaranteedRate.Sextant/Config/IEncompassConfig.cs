using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuaranteedRate.Sextant.Config
{
    /// <summary>
    /// Standard Encompass Config.
    /// </summary>
    public interface IEncompassConfig
    {
        /// <summary>
        /// Initialize the config, using a session
        /// </summary>
        /// <param name="session">Encompass Session</param>
        /// <returns>Success or Failure</returns>
        bool Init(Session session);

        /// <summary>
        /// Initialize the config, using a session, given an orgId
        /// </summary>
        /// <param name="orgId">Organization ID. Should be a number.</param>
        /// <param name="session">Encompass Session</param>
        /// <returns>Success or Failure</returns>
        bool Init(string orgId, Session session);

        /// <summary>
        /// Initialize the config, given a json string.
        /// </summary>
        /// <param name="configAsString">JSON, as a string.</param>
        /// <returns>Success or Failure</returns>
        bool Init(string configAsString);

        /// <summary>
        /// Initialize the config, given a json string.
        /// </summary>
        /// <param name="orgId">Organization ID. Should be a number.</param>
        /// <param name="configAsString">JSON, as a string.</param>
        /// <returns>Success or Failure</returns>
        bool Init(string orgId, string configAsString);

        /// <summary>
        /// Reload the configuration value
        /// </summary>
        /// <param name="session">Encompass Session</param>
        /// <returns>Success or Failure</returns>
        bool Reload(Session session);

        /// <summary>
        /// Reload the configuration value
        /// </summary>
        /// <param name="orgId">Organization ID. Should be a number.</param>
        /// <param name="session">Encompass Session</param>
        /// <returns>Success or Failure</returns>
        bool Reload(string orgId, Session session);

        /// <summary>
        /// Get value of key, return default if key does not exist
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="defaultValue">The default if the key does not exist</param>
        /// <returns>The value of the key or the default.</returns>
        string GetValue(string key, string defaultValue = null);


        /// <summary>
        /// Get value of key, return default if key does not exist
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="defaultValue">The default if the key does not exist</param>
        /// <param name="orgId">Get the value for an orgId, regardless of the current configured orgId</param>
        /// <returns>The value of the key or the default.</returns>
        string GetValue(string key, string defaultValue, string orgId = null);

        /// <summary>
        /// Get value of key, return default if key does not exist
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="defaultValue">The default if the key does not exist</param>
        /// <returns>The value of the key or the default.</returns>
        bool GetValue(string key, bool defaultValue);

        /// <summary>
        /// Get value of key, return default if key does not exist
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="defaultValue">The default if the key does not exist</param>
        /// <param name="orgId">Get the value for an orgId, regardless of the current configured orgId</param>
        /// <returns>The value of the key or the default.</returns>
        bool GetValue(string key, bool defaultValue, string orgId = null);

        /// <summary>
        /// Get value of key, return default if key does not exist
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="defaultValue">The default if the key does not exist</param>
        /// <returns>The value of the key or the default.</returns>
        int GetValue(string key, int defaultValue);

        /// <summary>
        /// Get value of key, return default if key does not exist
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="defaultValue">The default if the key does not exist</param>
        /// <param name="orgId">Get the value for an orgId, regardless of the current configured orgId</param>
        /// <returns>The value of the key or the default.</returns>
        int GetValue(string key, int defaultValue, string orgId = null);

        /// <summary>
        /// Get all Keys in the dictionary.
        /// </summary>
        /// <returns>A collection of keys</returns>
        ICollection<string> GetKeys();

        /// <summary>
        /// Get the Encompass Config for a Config Group.
        /// </summary>
        /// <param name="key">The Config Group, such as a plugin name.</param>
        /// <returns>An Encompass Config.</returns>
        IEncompassConfig GetConfigGroup(string key);

        /// <summary>
        /// Switches to the indicated orgId for config purposes.
        /// </summary>
        /// <param name="orgId">Organization ID. Should be a stringified number. Pass null to fall back to the default.</param>
        /// <returns>Success or Failure.</returns>
        bool SwitchToOrgId(string orgId);
    }
}