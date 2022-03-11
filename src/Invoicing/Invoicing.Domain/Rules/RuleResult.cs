namespace Invoicing.Domain.Rules
{
    public class RuleResult
    {
        public bool IsValid { get; }

        public string Message { get; }

        public RuleResult(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }
    }
}
