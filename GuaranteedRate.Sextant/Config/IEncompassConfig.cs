using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.Config
{
    public interface IEncompassConfig
    {
        bool Init(Session session);
        bool Reload(Session session);

        /**
         * Get value of key, return default if key does not exist
         */
        string GetValue(string key, string defaultVal = null);
        bool GetValue(string value, bool defaultValue);
        ICollection<string> GetKeys();
    }
}
