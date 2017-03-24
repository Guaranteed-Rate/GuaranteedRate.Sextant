using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.RemotingServices;
using System;
using System.Collections.Generic;

namespace GuaranteedRate.Util.Rules
{
    public class RuleUtils
    {
        public static readonly IDictionary<BpmCategory, string> CategoryResourceName = new Dictionary<BpmCategory, string>()
        {
            {BpmCategory.AutomatedConditions,       "automatedconditions"},
            //{BpmCategory.Document,                  "inputformlist"},//Additional information: Unable to cast object of type 'EllieMae.EMLite.RemotingServices.DocumentAccessRuleManager' to type 'EllieMae.EMLite.RemotingServices.BpmManager'.
            //{BpmCategory.EmailTriggers,             "milestonecompletions"},//Throws An unhandled exception of type 'System.Exception' occurred in ClientSession.dll
            {BpmCategory.FieldAccess,               "personaaccesstofields"},
            {BpmCategory.FieldRules,                "fieldrules"},
            {BpmCategory.InputForms,                "inputformlist"},
            {BpmCategory.LoanAccess,                "milestonecompletions"},
            {BpmCategory.LoanActionAccess,          "personaaccesstoloans"},//TODO: Check
            {BpmCategory.LoanActionCompletionRules, "milestonecompletions"},//TODO: Check
            //{BpmCategory.LoanFolder,                "inputformlist"},//Additional information: Unable to cast object of type 'EllieMae.EMLite.RemotingServices.LoanFolderRuleManager' to type 'EllieMae.EMLite.RemotingServices.BpmManager'.
            //{BpmCategory.Milestones,                "milestonecompletions"},//Additional information: Unexpected server error: Object reference not set to an instance of an object.
            {BpmCategory.MilestoneRules,            "milestonecompletions"},
            {BpmCategory.PrintForms,                "fieldtriggers"},
            {BpmCategory.PrintSelection,            "printautoselection"},
            {BpmCategory.Triggers,                  "fieldtriggers"},
            //{BpmCategory.Workflow,                  "milestonecompletions"}//Additional information: Unable to cast object of type 'EllieMae.EMLite.RemotingServices.WorkflowManager' to type 'EllieMae.EMLite.RemotingServices.BpmManager'.
        };

        public static void ExtractRule(string url, string user, string pw, BpmCategory category)
        {
            //This is not the same session obj as the SDK
            Session.Start(url, user, pw, string.Empty);

            /*
            var workflow = (WorkflowManager) Session.DefaultInstance.BPM.GetBpmManager(BpmCategory.Workflow);
            var alerts = workflow.GetAllMilestoneAlertMessages();
            foreach (var obj in alerts)
            {
                Console.WriteLine(obj.GetType());
            }
            */

            var rulesManager = (BpmManager)Session.DefaultInstance.BPM.GetBpmManager(category);
            BizRuleInfo[] rules = rulesManager.GetAllRules();
            foreach (var rule in rules)
            {
                Console.WriteLine($"Name={rule.RuleName} |ModifiedBy={rule.LastModifiedByFullName} |RuleID= {rule.RuleID}");
                Console.WriteLine($"LastModTime={rule.LastModTime} |MilestoneID={rule.MilestoneID}\n\n");

                var rp = new RuleProxy(rule, CategoryResourceName[category]);
                Console.WriteLine(rp.Export());
                Console.WriteLine("");
                //Console.WriteLine($"{rule.AdvancedCodeXML}");
                //Console.WriteLine(rp.SimpleExport());
            }

            Session.End();

        }
    }
}
