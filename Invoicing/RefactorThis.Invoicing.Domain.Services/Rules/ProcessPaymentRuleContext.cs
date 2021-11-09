using RefactorThis.Common.Rules;
using RefactorThis.Invoicing.Domain.Interfaces.Rules;
using RefactorThis.Invoicing.Domain.Model;

namespace RefactorThis.Invoicing.Domain.Services.Rules
{
    public class ProcessPaymentRuleContext : RuleContext, IProcessPaymentRuleContext
    {
        public Invoice Invoice { get; set; }
        public Payment Payment { get; set; }
    }
}
