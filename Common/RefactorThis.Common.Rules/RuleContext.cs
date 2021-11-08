using RefactorThis.Common.Interfaces.Rules;
using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Common.Rules
{
    public class RuleContext : IRuleContext
    {
        public RuleContext()
        {
            Validations = new List<IRuleValidation>();
            Information = new List<string>();
        }

        public List<IRuleValidation> Validations { get; set; }
        public List<string> Information { get; set; }

        public bool IsValid()
        {
            return !Validations.Any();
        }
    }
}
