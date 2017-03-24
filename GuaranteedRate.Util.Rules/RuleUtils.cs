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
            {BpmCategory.LoanActionAccess,          "personaaccesstoloans"},//empty
            {BpmCategory.LoanActionCompletionRules, "milestonecompletions"},//empty
            //{BpmCategory.LoanFolder,                "inputformlist"},//Additional information: Unable to cast object of type 'EllieMae.EMLite.RemotingServices.LoanFolderRuleManager' to type 'EllieMae.EMLite.RemotingServices.BpmManager'.
            //{BpmCategory.Milestones,                "milestonecompletions"},//Additional information: Unexpected server error: Object reference not set to an instance of an object.
            {BpmCategory.MilestoneRules,            "milestonecompletions"},
            {BpmCategory.PrintForms,                "fieldtriggers"},
            {BpmCategory.PrintSelection,            "printautoselection"},
            {BpmCategory.Triggers,                  "fieldtriggers"},
            //{BpmCategory.Workflow,                  "milestonecompletions"}//Additional information: Unable to cast object of type 'EllieMae.EMLite.RemotingServices.WorkflowManager' to type 'EllieMae.EMLite.RemotingServices.BpmManager'.
        };

        public static IList<RuleDetails> ExtractRule(string url, string user, string pw, BpmCategory category)
        {
            //This is not the same session obj as the SDK
            Session.Start(url, user, pw, string.Empty);

            var details = new List<RuleDetails>();

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
                var ruleDetail = new RuleDetails();
                details.Add(ruleDetail);
                ruleDetail.Category = category.ToString();
                ruleDetail.Name = rule.RuleName;
                ruleDetail.LastModified = rule.LastModTime;
                ruleDetail.ModifiedBy = rule.LastModifiedByUserId;
                ruleDetail.id = rule.RuleID;
                ruleDetail.Status = rule.Status.ToString();

                var rp = new RuleProxy(rule, CategoryResourceName[category]);
                ruleDetail.RuleXml = rp.Export();

                /*
                Console.WriteLine($"Category={category} | Name={rule.RuleName} |ModifiedBy={rule.LastModifiedByFullName} |RuleID= {rule.RuleID}");
                Console.WriteLine($"LastModTime={rule.LastModTime} |MilestoneID={rule.MilestoneID}\n\n");
                Console.WriteLine($"RuleType={rule.RuleType} |Inactive={rule.Inactive}\n\n");
                Console.WriteLine($"Status={rule.Status} |Condition={rule.Condition}\n\n");
                Console.WriteLine($"IsGeneralRule={rule.IsGeneralRule} |ConditionState={rule.ConditionState}\n\n");

                /*
                Console.WriteLine(rp.Export());
                Console.WriteLine("");
                //Console.WriteLine($"{rule.AdvancedCodeXML}");
                //Console.WriteLine(rp.SimpleExport());
                */
            }

            Session.End();
            return details;
        }
    }
}
