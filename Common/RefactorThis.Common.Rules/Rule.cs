using RefactorThis.Common.Interfaces.Rules;
using System.Collections.Generic;

namespace RefactorThis.Common.Rules
{
    public class Rule : IRule
    {
        public Rule()
        {           
            Actions = new List<IRuleAction>();            
        }

        public List<IRuleAction> Actions { get; set; }        
    }
}
