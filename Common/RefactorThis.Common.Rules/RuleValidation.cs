using RefactorThis.Common.Interfaces.Rules;

namespace RefactorThis.Common.Rules
{
    public class RuleValidation : IRuleValidation
    {
        public string Message { get; set; }
        public string Code { get; set; }
    }
}
