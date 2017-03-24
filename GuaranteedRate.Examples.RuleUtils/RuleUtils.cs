using System;
using System.Collections;
using System.Collections.Generic;
using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.RemotingServices;
using GuaranteedRate.Util.Rules;

namespace GuaranteedRate.Examples.RuleUtils
{
    class RuleUtils
    {
        static int Main(string[] args)
        {
            if (args != null && args.Length == 4)
            {
                var encompass = args[0];
                var user = args[1];
                var pw = args[2];
                var outputFile = args[3];

                List<RuleDetails> rules = new List<RuleDetails>(); //Leaving as List to leave AddRange accessable

                foreach (BpmCategory category in GuaranteedRate.Util.Rules.RuleUtils.CategoryResourceName.Keys)
                {
                    rules.AddRange(GuaranteedRate.Util.Rules.RuleUtils.ExtractRule(args[0], args[1], args[2], category));
                }

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(outputFile, true))
                {
                    foreach (RuleDetails rule in rules)
                    {
                        file.WriteLine($"\"{rule.Category}\",\"{rule.Name}\",\"{rule.id}\",\"{rule.ModifiedBy}\",\"{rule.LastModified}\",\"{rule.Status}\",\"{rule.RuleXml}\",");
                    }
                }
                return 1;
            }
            else
            {
                Console.WriteLine("Usage[Encompass url] [Encompass User] [Encompass Password]  [output file name]");
                return 0;
            }
        }
    }
}
