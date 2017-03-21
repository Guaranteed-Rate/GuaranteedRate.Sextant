using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.RemotingServices;
using System;

namespace GuaranteedRate.Util.Rules
{
    public class RuleUtils
    {
        public static void ExtractRule(string url, string user, string pw)
        {
            //This is not the same session obj as the SDK
            Session.Start(url, user, pw, string.Empty);
            var rulesManager = (BpmManager)Session.DefaultInstance.BPM.GetBpmManager(BpmCategory.MilestoneRules);
            /*
            return rulesManager.GetAllRules()
                .Select(ruleInfo => new RuleProxy<TRuleInfo>((TRuleInfo)ruleInfo, importExportResource))
                .ToList();
            */
            BizRuleInfo[] rules = rulesManager.GetAllRules();
            foreach (var rule in rules)
            {
                //RuleProxy rp = new RuleProxy(rule, "milestonecompletions");
                //Console.WriteLine(rp.SimpleExport());
                Console.WriteLine($"Name={rule.RuleName} |ModifiedBy={rule.LastModifiedByFullName} |RuleID= {rule.RuleID}");
                Console.WriteLine($"LastModTime={rule.LastModTime} |MilestoneID={rule.MilestoneID}\n\n");

                var rp = new RuleProxy<BizRuleInfo>(rule, "milestonecompletions");
                Console.WriteLine(rp.Export());
                Console.WriteLine("");
                //Console.WriteLine($"{rule.AdvancedCodeXML}");
                //Console.WriteLine(rp.SimpleExport());
            }

            Session.End();

        }
    }
}
