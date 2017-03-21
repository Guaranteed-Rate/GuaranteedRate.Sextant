using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.RemotingServices;

namespace GuaranteedRate.Sextant.EncompassUtils
{
    class RulesUtils
    {
        public static void ExtractAllRules(Session session)
        {
            
        }

        public static void ExtractAllRules(Session session, BpmCategory ruleType, string resourceName)
        {
            var rulesManager = (BpmManager)Session.DefaultInstance.BPM.GetBpmManager(ruleType);
            BizRuleInfo[] bri = rulesManager.GetAllRules();
            foreach (var rule in bri)
            {
                //rule.g
            }
        }
    }
}
