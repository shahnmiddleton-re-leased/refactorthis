using RefactorThis.Common.Interfaces.Rules;
using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Common.Rules
{
    public class RuleEngineService : IRuleEngineService
    {
        protected Dictionary<string, List<IRule>> MasterRulesSet;
        
        public virtual bool ExecuteRules(IRuleContext context, string rulesetType)
        {
            IEnumerable<IRule> ruleList = MasterRulesSet[rulesetType];
            foreach (var rule in ruleList)
            {                
                foreach (var action in rule.Actions)
                {                        
                    action.Process(context);                        
                }                
            }

            return !context.Validations.Any();
        }
    }
}
