using RefactorThis.Invoicing.Domain.Services.Rules.Actions;
using RefactorThis.Common.Interfaces.Rules;
using RefactorThis.Common.Rules;
using RefactorThis.Invoicing.Domain.Interfaces.Rules;
using System.Collections.Generic;

namespace RefactorThis.Invoicing.Domain.Services.Rules
{
    public class InvoicingRulesEngine : RuleEngineService, IInvoicingRulesEngine
    {
        public InvoicingRulesEngine() 
        {            
            MasterRulesSet = GetRules();
        }

        private static Dictionary<string, List<IRule>> GetRules()
        {
            var rules = new Dictionary<string, List<IRule>>
            {
                { Rules.InvoicingRulesSet.ProcessPayment, ProcessPaymentRules() }                
            };

            return rules;
        }

        private static List<IRule> ProcessPaymentRules()
        {
            var rulesList = new List<IRule>();
            var ruleActions = new List<IRuleAction>
            {
                new ProcessPaymentValidator()
            };            
            rulesList.Add(new Rule
            {                
                Actions = ruleActions
            });
            return rulesList;
        }

    }
}
